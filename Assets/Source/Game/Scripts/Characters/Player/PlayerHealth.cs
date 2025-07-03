using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerHealth : IDisposable
    {
        private readonly Player _player;
        private readonly GamePauseService _gamePauseService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly int _minHealth = 0;
        private readonly int _minDamage = 2;

        private int _maxHealth;
        private int _currentHealth;
        private int _delayHealing = 1;
        private Coroutine _regeneration;
        private Coroutine _timeReduce;
        private bool _canHealind = true;

        public event Action DamageTaked;
        public event Action<int> HealthChanged;
        public event Action PlayerDied;

        public PlayerHealth(Player player, ICoroutineRunner coroutineRunner, GamePauseService gamePauseService, int currentHealth)
        {
            _player = player;
            _coroutineRunner = coroutineRunner;
            _gamePauseService = gamePauseService;
            _currentHealth = currentHealth;
            _maxHealth = currentHealth;
            AddListeners();
            _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());
        }

        public void ReduceHealth(float reduce)
        {
            _currentHealth -= Convert.ToInt32(_currentHealth * (reduce / 100));
            HealthChanged?.Invoke(_currentHealth);
        }

        public void TakeDamage(int damage)
        {
            if (_currentHealth <= _minHealth)
                return;

            if (_currentHealth > _minHealth)
            {
                DamageTaked?.Invoke();
                int currentDamage = damage - _player.Armor;

                if (currentDamage <= 1)
                    currentDamage = _minDamage;

                _currentHealth = Mathf.Clamp(_currentHealth - currentDamage, _minHealth, _maxHealth);
                HealthChanged?.Invoke(_currentHealth);

                if (_currentHealth <= _minHealth)
                    SetPlayerDie();
            }
        }

        public void DisableHealing()
        {
            _canHealind = false;

            if (_regeneration != null && _coroutineRunner != null)
                _coroutineRunner.StopCoroutine(_regeneration);

            if (_timeReduce != null && _coroutineRunner != null)
                _coroutineRunner.StopCoroutine(_timeReduce);
        }

        public void TakeHealing(int heal)
        {
            if (heal < _minHealth)
                return;

            _currentHealth += heal;

            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;

            HealthChanged?.Invoke(_currentHealth);
        }

        public void ChangeMaxHealth(int value, out int currentHealth, out int maxHealth)
        {
            _maxHealth += value;
            currentHealth = _currentHealth;
            maxHealth = _maxHealth;
        }

        public void ApplyHealthUpgrade(int value, out int currentHealth, out int maxHealth)
        {
            _maxHealth += value;
            _currentHealth = _maxHealth;
            currentHealth = _currentHealth;
            maxHealth = _maxHealth;
        }

        public int GetMaxHealth()
        {
            return _maxHealth;
        }

        public int GetCurrentHealth()
        {
            return _currentHealth;
        }

        private void AddListeners()
        {
            _gamePauseService.GamePaused += OnPauseGame;
            _gamePauseService.GameResumed += OnResumeGame;
            HealthChanged += OnHealthChanged;
        }

        private void RemoveListeners()
        {
            _gamePauseService.GamePaused -= OnPauseGame;
            _gamePauseService.GameResumed -= OnResumeGame;
            HealthChanged -= OnHealthChanged;
        }

        private void OnPauseGame(bool state)
        {
            if (_regeneration != null)
                _coroutineRunner.StopCoroutine(_regeneration);

            if (_timeReduce != null)
                _coroutineRunner.StopCoroutine(_timeReduce);
        }

        private void OnResumeGame(bool state)
        {
            if (_regeneration != null)
                _coroutineRunner.StopCoroutine(_regeneration);

            if (_timeReduce != null)
                _coroutineRunner.StopCoroutine(_timeReduce);

            if (_canHealind)
                _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());
        }

        private void OnHealthChanged(int value)
        {
            if (_regeneration != null)
                return;
            else
                _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());
        }

        private IEnumerator RegenerationHealth()
        {
            while (_currentHealth < _maxHealth)
            {
                yield return new WaitForSeconds(_delayHealing);
                _currentHealth += _player.Regeneration;
                HealthChanged?.Invoke(_currentHealth);
            }

            _regeneration = null;
        }

        private void SetPlayerDie()
        {
            PlayerDied?.Invoke();
        }

        public void Dispose()
        {
            if (_regeneration != null)
                _coroutineRunner.StopCoroutine(_regeneration);

            if (_timeReduce != null)
                _coroutineRunner.StopCoroutine(_timeReduce);

            RemoveListeners();
            GC.SuppressFinalize(this);
        }
    }
}