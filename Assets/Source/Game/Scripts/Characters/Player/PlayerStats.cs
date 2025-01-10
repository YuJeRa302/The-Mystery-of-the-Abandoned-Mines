using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerStats : IDisposable
    {
        private readonly Dictionary<int, int> _levels = new();
        private readonly Dictionary<int, int> _upgradeLevels = new();
        private readonly int _maxExperience = 100;
        private readonly int _maxUpgradeExperience = 1000;
        private readonly int _minValue = 0;

        //[SerializeField] private float _speed;
        private int _maxPlayerLevel;//
        private int _maxUpgradeLevel;//

        private Player _player;
        private PlayerView _playerView;
        private int _currentLevel = 1;
        private int _currentUpgradeLevel = 0;
        private int _currentUpgradePoints = 0;
        private int _currentExperience = 0;
        private int _currentUpgradeExperience = 0;
        private int _rerollPoints = 2;
        private int _score = 0;
        private int _damage = 10;//
        private int _armor = 2;
        private int _regeneration = 1;
        private int _countKillEnemy = 0;
        private int _maxLevelValue;
        private int _maxUpgradeValue;

        public event Action<int> MaxHealthChanged;//+
        public event Action<int> RegenerationChanged;//+
        public event Action<int> ArmorChanged;//+
        public event Action<int> ExperienceValueChanged;//view
        public event Action<int> UpgradeExperienceValueChanged;//view
        public event Action<int> AbilityDurationChanged;//+
        public event Action<int> AbilityDamageChanged;//+
        public event Action<int> AbilityCooldownReductionChanged;//+
        public event Action<int> KillCountChanged;//view

        public PlayerStats(Player player, int score, UpgradeState[] upgradeState, LevelObserver levelObserver, 
            AbilityFactory abilityFactory, AbilityPresenterFactory abilityPresenterFactory)
        {
            //UpgradePlayerStats(upgradeState, levelObserver.UpgradeDatas);
            _player = player;
            _playerView = levelObserver.PlayerView;

            GenerateLevelPlayer(_maxPlayerLevel);
            GenerateUpgradeLevel(_maxUpgradeLevel);
            SetPlayerStats(score);
        }

        //public float Speed => _speed;
        public int Armor => _armor;
        public int UpgradePoints => _currentUpgradePoints;
        public int Damage => _damage;
        public int Score => _score;
        public int CountKillEnemy => _countKillEnemy;
        public int Regeneration => _regeneration;
        public int UpgradeExperience => _currentUpgradeExperience;
        public int CurrentExperience => _currentExperience;
        public int MaxLevelValue => _maxLevelValue;
        public int MaxUpgradeValue => _maxUpgradeValue;
        public int CurrentLevel => _currentLevel;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void EnemyDied(Enemy enemy)
        {
            //_score += enemy.Score;
            //_currentExperience += enemy.ExperienceReward;
            //_currentUpgradeExperience += enemy.UpgradeExperienceReward;
            //UpgradeExperienceValueChanged?.Invoke(enemy.UpgradeExperienceReward);
            //ExperienceValueChanged?.Invoke(enemy.ExperienceReward);
            _countKillEnemy++;
            KillCountChanged?.Invoke(_countKillEnemy);
            SetNewPlayerLevel(_currentLevel);
            SetNewUpgradePoints(_currentUpgradeLevel);
        }

        public bool TryGetRerollPoints()
        {
            _rerollPoints = Mathf.Clamp(_rerollPoints--, _minValue, _rerollPoints);
            return _rerollPoints == _minValue ? false : true;
        }

        public void UpdatePlayerStats(CardView cardView)
        {
            foreach (var parameter in cardView.CardData.AttributeData.CardParameters[cardView.CardState.CurrentLevel].CardParameters)
            {
                switch (parameter.TypeParameter)
                {
                    case TypeParameter.Armor:
                        _armor += parameter.Value;
                        ArmorChanged?.Invoke(_armor);
                        break;
                    case TypeParameter.Damage:
                        _damage += parameter.Value;
                        break;
                    case TypeParameter.Regeneration:
                        _regeneration += parameter.Value;
                        RegenerationChanged?.Invoke(_regeneration);
                        break;
                    case TypeParameter.Health:
                        MaxHealthChanged?.Invoke(parameter.Value);
                        break;
                }
            }

            cardView.CardState.CurrentLevel++;
            cardView.CardState.Weight++;
        }

        public void UpdateRerollPoints(CardView cardView)
        {
            _rerollPoints = cardView.CardData.AttributeData.CardParameters[cardView.CardState.CurrentLevel].CardParameters[0].Value;
            cardView.CardState.CurrentLevel++;
            cardView.CardState.Weight++;
        }

        public void SetNewAbility(CardView cardView)
        {
            cardView.CardState.CurrentLevel++;
            cardView.CardState.Weight++;
        }

        private void UpgradePlayerStats(UpgradeState[] upgradeStates, UpgradeData[] upgradeDatas)
        {
            if (upgradeStates == null)
                return;

            if (upgradeDatas.Length < _minValue)
                return;

            foreach (UpgradeState upgradeState in upgradeStates)
            {
                foreach (UpgradeData upgradeData in upgradeDatas)
                {
                    if (upgradeState.CurrentLevel > _minValue)
                    {
                        if (upgradeState.Id == upgradeData.Id)
                        {
                            switch (upgradeData.TypeParameter)
                            {
                                case TypeParameter.Armor:
                                    _armor += upgradeData.UpgradeParameters[upgradeState.CurrentLevel].Value;
                                    break;
                                case TypeParameter.Damage:
                                    _damage += upgradeData.UpgradeParameters[upgradeState.CurrentLevel].Value;
                                    break;
                                case TypeParameter.Regeneration:
                                    _regeneration += upgradeData.UpgradeParameters[upgradeState.CurrentLevel].Value;
                                    break;
                                case TypeParameter.Reroll:
                                    _rerollPoints += upgradeData.UpgradeParameters[upgradeState.CurrentLevel].Value;
                                    break;
                                case TypeParameter.Health:
                                    MaxHealthChanged?.Invoke(upgradeData.UpgradeParameters[upgradeState.CurrentLevel].Value);
                                    break;
                                case TypeParameter.AbilityCooldown:
                                    AbilityCooldownReductionChanged?.Invoke(upgradeData.UpgradeParameters[upgradeState.CurrentLevel].Value);
                                    break;
                                case TypeParameter.AbilityDuration:
                                    AbilityDurationChanged?.Invoke(upgradeData.UpgradeParameters[upgradeState.CurrentLevel].Value);
                                    break;
                                case TypeParameter.AbilityValue:
                                    AbilityDamageChanged?.Invoke(upgradeData.UpgradeParameters[upgradeState.CurrentLevel].Value);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void AbilityUsed(Ability ability)
        {
            if (ability.TypeAbility == TypeAbility.PlayerDamageAmplifier)
                _damage += ability.CurrentAbilityValue;
            else if (ability.TypeAbility == TypeAbility.PlayerArmorAmplifier)
                _armor += ability.CurrentAbilityValue;
            else if (ability.TypeAbility == TypeAbility.PlayerRegenerationAmplifier)
                _regeneration += ability.CurrentAbilityValue;

            //отдельный метод для классовых умений!!
        }

        public void AbilityEnded(Ability ability)
        {
            if (ability.TypeAbility == TypeAbility.PlayerDamageAmplifier)
                _damage -= ability.CurrentAbilityValue;
            else if (ability.TypeAbility == TypeAbility.PlayerArmorAmplifier)
                _armor -= ability.CurrentAbilityValue;
            else if (ability.TypeAbility == TypeAbility.PlayerRegenerationAmplifier)
                _regeneration -= ability.CurrentAbilityValue;
        }

        private void SetNewUpgradePoints(int level)
        {
            if (_upgradeLevels.TryGetValue(level, out int value))
            {
                if (_currentUpgradeExperience >= value)
                {
                    var difference = _currentUpgradeExperience - value;
                    _currentUpgradeLevel++;
                    _currentUpgradeExperience = difference;
                    _playerView.SetNewUpgradeLevelValue(_currentUpgradeLevel);
                    _upgradeLevels.TryGetValue(_currentUpgradeLevel, out int currentValue);
                    _playerView.SetUpgradeExperienceSliderValue(currentValue, _currentUpgradeExperience);
                }
            }
        }

        private void SetNewPlayerLevel(int level)
        {
            if (_levels.TryGetValue(level, out int value))
            {
                if (_currentExperience >= value)
                {
                    var difference = _currentExperience - value;
                    _currentLevel++;
                    _currentExperience = difference;
                    _playerView.SetNewLevelValue(_currentLevel);
                    _levels.TryGetValue(_currentLevel, out int currentValue);
                    _playerView.SetExperienceSliderValue(currentValue, _currentExperience);
                }
            }
        }

        private void SetPlayerStats(int score)
        {
            _score = score;
            _levels.TryGetValue(_currentLevel, out int levelValue);
            _maxLevelValue = levelValue;
            _upgradeLevels.TryGetValue(_currentUpgradeLevel, out int upgradeValue);
            _maxUpgradeValue = upgradeValue;
        }

        private void GenerateLevelPlayer(int level)
        {
            if (_levels.Count == _minValue)
            {
                for (int index = 0; index < level; index++)
                {
                    _levels.Add(index, _maxExperience + _maxExperience * index);
                }
            }
        }

        private void GenerateUpgradeLevel(int level)
        {
            if (_upgradeLevels.Count == _minValue)
            {
                for (int index = 0; index < level; index++)
                {
                    _upgradeLevels.Add(index, _maxUpgradeExperience + _maxUpgradeExperience * index);
                }
            }
        }
    }
}