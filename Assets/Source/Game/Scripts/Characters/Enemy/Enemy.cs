using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Enemy : PoolObject
    {
        private readonly System.Random _rnd = new();

        [SerializeField] private EnemyStateMashineExample _stateMashine;
        [SerializeField] private EnemyAnimation _animationController;
        [SerializeField] private Transform _damageEffectConteiner;
        [SerializeField] protected Pool _pool;
        [SerializeField] private HealthBarView _healthView;

        private float _attackDistance;
        private float _attackDelay;
        private int _health = 20;
        protected int _damage;
        private float _speed;
        private float _baseMoveSpeed;
        private int _id;
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
        private int _tire;

        public int Id => _id;
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
        public AudioClip HitAudio => _hitAudio;

        public event Action<Enemy> Died;
        public event Action Stuned;
        public event Action EndedStun;
        public event Action<float> DamageTaked;
        public event Action HealthChanged;
        public event Action<AudioClip> AttackPlayer;

        private void OnDisable()
        {
            foreach (var spawnedParticle in _spawnedEffects)
            {
                spawnedParticle.ReturObjectPool();
            }

            CorountineStop(_slowDamage);
            CorountineStop(_stunDamage);
            CorountineStop(_repulsiveDamage);
            CorountineStop(_burnDamage);
        }

        private void OnDestroy()
        {
            _animationController.Attacked -= OnEnemyAttack;
        }

        public virtual void Initialize(Player player, int lvlRoom, EnemyData data, int tire)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _currentLvlRoom = lvlRoom;
            _tire = tire;
            Fill(data);
            _stateMashine.InitializeStateMashine(player);
            _healthView.Initialize(this);
            _currentHealth = _health;
            _animationController.Attacked += OnEnemyAttack;
            HealthChanged?.Invoke();
        }

        public virtual void ResetEnemy(int lvlRoom)
        {
            _isDead = false;
            _currentHealth = _health;

            if(_currentLvlRoom < lvlRoom)
            {
                _health = _health * (1 + lvlRoom / 10);
                _damage = _damage * (1 + lvlRoom / 10);
            }

            _stateMashine.ResetState();
            _currentHealth = _health;
            HealthChanged?.Invoke();
        }

        public void TakeDamage(DamageSource damageSource)
        {
            float damageValue = damageSource.Damage;
            float chance = 0;
            float duration = 0;
            float repulsive = 0;
            float gradual = 0;
            float slowDown = 0;

            foreach (var parametr in damageSource.DamageParameters)
            {
                switch (parametr.TypeDamageParameter)
                {
                    case TypeDamageParameter.Chance:
                        chance = parametr.Value;
                        break;
                    case TypeDamageParameter.Duration:
                        duration = parametr.Value;
                        break;
                    case TypeDamageParameter.Repulsive:
                        repulsive = parametr.Value;
                        break;
                    case TypeDamageParameter.Gradual:
                        gradual = parametr.Value;
                        break;
                    case TypeDamageParameter.Slowdown:
                        slowDown = parametr.Value;
                        break;
                }
            }

            if (damageSource.TypeDamage == TypeDamage.PhysicalDamage)
                ApplyDamage(damageValue);

            if (damageSource.TypeDamage == TypeDamage.StunDamage)
            {
                ApplyDamage(damageValue);

                if (_isDead)
                    return;

                if (_rnd.Next(0, 100) <= chance)
                {
                    CorountineStop(_stunDamage);
                    _stunDamage = StartCoroutine(Stun(duration, damageSource.PoolParticle));
                }
            }
            else if (damageSource.TypeDamage == TypeDamage.RepulsiveDamage)
            {
                ApplyDamage(damageValue);

                if (_isDead)
                    return;

                CorountineStop(_repulsiveDamage);
                _repulsiveDamage = StartCoroutine(Repulsive(repulsive));
            }
            else if (damageSource.TypeDamage == TypeDamage.BurningDamage)
            {
                ApplyDamage(damageValue);

                if (_isDead)
                    return;

                CorountineStop(_burnDamage);
                _burnDamage = StartCoroutine(Burn(gradual, duration, damageSource.PoolParticle));
            }
            else if (damageSource.TypeDamage == TypeDamage.SlowedDamage)
            {
                ApplyDamage(damageValue);

                if (_isDead)
                    return;

                CorountineStop(_slowDamage);
                _slowDamage = StartCoroutine(Slowed(duration, slowDown, damageSource.PoolParticle));
            }
        }

        protected override void ReturnToPool()
        {
            CorountineStop(_slowDamage);
            CorountineStop(_stunDamage);
            CorountineStop(_repulsiveDamage);
            CorountineStop(_burnDamage);
            base.ReturnToPool();
            _isDead = true;
            _speed = _baseMoveSpeed;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _currentHealth = _health;
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
            _health = data.EnemyStats[_tire].Health;
            _damage = data.EnemyStats[_tire].Damage;
            _speed = data.EnemyStats[_tire].MoveSpeed;
            _attackDelay = data.EnemyStats[_tire].AttackDelay;
            _attackDistance = data.EnemyStats[_tire].AttackDistance;
            _id = data.Id;
            _gold = data.EnemyStats[_tire].GoldReward;
            _score = data.EnemyStats[_tire].Score;
            _experience = data.EnemyStats[_tire].ExperienceReward;
            _upgradeExperience = data.EnemyStats[_tire].UpgradeExperienceReward;
            _baseMoveSpeed = _speed;
            _deathAudio = data.AudioClipDie;
            _hitAudio = data.Hit;
        }

        private void OnEnemyAttack()
        {
            AttackPlayer?.Invoke(_hitAudio);
        }

        private void CreateDamageParticle(PoolParticle poolParticle)
        {
            PoolParticle particle;

            if (_pool.TryPoolObject(poolParticle.gameObject, out PoolObject pollParticle))
            {
                particle = pollParticle as PoolParticle;
                particle.transform.position = _damageEffectConteiner.position;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = Instantiate(poolParticle, _damageEffectConteiner);
                _pool.InstantiatePoolObject(particle, poolParticle.name);
                _spawnedEffects.Add(particle);
            }
        }

        private void DisableParticle(PoolParticle particle)
        {
            foreach (var spawnedParticle in _spawnedEffects)
            {
                if (particle.name == spawnedParticle.NameObject)
                    if (spawnedParticle.isActiveAndEnabled)
                        spawnedParticle.ReturObjectPool();
            }
        }

        private void CorountineStop(Coroutine corontine)
        {
            if (corontine != null)
                StopCoroutine(corontine);
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
            EndedStun?.Invoke();
            DisableParticle(particle);
        }

        private IEnumerator Slowed(float duration, float valueSlowed, PoolParticle particle)
        {
            float currentTime = 0;
            CreateDamageParticle(particle);
            _baseMoveSpeed = _speed;
            _speed = _speed * (1 - valueSlowed/100);

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