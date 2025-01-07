using System;
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

        private int _id;
        private bool _isDead;
        private float _currentHealth;

        public int Id => _id;
        public float AttackDelay => _attackDelay;
        public float Damage => _damage;
        public float Speed => _speed;
        public float AttackDistance => _attackDistance;
        public EnemyAnimation AnimationStateController => _animationController;
        public EnemyStateMashineExample StateMashine => _stateMashine;

        public event Action<Enemy> Died;

        public void Initialize(Player player, int id)
        {
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

        public void TakeDamage(float damage)
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

        protected override void ReturnToPool()
        {
            base.ReturnToPool();
            _isDead = false;
            _currentHealth = _health;
        }
    }
}