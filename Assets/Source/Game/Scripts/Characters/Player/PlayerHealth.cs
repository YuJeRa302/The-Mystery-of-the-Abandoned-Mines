using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerHealth : IDisposable
    {
        private readonly int _minHealth = 0;
        private readonly int _delayHealing = 1;

        private Player _player;
        private ICoroutineRunner _coroutineRunner;

        private int _maxHealth = 100;
        private int _currentHealth = 0;
        private Coroutine _regeneration;
        private LevelObserver _levelObserver;

        public event Action DamageTaked;
        public event Action<int> HealthChanged;
        public event Action PlayerDied;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;

        public PlayerHealth(LevelObserver levelObserver, Player player,ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
            _levelObserver = levelObserver;
            _player = player;
            _currentHealth = 50;
            _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());
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
                _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());
        }

        private void OnResumeGame()
        {
            if (_regeneration != null)
                _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());
        }

        private void OnHealthChanged(int value) 
        {
            if (_regeneration != null)
            {
                return;
            }
            else 
            {
                _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());
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

        public void Dispose()
        {
            _levelObserver.GamePaused -= OnPauseGame;
            _levelObserver.GameResumed -= OnResumeGame;
            _player.PlayerStats.MaxHealthChanged -= OnMaxHealthChanged;
            HealthChanged -= OnHealthChanged;

            if (_regeneration != null)
                _coroutineRunner.StopCoroutine(_regeneration);

            GC.SuppressFinalize(this);
        }
    }
}