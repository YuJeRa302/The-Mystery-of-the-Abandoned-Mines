using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerHealth : IDisposable
    {
        private readonly int _minHealth = 0;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IGameLoopService _gameLoopService;

        private int _maxHealth = 100;
        private int _currentHealth;
        private int _delayHealing = 1;
        private int _regenerationValue = 1;
        private int _armor = 2;
        private float _timeAfterLastAttack = 0;
        private float _maxTimeAfterAttack = 1;
        private Coroutine _regeneration;
        private Coroutine _timeReduce;

        public event Action DamageTaked;
        public event Action<int> HealthChanged;
        public event Action PlayerDied;

        public PlayerHealth(int currentHealth, WeaponData weapon, ICoroutineRunner coroutineRunner, IGameLoopService gameLoopService)
        {
            _coroutineRunner = coroutineRunner;
            _gameLoopService = gameLoopService;
            _currentHealth = currentHealth;
            ApplyWeaponParameters(weapon);
            AddListeners();
            _regeneration = _coroutineRunner.StartCoroutine(RegenerationHealth());
        }

        public void ReduceHealth(float reduce)
        {
            _currentHealth -= Convert.ToInt32(_currentHealth * (reduce/100));
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
                    var currentDamage = (damage - _armor);

                    if (currentDamage <= _minHealth)
                        currentDamage = 1;

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

        public void ChangeRegeniration(int regeniration)
        {
            _regenerationValue = regeniration;
        }

        public void ChangeArmor(int armor)
        {
            _armor = armor;
            Debug.Log(armor);
        }

        public int GetMaxHealth() 
        {
            return _maxHealth;
        }

        public int GetCurrentHealth() 
        {
            return _currentHealth;
        }

        private void ApplyWeaponParameters(WeaponData weapon) 
        {
            foreach (var parametr in weapon.WeaponParameter.WeaponSupportivePatametrs)
            {
                if (parametr.SupportivePatametr == TypeWeaponSupportiveParameter.BonusArmor)
                    _armor += Convert.ToInt32(parametr.Value);
            }
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
                _currentHealth += _regenerationValue;
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