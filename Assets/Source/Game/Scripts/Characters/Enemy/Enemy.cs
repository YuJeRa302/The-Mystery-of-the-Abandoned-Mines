using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Game.Scripts.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class Enemy : PoolObject
    {
        [SerializeField] protected Pool Pool;

        private readonly System.Random _rnd = new();

        [SerializeField] private EnemyStateMachineExample _stateMachine;
        [SerializeField] private EnemyAnimation _animationController;
        [SerializeField] private Transform _damageEffectContainer;
        [SerializeField] private HealthBarView _healthView;

        private int _damage;
        private float _attackDistance;
        private float _attackDelay;
        private int _health = 20;
        private float _speed;
        private float _baseMoveSpeed;
        private int _currentLvlRoom;
        private bool _isDead;
        private float _currentHealth;
        private int _experience;
        private int _score;
        private int _gold;
        private int _upgradeExperience;
        private Rigidbody _rigidbody;
        private Coroutine _stunDamage;
        private Coroutine _burnDamage;
        private Coroutine _slowDamage;
        private Coroutine _repulsiveDamage;
        private List<PoolObject> _spawnedEffects = new();
        private AudioClip _deathAudio;
        private AudioClip _hitAudio;
        private int _tier;

        public float AttackDelay => _attackDelay;
        public float Damage => _damage;
        public float Speed => _speed;
        public float AttackDistance => _attackDistance;
        public int ExperienceReward => _experience;
        public int Score => _score;
        public int GoldReward => _gold;
        public int UpgradeExperienceReward => _upgradeExperience;
        public int MaxHealth => _health;
        public float CurrentHealth => _currentHealth;
        public EnemyAnimation AnimationStateController => _animationController;
        public AudioClip DeathAudio => _deathAudio;
        public Pool EnemyBulletPool => Pool;

        public event Action<Enemy> Died;
        public event Action Stuned;
        public event Action StunEnded;
        public event Action<float> DamageTaked;
        public event Action HealthChanged;
        public event Action<AudioClip> PlayerAttacked;

        private void OnDisable()
        {
            foreach (var spawnedParticle in _spawnedEffects)
            {
                spawnedParticle.ReturnObjectPool();
            }

            CoroutineStop(_slowDamage);
            CoroutineStop(_stunDamage);
            CoroutineStop(_repulsiveDamage);
            CoroutineStop(_burnDamage);
        }

        private void OnDestroy()
        {
            _animationController.Attacked -= OnEnemyAttack;
        }

        public virtual void Initialize(Player player, int lvlRoom, EnemyData data, int tire)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _currentLvlRoom = lvlRoom;
            _tier = tire;
            Fill(data);
            _stateMachine.InitializeStateMachine(player);
            _healthView.Initialize(this);
            _currentHealth = _health;
            _animationController.Attacked += OnEnemyAttack;
            HealthChanged?.Invoke();
        }

        public virtual void ResetEnemy(int lvlRoom)
        {
            _isDead = false;
            _currentHealth = _health;

            if (_currentLvlRoom < lvlRoom)
            {
                _health = _health * (1 + lvlRoom / GameConstants.EnemyBoostDivider);
                _damage = _damage * (1 + lvlRoom / GameConstants.EnemyBoostDivider);
            }

            _stateMachine.ResetState();
            _currentHealth = _health;
            HealthChanged?.Invoke();
        }

        public void TakeDamage(DamageSource damageSource)
        {
            if (damageSource == null)
                return;

            ExtractDamageParameters(damageSource,
                out float chance, out float duration,
                out float repulsive, out float gradual,
                out float slowDown);

            ApplyDamage(damageSource.Damage);

            if (_isDead)
                return;

            switch (damageSource.TypeDamage)
            {
                case TypeDamage.PhysicalDamage:
                    break;
                case TypeDamage.StunDamage:
                    TryApplyEffect(chance, () =>
                        _stunDamage = StartCoroutine(Stun(duration, damageSource.PoolParticle)),
                        _stunDamage);
                    break;
                case TypeDamage.RepulsiveDamage:
                    _repulsiveDamage = RestartCoroutine(_repulsiveDamage,
                        () => Repulsive(repulsive));
                    break;
                case TypeDamage.BurningDamage:
                    _burnDamage = RestartCoroutine(_burnDamage,
                        () => Burn(gradual, duration, damageSource.PoolParticle));
                    break;
                case TypeDamage.SlowedDamage:
                    _slowDamage = RestartCoroutine(_slowDamage,
                        () => Slowed(duration, slowDown, damageSource.PoolParticle));
                    break;
            }
        }

        protected override void ReturnToPool()
        {
            CoroutineStop(_slowDamage);
            CoroutineStop(_stunDamage);
            CoroutineStop(_repulsiveDamage);
            CoroutineStop(_burnDamage);
            base.ReturnToPool();
            _isDead = true;
            _speed = _baseMoveSpeed;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _currentHealth = _health;
        }

        private void ExtractDamageParameters(
            DamageSource damageSource,
            out float chance, out float duration,
            out float repulsive, out float gradual,
            out float slowDown)
        {
            chance = 0;
            duration = 0;
            repulsive = 0;
            gradual = 0;
            slowDown = 0;

            foreach (var parameter in damageSource.DamageParameters)
            {
                switch (parameter.TypeDamageParameter)
                {
                    case TypeDamageParameter.Chance:
                        chance = parameter.Value;
                        break;
                    case TypeDamageParameter.Duration:
                        duration = parameter.Value;
                        break;
                    case TypeDamageParameter.Repulsive:
                        repulsive = parameter.Value;
                        break;
                    case TypeDamageParameter.Gradual:
                        gradual = parameter.Value;
                        break;
                    case TypeDamageParameter.Slowdown:
                        slowDown = parameter.Value;
                        break;
                }
            }
        }

        private void TryApplyEffect(float chance, Action effectAction, Coroutine runningCoroutine)
        {
            if (_rnd.Next(0, 100) <= chance)
            {
                CoroutineStop(runningCoroutine);
                effectAction();
            }
        }

        private Coroutine RestartCoroutine(Coroutine runningCoroutine, Func<IEnumerator> coroutineMethod)
        {
            CoroutineStop(runningCoroutine);

            return StartCoroutine(coroutineMethod());
        }

        private void ApplyDamage(float damage)
        {
            if (damage < 0)
                return;

            if (_currentHealth <= 0)
                return;

            _currentHealth -= damage;
            DamageTaked?.Invoke(damage);
            HealthChanged?.Invoke();

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _isDead = true;
                Died?.Invoke(this);
                ReturnToPool();
            }
        }

        private void Fill(EnemyData data)
        {
            _health = data.EnemyStats[_tier].Health;
            _damage = data.EnemyStats[_tier].Damage;
            _speed = data.EnemyStats[_tier].MoveSpeed;
            _attackDelay = data.EnemyStats[_tier].AttackDelay;
            _attackDistance = data.EnemyStats[_tier].AttackDistance;
            _gold = data.EnemyStats[_tier].GoldReward;
            _score = data.EnemyStats[_tier].Score;
            _experience = data.EnemyStats[_tier].ExperienceReward;
            _upgradeExperience = data.EnemyStats[_tier].UpgradeExperienceReward;
            _baseMoveSpeed = _speed;
            _deathAudio = data.AudioClipDie;
            _hitAudio = data.Hit;
        }

        private void OnEnemyAttack()
        {
            PlayerAttacked?.Invoke(_hitAudio);
        }

        private void CreateDamageParticle(PoolParticle poolParticle)
        {
            PoolParticle particle;

            if (Pool.TryPoolObject(poolParticle.gameObject, out PoolObject poolObject))
            {
                particle = poolObject as PoolParticle;
                particle.transform.position = _damageEffectContainer.position;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = Instantiate(poolParticle, _damageEffectContainer);
                Pool.InstantiatePoolObject(particle, poolObject.name);
                _spawnedEffects.Add(particle);
            }
        }

        private void DisableParticle(PoolParticle particle)
        {
            foreach (var spawnedParticle in _spawnedEffects)
            {
                if (particle.name == spawnedParticle.NameObject)
                    if (spawnedParticle.isActiveAndEnabled)
                        spawnedParticle.ReturnObjectPool();
            }
        }

        private void CoroutineStop(Coroutine coroutine)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        private IEnumerator Burn(float damage, float time, PoolParticle particle)
        {
            float currentTime = 0;
            float pastSeconds = 0;
            float delayDamage = 1f;

            CreateDamageParticle(particle);

            while (currentTime <= time)
            {
                pastSeconds += Time.deltaTime;

                if (pastSeconds >= delayDamage)
                {
                    ApplyDamage(damage);
                    pastSeconds = 0;
                    currentTime++;
                }

                yield return null;
            }

            DisableParticle(particle);
        }

        private IEnumerator Repulsive(float value)
        {
            float currentTime = 0;
            _rigidbody.isKinematic = false;

            while (currentTime <= 0.17f)
            {
                _rigidbody.AddForce(transform.forward * -value, ForceMode.Impulse);
                currentTime += Time.deltaTime;
                yield return null;
            }

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }

        private IEnumerator Stun(float duration, PoolParticle particle)
        {
            CreateDamageParticle(particle);
            Stuned?.Invoke();
            yield return new WaitForSeconds(duration);
            StunEnded?.Invoke();
            DisableParticle(particle);
        }

        private IEnumerator Slowed(float duration, float valueSlowed, PoolParticle particle)
        {
            float currentTime = 0;
            CreateDamageParticle(particle);
            _baseMoveSpeed = _speed;
            _speed = _speed * (1 - valueSlowed / 100);

            while (currentTime <= duration)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }

            _speed = _baseMoveSpeed;
            DisableParticle(particle);
        }
    }
}