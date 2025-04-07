using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerStats : IDisposable
    {
        private readonly Dictionary<int, int> _levels = new();
        private readonly Dictionary<int, int> _upgradeLevels = new();
        private readonly int _maxExperience = 50;
        private readonly int _maxUpgradeExperience = 1000;
        private readonly int _minValue = 0;

        private int _maxPlayerLevel = 100;//
        private int _maxUpgradeLevel = 5;//

        private Player _player;
        private float _speed = 2;
        private float _ownSpeed;
        private int _currentLevel;
        private int _currentUpgradeLevel = 0;
        private int _currentUpgradePoints = 0;
        private int _currentExperience = 0;
        private int _currentUpgradeExperience = 0;

        private UpgradeState[] _upgradeState;
        private UpgradeData[] _upgradeDatas;
        private int _rerollPoints;
        private int _score;
        private int _damage;
        private int _armor;
        private int _regeneration;
        private int _countKillEnemy;

        public event Action<int> MaxHealthChanged;
        public event Action<int> RegenerationChanged;
        public event Action<int> ArmorChanged;
        public event Action<int> DamageChenged;
        public event Action<float> MoveSpeedChanged;
        public event Action<float> HealthReduced;
        public event Action<int> Healed;
        public event Action<int> ExperienceValueChanged;
        public event Action<int> UpgradeExperienceValueChanged;
        public event Action<int> AbilityDurationChanged;
        public event Action<int> AbilityDamageChanged;
        public event Action<int> AbilityCooldownReductionChanged;
        public event Action<int> KillCountChanged;
        public event Action LvlUpped;
        public event Action<int, int, int> PlayerLevelChanged;
        public event Action<int, int, int> PlayerUpgradeLevelChanged;
        public event Action<int> AddingConins;

        public PlayerStats(Player player, int currentLevel, int rerollPoints, int damage, int armor, int regeneration, int countKillEnemy, UpgradeState[] upgradeState, TemporaryData temporaryData)
        {
            _currentLevel = currentLevel;
            _rerollPoints = rerollPoints;
            _damage = damage;
            _armor = armor;
            _regeneration = regeneration;
            _countKillEnemy = countKillEnemy;
            _upgradeState = new UpgradeState[upgradeState.Length];
            Array.Copy(upgradeState, _upgradeState, upgradeState.Length);
            _upgradeDatas = temporaryData.UpgradeDatas;
            _player = player;
            _damage = Convert.ToInt32(_player.PlayerAttacker.Damage);
            GenerateLevelPlayer(_maxPlayerLevel);
            GenerateUpgradeLevel(_maxUpgradeLevel);
        }

        public int RerollPoints => _rerollPoints;
        public float Speed => _speed;
        public int Armor => _armor;
        public int UpgradePoints => _currentUpgradePoints;
        public int Damage => _damage;
        public int Score => _score;
        public int CountKillEnemy => _countKillEnemy;
        public int RerollPoint => _rerollPoints;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void EnemyDied(Enemy enemy)
        {
            _score += enemy.Score;
            _currentExperience += enemy.ExperienceReward;
            _currentUpgradeExperience += enemy.UpgradeExperienceReward;
            _countKillEnemy++;
            UpgradeExperienceValueChanged?.Invoke(enemy.UpgradeExperienceReward);
            ExperienceValueChanged?.Invoke(enemy.ExperienceReward);
            AddingConins?.Invoke(enemy.GoldReward);
            KillCountChanged?.Invoke(_countKillEnemy);
            
            SetNewPlayerLevel(_currentLevel);
            SetNewUpgradePoints(_currentUpgradeLevel);
        }

        public bool TryGetRerollPoints()
        {
            return _rerollPoints == _minValue ? false : true;
        }

        public void UpdateCardPanelByRerollPoints()
        {
            _rerollPoints = Mathf.Clamp(_rerollPoints--, _minValue, _rerollPoints);
        }

        public int GetMaxExperienceValue(int currentLevel) 
        {
            _levels.TryGetValue(currentLevel, out int maxLevelExperience);
            return maxLevelExperience;
        }

        public int GetMaxUpgradeExperienceValue(int currentLevel) 
        {
            _upgradeLevels.TryGetValue(currentLevel, out int maxUpgradeExperience);
            return maxUpgradeExperience;
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
                        DamageChenged?.Invoke(_damage);
                        break;
                    case TypeParameter.Regeneration:
                        _regeneration += parameter.Value;
                        RegenerationChanged?.Invoke(_regeneration);
                        break;
                    case TypeParameter.Health:
                        MaxHealthChanged?.Invoke(parameter.Value);
                        break;
                    case TypeParameter.MoveSpeed:
                        _speed += parameter.Value/2;
                        MoveSpeedChanged?.Invoke(_speed);
                        break;
                }
            }
        }

        public void UpdateRerollPoints(CardView cardView)
        {
            _rerollPoints += cardView.CardData.AttributeData.CardParameters[cardView.CardState.CurrentLevel].CardParameters[0].Value;
        }

        public void UpgradePlayerStats()
        {
            if (_upgradeState == null)
                return;

            if (_upgradeDatas.Length < _minValue)
                return;

            foreach (UpgradeState upgradeState in _upgradeState)
            {
                foreach (UpgradeData upgradeData in _upgradeDatas)
                {
                    if (upgradeState.CurrentLevel > _minValue)
                    {
                        if (upgradeState.Id == upgradeData.Id)
                        {
                            switch (upgradeData.TypeParameter)
                            {
                                case TypeParameter.Armor:
                                    Debug.Log(upgradeState.CurrentLevel);
                                    _armor += upgradeData.UpgradeParameters[upgradeState.CurrentLevel-1].Value;
                                    ArmorChanged?.Invoke(_armor);
                                    break;
                                case TypeParameter.Damage:
                                    _damage += upgradeData.UpgradeParameters[upgradeState.CurrentLevel-1].Value;
                                    DamageChenged?.Invoke(_damage);
                                    break;
                                case TypeParameter.Regeneration:
                                    _regeneration += upgradeData.UpgradeParameters[upgradeState.CurrentLevel-1].Value;
                                    break;
                                case TypeParameter.Reroll:
                                    _rerollPoints += upgradeData.UpgradeParameters[upgradeState.CurrentLevel-1].Value;
                                    break;
                                case TypeParameter.Health:
                                    MaxHealthChanged?.Invoke(upgradeData.UpgradeParameters[upgradeState.CurrentLevel-1].Value);
                                    break;
                                case TypeParameter.AbilityCooldown:
                                    AbilityCooldownReductionChanged?.Invoke(upgradeData.UpgradeParameters[upgradeState.CurrentLevel-1].Value);
                                    break;
                                case TypeParameter.AbilityDuration:
                                    AbilityDurationChanged?.Invoke(upgradeData.UpgradeParameters[upgradeState.CurrentLevel-1].Value);
                                    break;
                                case TypeParameter.AbilityValue:
                                    AbilityDamageChanged?.Invoke(upgradeData.UpgradeParameters[upgradeState.CurrentLevel-1].Value);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void AbilityUsed(Ability ability)
        {
            if (ability.IsAutoCast)
            {
                if (ability.TypeAbility == TypeAbility.DamageAmplifier)
                {
                    _damage += ability.CurrentAbilityValue;
                    DamageChenged?.Invoke(_damage);
                }
                else if (ability.TypeAbility == TypeAbility.ArmorAmplifier)
                {
                    _armor += ability.CurrentAbilityValue;
                    ArmorChanged?.Invoke(_armor);
                }
                else if (ability.TypeAbility == TypeAbility.RegenerationAmplifier)
                {
                    _regeneration += ability.CurrentAbilityValue;
                }
                else if (ability.TypeAbility == TypeAbility.MoveSpeedAmplifier)
                {
                    _speed += ability.CurrentAbilityValue/2;
                    MoveSpeedChanged?.Invoke(_speed);
                }
                else if (ability.TypeAbility == TypeAbility.Healing)
                {
                    _regeneration += ability.CurrentAbilityValue;
                }
            }
            else
            {
                foreach (CardParameter parameter in ability.AmplifierParametrs)
                {
                    if (parameter.TypeParameter == TypeParameter.Damage)
                    {
                        _damage += parameter.Value;
                        DamageChenged?.Invoke(_damage);
                    }
                    else if (parameter.TypeParameter == TypeParameter.Armor)
                    {
                        _armor += parameter.Value;
                        ArmorChanged?.Invoke(_armor);
                    }
                    else if (parameter.TypeParameter == TypeParameter.MoveSpeed)
                    {
                        _speed += parameter.Value/2;
                        MoveSpeedChanged?.Invoke(_speed);
                    }
                    else if (parameter.TypeParameter == TypeParameter.HealtReduce)
                    {
                        HealthReduced?.Invoke(parameter.Value);
                    }
                    else if (parameter.TypeParameter == TypeParameter.Healing)
                    {
                        Healed?.Invoke(parameter.Value);
                    }
                    else if (parameter.TypeParameter == TypeParameter.Regeneration)
                    {
                        _regeneration += parameter.Value;
                        RegenerationChanged?.Invoke(_regeneration);
                    }
                    else if (parameter.TypeParameter == TypeParameter.TargetMoveSpeed)
                    {
                        _ownSpeed = _speed;
                        _speed = parameter.Value/2;
                        MoveSpeedChanged?.Invoke(_speed);
                    }
                }
            }
        }

        public void AbilityEnded(Ability ability)
        {
            if (ability.IsAutoCast)
            {
                if (ability.TypeAbility == TypeAbility.DamageAmplifier)
                {
                    _damage -= ability.CurrentAbilityValue;
                    DamageChenged?.Invoke(_damage);
                }
                else if (ability.TypeAbility == TypeAbility.ArmorAmplifier)
                {
                    _armor -= ability.CurrentAbilityValue;
                    ArmorChanged?.Invoke(_armor);
                }
                else if (ability.TypeAbility == TypeAbility.RegenerationAmplifier)
                {
                    _regeneration -= ability.CurrentAbilityValue;
                }
                else if (ability.TypeAbility == TypeAbility.MoveSpeedAmplifier)
                {
                    _speed -= ability.CurrentAbilityValue/2;
                    MoveSpeedChanged?.Invoke(_speed);
                }
                else if (ability.TypeAbility == TypeAbility.Healing)
                {
                    _regeneration -= ability.CurrentAbilityValue;
                }
            }
            else
            {
                foreach (CardParameter parameter in ability.AmplifierParametrs)
                {
                    if (parameter.TypeParameter == TypeParameter.Damage)
                    {
                        _damage -= parameter.Value;
                        DamageChenged?.Invoke(_damage);
                    }
                    else if (parameter.TypeParameter == TypeParameter.Armor)
                    {
                        _armor -= parameter.Value;
                        ArmorChanged?.Invoke(_armor);
                    }
                    else if (parameter.TypeParameter == TypeParameter.MoveSpeed)
                    {
                        _speed -= parameter.Value/2;
                        MoveSpeedChanged?.Invoke(_speed);
                    }
                    else if (parameter.TypeParameter == TypeParameter.Regeneration)
                    {
                        _regeneration -= parameter.Value;
                        RegenerationChanged?.Invoke(_regeneration);
                    }
                    else if (parameter.TypeParameter == TypeParameter.TargetMoveSpeed)
                    {
                        _speed = _ownSpeed;
                        MoveSpeedChanged?.Invoke(_speed);
                    }
                }
            }
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
                    _upgradeLevels.TryGetValue(_currentUpgradeLevel, out int maxExperienceValue);
                    PlayerUpgradeLevelChanged?.Invoke(_currentUpgradeLevel, maxExperienceValue, _currentUpgradeExperience);
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

                    LvlUpped?.Invoke();//rework
                    _levels.TryGetValue(_currentLevel, out int maxExperienceValue);
                    PlayerLevelChanged?.Invoke(_currentLevel, maxExperienceValue, _currentExperience);
                }
            }
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