using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Enemy : PoolObject
    {
        [SerializeField] private EnemyStateMashineExample _stateMashine;
        [SerializeField] private float _attackDelay;
        [SerializeField] protected float _damage;
        [SerializeField] private EnemyAnimation _animationController;
        [SerializeField] private float _health = 20f;
        [SerializeField] private float _speed;
        [SerializeField] private float _attackDistance;
        [SerializeField] private Transform _damageEffectConteiner;
        [SerializeField] protected Pool _pool;

        private readonly System.Random _rnd = new();
        private int _id;
        private bool _isDead;
        private float _currentHealth;
        private Player _player;
        private Rigidbody _rigidbody;
        private Coroutine _coroutine;
        private List<PoolObject> _spawnedEffects = new();

        public int Id => _id;
        public float AttackDelay => _attackDelay;
        public float Damage => _damage;
        public float Speed => _speed;
        public float AttackDistance => _attackDistance;
        public EnemyAnimation AnimationStateController => _animationController;
        public EnemyStateMashineExample StateMashine => _stateMashine;

        public event Action<Enemy> Died;
        public event Action Stuned;
        public event Action EndedStun;

        public void Initialize(Player player, int id)
        {
            _player = player;
            _rigidbody = GetComponent<Rigidbody>();
            _currentHealth = _health;
            _id = id;
            _stateMashine.InitializeStateMashine(player);
        }

        public void ResetEnemy()
        {
            _isDead = true;
            _currentHealth = _health;
            _stateMashine.ResetState();
        }

        public void UpdateEnemyStats(int lvlRoom)
        {
            _health = _health * (1 + lvlRoom / 10);
            _damage = _damage * (1 + lvlRoom / 10);
        }

        public void TakeDamage(float damage)//+тип урона
        {
            if (damage < 0)
                return;

            if (_currentHealth <= 0)
                return;

            _currentHealth -= damage;

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

            foreach (var parametr in damage.DamageSupportivePatametrs)
            {
                if (parametr.SupportivePatametr == TypeSupportivePatametr.Damage)
                {
                    damageValue = parametr.Value;
                }
                else if (parametr.SupportivePatametr == TypeSupportivePatametr.Chence)
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

                if (_rnd.Next(0, 100) <= chance)
                {
                    _coroutine = StartCoroutine(Stun(duration, damage.Particle));
                }
            }
            else if (damage.TypeDamage == TypeDamage.RepulsiveDamage)
            {
                TakeDamage(damageValue);
                _coroutine = StartCoroutine(Repulsive(repulsive));
            }
            else if (damage.TypeDamage == TypeDamage.BurningDamage)
            {
                TakeDamage(damageValue);
                _coroutine = StartCoroutine(Burn(gradualDamage, duration, damage.Particle));
            }
        }

        protected override void ReturnToPool()
        {
            base.ReturnToPool();
            _isDead = false;
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

        private IEnumerator Burn(float damage, float time, PoolParticle particle)
        {
            float currentTime = 0;

            CreateDamageParticle(particle);

            while (currentTime < time)
            {
                TakeDamage(damage);
                currentTime += Time.deltaTime;
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
    }
}