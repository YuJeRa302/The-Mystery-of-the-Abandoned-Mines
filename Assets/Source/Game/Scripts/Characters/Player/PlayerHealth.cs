using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerHealth : MonoBehaviour
    {
        private readonly int _minHealth = 0;
        private readonly int _delayHealing = 1;

        [SerializeField] private Player _player;

        private int _maxHealth = 100;
        private int _currentHealth = 0;
        private IEnumerator _regeneration;
        private LevelObserver _levelObserver;

        public event Action DamageTaked;
        public event Action<int> HealthChanged;
        public event Action PlayerDied;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;

        private void OnDestroy()
        {
            _levelObserver.GamePaused -= OnPauseGame;
            _levelObserver.GameResumed -= OnResumeGame;
            _player.PlayerStats.MaxHealthChanged -= OnMaxHealthChanged;
            HealthChanged -= OnHealthChanged;
        }

        public void Initialize(LevelObserver levelObserver)
        {
            _levelObserver = levelObserver;
            _currentHealth = 50;
            _regeneration = RegenerationHealth();
            StartCoroutine(_regeneration);
            AddListener();
        }

        public void TakeDamage(int damage)
        {
            if (_currentHealth == _minHealth)
                SetPlayerDie();

            if (_currentHealth > _minHealth)
            {
                DamageTaked?.Invoke();

                var currentDamage = (damage - _player.PlayerStats.Armor);

                if (currentDamage < _minHealth)
                    currentDamage = _minHealth;

                _currentHealth = Mathf.Clamp(_currentHealth - currentDamage, _minHealth, _maxHealth);
                HealthChanged?.Invoke(_currentHealth);
            }
        }

        private void AddListener() 
        {
            _levelObserver.GamePaused += OnPauseGame;
            _levelObserver.GameResumed += OnResumeGame;
            _player.PlayerStats.MaxHealthChanged += OnMaxHealthChanged;
            HealthChanged += OnHealthChanged;
        }

        private void OnMaxHealthChanged(int value) 
        {
            _maxHealth = value;
        }

        private void OnPauseGame()
        {
            if (_regeneration != null)
                StopCoroutine(_regeneration);
        }

        private void OnResumeGame()
        {
            if (_regeneration != null)
                StartCoroutine(_regeneration);
        }

        private void OnHealthChanged(int value) 
        {
            if (_regeneration != null)
            {
                return;
            }
            else 
            {
                _regeneration = RegenerationHealth();
                StartCoroutine(_regeneration);
            }
        }

        private IEnumerator RegenerationHealth()
        {
            while (_currentHealth < _maxHealth)
            {
                yield return new WaitForSeconds(_delayHealing);
                _currentHealth += _player.PlayerStats.Regeneration;
                HealthChanged?.Invoke(_currentHealth);
            }

            _regeneration = null;
        }

        private void SetPlayerDie()
        {
            //Destroy(gameObject);
            PlayerDied?.Invoke();
        }
    }
}