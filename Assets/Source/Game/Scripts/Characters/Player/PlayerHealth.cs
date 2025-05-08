using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerHealth : IDisposable
    {
        private readonly Player _player;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IGameLoopService _gameLoopService;
        private readonly int _minHealth = 0;

        private int _maxHealth = 100;
        private int _currentHealth;
        private int _delayHealing = 1;
        private Coroutine _regeneration;
        private Coroutine _timeReduce;
        private float _timeAfterLastAttack = 0;
        private float _maxTimeAfterAttack = 0f;

        public event Action DamageTaked;
        public event Action<int> HealthChanged;
        public event Action PlayerDied;

        public PlayerHealth(Player player, ICoroutineRunner coroutineRunner, IGameLoopService gameLoopService, int currentHealth)
        {
            _player = player;
            _coroutineRunner = coroutineRunner;
            _gameLoopService = gameLoopService;
            _currentHealth = _maxHealth;
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
            if (_currentHealth == _minHealth)
                SetPlayerDie();

            if (_currentHealth > _minHealth)
            {
                if(_timeAfterLastAttack <= _minHealth)
                {
                    DamageTaked?.Invoke();
                    var currentDamage = (damage - _player.Armor);
                    _currentHealth = Mathf.Clamp(_currentHealth - currentDamage, _minHealth, _maxHealth);
                    HealthChanged?.Invoke(_currentHealth);
                    _timeAfterLastAttack = _maxTimeAfterAttack;

                    if (_timeReduce == null)
                        _timeReduce = _coroutineRunner.StartCoroutine(ReduceTimeAfterLastAttack());
                }
                else
                {
                    if (_timeReduce == null)
                        _timeReduce = _coroutineRunner.StartCoroutine(ReduceTimeAfterLastAttack());
                }
            }
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
            _gameLoopService.GamePaused += OnPauseGame;
            _gameLoopService.GameResumed += OnResumeGame;
            HealthChanged += OnHealthChanged;
        }

        private void RemoveListeners()
        {
            _gameLoopService.GamePaused -= OnPauseGame;
            _gameLoopService.GameResumed -= OnResumeGame;
            HealthChanged -= OnHealthChanged;
        }

        private void OnPauseGame()
        {
            if (_regeneration != null)
                _coroutineRunner.StopCoroutine(_regeneration);

            if (_timeReduce != null)
                _coroutineRunner.StopCoroutine(_timeReduce);
        }

        private void OnResumeGame()
        {
            if (_regeneration != null)
                _coroutineRunner.StopCoroutine(_regeneration);

            _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());

            if (_timeReduce != null)
                _coroutineRunner.StopCoroutine(_timeReduce);

            _timeReduce = _coroutineRunner.StartCoroutine(ReduceTimeAfterLastAttack());
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

        private IEnumerator ReduceTimeAfterLastAttack()
        {
            while (_timeAfterLastAttack > _minHealth)
            {
                _timeAfterLastAttack -= Time.deltaTime;
                yield return null;
            }

            _timeReduce = null;
        }

        private void SetPlayerDie()
        {
            //Destroy(gameObject);
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