using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Assets.Source.Game.Scripts
{
    public class Enemy : PoolObject
    {
        [SerializeField] private EnemyStateMashineExample _stateMashine;
        [SerializeField] private EnemyAnimation _animationController;
        [SerializeField] private Transform _damageEffectConteiner;
        [SerializeField] protected Pool _pool;
        [SerializeField] private HealthBarView _healthView;

        private readonly System.Random _rnd = new();
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
        private Player _player;
        private Rigidbody _rigidbody;
        private Coroutine _stunDamage;
        private Coroutine _burnDamage;
        private Coroutine _slowDamage;
        private Coroutine _repulsiveDamage;
        private List<PoolObject> _spawnedEffects = new();

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
        public EnemyStateMashineExample StateMashine => _stateMashine;

        public event Action<Enemy> Died;
        public event Action Stuned;
        public event Action EndedStun;
        public event Action<float> DamageTaked;
        public event Action HealthChenged;

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

        public virtual void Initialize(Player player, int lvlRoom, EnemyData data)
        {
            _player = player;
            _rigidbody = GetComponent<Rigidbody>();
            _currentLvlRoom = lvlRoom;
            Fill(data);
            _stateMashine.InitializeStateMashine(player);
            _healthView.Initialize(this);
            _currentHealth = _health;
            HealthChenged?.Invoke();
        }

        public void ResetEnemy(int lvlRoom)
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
            HealthChenged?.Invoke();
        }

        private void Fill(EnemyData data)
        {
            _health = data.Health;
            _damage = data.Damage;
            _speed = data.MoveSpeed;
            _attackDelay = data.AttackDelay;
            _attackDistance = data.AttackDistance;
            _id = data.Id;
            _gold = data.GoldReward;
            _score = data.Score;
            _experience = data.ExperienceReward;
            _upgradeExperience = data.UpgradeExperienceReward;
            _baseMoveSpeed = _speed;
        }

        private void TakeDamage(float damage)//+��� �����
        {
            if (damage < 0)
                return;

            if (_currentHealth <= 0)
                return;

            _currentHealth -= damage;
            DamageTaked?.Invoke(damage);
            HealthChenged?.Invoke();

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _isDead = true;
                Died?.Invoke(this);
                ReturnToPool();
            }
        }

        public void TakeDamageTest(DamageParametr damage)
        {
            float damageValue = 0;
            float chance = 0;
            float duration = 0;
            float repulsive = 0;
            float gradualDamage = 0;
            float slowDown = 0;

            foreach (var parametr in damage.DamageSupportivePatametrs)
            {
                if (parametr.SupportivePatametr == TypeSupportivePatametr.Damage)
                {
                    damageValue = parametr.Value;
                }
                else if (parametr.SupportivePatametr == TypeSupportivePatametr.Chance)
                {
                    chance = parametr.Value;
                }
                else if (parametr.SupportivePatametr == TypeSupportivePatametr.Duration)
                {
                    duration = parametr.Value;
                }
                else if (parametr.SupportivePatametr == TypeSupportivePatametr.Repulsive)
                {
                    repulsive = parametr.Value;
                }
                else if (parametr.SupportivePatametr == TypeSupportivePatametr.GradualDamage)
                {
                    gradualDamage = parametr.Value;
                }
                else if (parametr.SupportivePatametr == TypeSupportivePatametr.Slowdown)
                {
                    slowDown = parametr.Value;
                }
            }

            if (damage.TypeDamage== TypeDamage.PhysicalDamage)
            {
                foreach (var item in damage.DamageSupportivePatametrs)
                {
                    if (item.SupportivePatametr == TypeSupportivePatametr.Damage)
                    {
                        TakeDamage(item.Value);
                    }
                }
            }

            if (damage.TypeDamage == TypeDamage.StunDamage)
            {
                TakeDamage(damageValue);

                if (_isDead)
                    return;

                if (_rnd.Next(0, 100) <= chance)
                {
                    CorountineStop(_stunDamage);
                    _stunDamage = StartCoroutine(Stun(duration, damage.Particle));
                }
            }
            else if (damage.TypeDamage == TypeDamage.RepulsiveDamage)
            {
                TakeDamage(damageValue);

                if (_isDead)
                    return;

                CorountineStop(_repulsiveDamage);
                _repulsiveDamage = StartCoroutine(Repulsive(repulsive));
            }
            else if (damage.TypeDamage == TypeDamage.BurningDamage)
            {
                TakeDamage(damageValue);

                if (_isDead)
                    return;

                CorountineStop(_burnDamage);
                _burnDamage = StartCoroutine(Burn(gradualDamage, duration, damage.Particle));
            }
            else if (damage.TypeDamage == TypeDamage.SlowedDamage)
            {
                TakeDamage(damageValue);

                if (_isDead)
                    return;

                CorountineStop(_slowDamage);
                _slowDamage = StartCoroutine(Slowed(duration, slowDown, damage.Particle));
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
            _currentHealth = _health;
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

            CreateDamageParticle(particle);

            while (currentTime <= time)
            {
                pastSeconds += Time.deltaTime;

                if (pastSeconds >= 1)
                {
                    TakeDamage(damage);
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

            while (currentTime <= 0.3f)
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