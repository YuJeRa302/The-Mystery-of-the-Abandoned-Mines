using System;

namespace Assets.Source.Game.Scripts.Characters
{
    public class EnemyHealth
    {
        private int _maxHealth;
        private float _currentHealth;
        private bool _isDead = false;

        public event Action<float> DamageTaked;
        public event Action HealthChanged;

        public EnemyHealth(int health)
        {
            _maxHealth = health;
            _currentHealth = health;
        }

        public bool IsDead => _isDead;
        public int MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;

        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            _isDead = false;
            HealthChanged?.Invoke();
        }

        public void ApplyDamage(float damage)
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
            }
        }

        public void ChengeMaxHealth(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;
            HealthChanged?.Invoke();
        }
    }
}