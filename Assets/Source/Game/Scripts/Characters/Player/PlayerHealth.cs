using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.Upgrades;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
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
        private bool _canHealing = true;
        private CompositeDisposable _disposables = new();

        public PlayerHealth(
            Player player,
            ICoroutineRunner coroutineRunner,
            GamePauseService gamePauseService,
            int currentHealth)
        {
            _player = player;
            _coroutineRunner = coroutineRunner;
            _gamePauseService = gamePauseService;
            _currentHealth = currentHealth;
            _maxHealth = currentHealth;
            AddListeners();
            _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());
        }

        public event Action PlayerDied;
        public ReactiveProperty<int> MaxHealthChanged { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> CurrentHealthChanged { get; } = new ReactiveProperty<int>();

        public void TakeDamage(int damage)
        {
            if (_currentHealth <= _minHealth)
                return;

            if (_currentHealth > _minHealth)
            {
                int currentDamage = damage - _player.PlayerStats.Armor;

                if (currentDamage <= 1)
                    currentDamage = _minDamage;

                _currentHealth = Mathf.Clamp(_currentHealth - currentDamage, _minHealth, _maxHealth);
                CurrentHealthChanged.Value = _currentHealth;

                if (_currentHealth <= _minHealth)
                    SetPlayerDie();
            }
        }

        public void DisableHealing()
        {
            _canHealing = false;

            if (_regeneration != null && _coroutineRunner != null)
                _coroutineRunner.StopCoroutine(_regeneration);

            if (_timeReduce != null && _coroutineRunner != null)
                _coroutineRunner.StopCoroutine(_timeReduce);
        }

        public int GetMaxHealth()
        {
            return _maxHealth;
        }

        public int GetCurrentHealth()
        {
            return _currentHealth;
        }

        public void Dispose()
        {
            if (_regeneration != null)
                _coroutineRunner.StopCoroutine(_regeneration);

            if (_timeReduce != null)
                _coroutineRunner.StopCoroutine(_timeReduce);

            if (_disposables != null)
                _disposables.Dispose();

            RemoveListeners();
        }

        private void AddListeners()
        {
            _gamePauseService.GamePaused += OnPauseGame;
            _gamePauseService.GameResumed += OnResumeGame;

            MessageBroker.Default
                .Receive<M_MaxHealthChanged>()
                .Subscribe(m => ChangeMaxHealth(Convert.ToInt32(m.Value)))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_HealthReduced>()
                .Subscribe(m => ReduceHealth(Convert.ToInt32(m.Reduction)))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_Healing>()
                .Subscribe(m => TakeHealing(Convert.ToInt32(m.Value)))
                .AddTo(_disposables);

            CurrentHealthChanged.Value = _currentHealth;
        }

        private void TakeHealing(int heal)
        {
            if (heal < _minHealth)
                return;

            _currentHealth += heal;

            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;

            CurrentHealthChanged.Value = _currentHealth;
        }

        private void ReduceHealth(float reduce)
        {
            _currentHealth -= Convert.ToInt32(_currentHealth * (reduce / 100));
            CurrentHealthChanged.Value = _currentHealth;
        }

        private void ChangeMaxHealth(int value)
        {
            _maxHealth += value;
            MaxHealthChanged.Value = _maxHealth;
        }


        private void RemoveListeners()
        {
            _gamePauseService.GamePaused -= OnPauseGame;
            _gamePauseService.GameResumed -= OnResumeGame;
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

            if (_canHealing)
                _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());
        }

        private IEnumerator RegenerationHealth()
        {
            while (_currentHealth < _maxHealth)
            {
                yield return new WaitForSeconds(_delayHealing);
                _currentHealth += _player.PlayerStats.Regeneration;
                CurrentHealthChanged.Value = _currentHealth;
            }

            _regeneration = null;
        }

        private void SetPlayerDie()
        {
            PlayerDied?.Invoke();
        }
    }
}