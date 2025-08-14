using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.Upgrades;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class PlayerStats : IDisposable
    {
        private readonly Dictionary<int, int> _levels = new();
        private readonly Dictionary<int, int> _upgradeLevels = new();
        private readonly int _maxExperience = 50;
        private readonly int _maxUpgradeExperience = 500;
        private readonly int _minValue = 0;
        private readonly float _moveSpeedAmplifier = 2f;
        private readonly float _defaultAttackRange = 5f;
        private readonly float _defaultSearchRadius = 5f;
        private readonly float _longAttackRange = 10f;
        private readonly float _longSearchRadius = 10f;

        private int _maxPlayerLevel = 100;
        private int _maxUpgradeLevel = 50;

        private DamageSource _damageSource;
        private float _moveSpeed;
        private int _currentLevel;
        private int _currentUpgradeLevel = 0;
        private int _currentUpgradePoints = 0;
        private int _currentExperience = 0;
        private int _currentUpgradeExperience = 0;
        private int _rerollPoints;
        private int _score;
        private int _armor;
        private int _regeneration;
        private int _countKillEnemy;
        private float _chanceCriticalDamage;
        private float _criticalDamageMultiplier;
        private float _chanceVampirism;
        private float _vampirismValue;
        private float _searchRadius;
        private float _attackRange;
        private PlayerClassData _classData;
        private Dictionary<TypeParameter, IUpgradeStats> _playerParametrs;

        public PlayerStats(
            WeaponData weaponData,
            PlayerClassData classData,
            int currentLevel,
            int rerollPoints,
            int armor,
            float moveSpeed,
            int regeneration,
            int countKillEnemy)
        {
            _playerParametrs = new Dictionary<TypeParameter, IUpgradeStats>
            {
                { TypeParameter.MoveSpeed, new MoveSpeedParametr(moveSpeed)},
                { TypeParameter.Armor, new ArmorParametr(armor)},
                { TypeParameter.Regeneration, new RegenerationParametr(regeneration)},
                { TypeParameter.Health, new HealthParametr()},
                { TypeParameter.Damage, new DamageParameterPlayer(_damageSource)},
                { TypeParameter.Reroll, new RerollPointsParametr(rerollPoints)},
                { TypeParameter.AbilityCooldown, new AbilityCooldownReductionParameter()},
                { TypeParameter.AbilityDuration, new AbilityDurationParameter()},
                { TypeParameter.AbilityValue, new AbilityDamageParameter()},
            };

            _currentLevel = currentLevel;
            _rerollPoints = rerollPoints;//
            _rerollPoints = 100;
            _armor = armor;///
            _moveSpeed = moveSpeed;///
            _regeneration = regeneration;///
            _countKillEnemy = countKillEnemy;
            _classData = classData;
            GenerateLevelPlayer(_maxPlayerLevel);
            GenerateUpgradeLevel(_maxUpgradeLevel);
            ApplyWeaponParameters(weaponData, classData.TypeAttackRange);
        }

        public event Action<int> HealthUpgradeApplied;
        public event Action<int> Healed;
        public event Action<int> ExperienceValueChanged;
        public event Action<int> UpgradeExperienceValueChanged;
        public event Action<int> AbilityDurationChanged;
        public event Action<int> AbilityDamageChanged;
        public event Action<int> AbilityCooldownReductionChanged;
        public event Action<int> KillCountChanged;
        public event Action<int, int, int> PlayerLevelChanged;
        public event Action<int, int, int> PlayerUpgradeLevelChanged;
        public event Action<int> CoinsAdding;

        public DamageSource DamageSource => _damageSource;

        public int Armor
        {
            get
            {
                //if (_playerParametrs.TryGetValue(TypeParameter.Armor, out var value))
                //    return Convert.ToInt32((value as ArmorParametr).CurrentArmorValue);

                return _armor;
            }
        }

        public float MoveSpeed
        {
            get
            {
                //if (_playerParametrs.TryGetValue(TypeParameter.MoveSpeed, out var value))
                //    return (value as MoveSpeedParametr).CurrentMoveSpeed;

                return _moveSpeed;
            }
        }

        public float MaxMoveSpeed
        {
            get
            {
                //if (_playerParametrs.TryGetValue(TypeParameter.MoveSpeed, out var value))
                //    return (value as MoveSpeedParametr).MaxMoveSpeed;

                return _moveSpeed;
            }
        }

        public int RerollPoints => _rerollPoints;
        public int Regeneration => _regeneration;
        public int UpgradePoints => _currentUpgradePoints;
        public int Score => _score;
        public int CountKillEnemy => _countKillEnemy;
        public float ChanceCriticalDamage => _chanceCriticalDamage;
        public float CriticalDamageMultiplier => _criticalDamageMultiplier;
        public float ChanceVampirism => _chanceVampirism;
        public float VampirismValue => _vampirismValue;
        public float SearchRadius => _searchRadius;
        public float AttackRange => _attackRange;
        public PlayerClassData PlayerClassData => _classData;

        public void Dispose()
        {
        }

        public void EnemyDied(Enemy enemy)
        {
            _score += enemy.Score;
            _currentExperience += enemy.ExperienceReward;
            _currentUpgradeExperience += enemy.UpgradeExperienceReward;
            _countKillEnemy++;
            UpgradeExperienceValueChanged?.Invoke(enemy.UpgradeExperienceReward);
            ExperienceValueChanged?.Invoke(enemy.ExperienceReward);
            CoinsAdding?.Invoke(enemy.GoldReward);
            KillCountChanged?.Invoke(_countKillEnemy);
            SetNewPlayerLevel(_currentLevel);
            SetNewUpgradePoints(_currentUpgradeLevel);
        }

        public IUpgradeStats GetParameter(TypeParameter type)
        {
            if (_playerParametrs.TryGetValue(type, out var value))
                return value;

            return null;
        }

        public bool TryGetRerollPoints()
        {
            return _rerollPoints == _minValue ? false : true;
        }

        public void TakeLootRoomReward(int reward)
        {
            CoinsAdding?.Invoke(reward);
        }

        public void UpdateCardPanelByRerollPoints()
        {
            var value = _rerollPoints = Mathf.Clamp(_rerollPoints--, _minValue, _rerollPoints);

            if (_playerParametrs.TryGetValue(TypeParameter.Reroll, out IUpgradeStats parametr))
            {
                (parametr as IRevertStats).Revent(value);
            }
        }

        public void GetReward(int value)
        {
            _rerollPoints += value;
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
            foreach (var parameter in
                cardView.CardData.AttributeData.Parameters[cardView.CardState.CurrentLevel].CardParameters)
            {
                if (_playerParametrs.TryGetValue(parameter.TypeParameter, out IUpgradeStats value))
                {
                    value.Apply(parameter.Value);
                }
            }
        }

        public void UpdateRerollPoints(CardView cardView)
        {
            var type = cardView.CardData.AttributeData.Parameters[cardView.CardState.CurrentLevel].CardParameters[0].TypeParameter;
            var value = cardView.CardData.AttributeData.Parameters[cardView.CardState.CurrentLevel].CardParameters[0].Value;

            if (_playerParametrs.TryGetValue(type, out IUpgradeStats parametr))
            {
                parametr.Apply(value);
            }
        }

        public void SetPlayerUpgrades(GameConfig gameConfig, PersistentDataService persistentDataService)
        {
            UpgradeData upgradeData;

            if (persistentDataService.PlayerProgress.UpgradeService.UpgradeStates == null)
                return;

            if (persistentDataService.PlayerProgress.UpgradeService.UpgradeStates.Count < _minValue)
                return;

            foreach (UpgradeState upgradeState in persistentDataService.PlayerProgress.UpgradeService.UpgradeStates)
            {
                if (upgradeState.CurrentLevel > _minValue)
                {
                    upgradeData = gameConfig.GetUpgradeDataById(upgradeState.Id);
                    float value = upgradeData.GetUpgradeParameterByCurrentLevel(upgradeState.CurrentLevel).Value;

                    if (_playerParametrs.TryGetValue(upgradeData.TypeParameter, out IUpgradeStats parametr))
                    {
                        parametr.Apply(value);
                    }
                }
            }
        }

        public void AbilityUsed(Ability ability)
        {
            foreach (CardParameter parameter in ability.AmplifierParameters)
            {
                if (_playerParametrs.TryGetValue(parameter.TypeParameter, out IUpgradeStats value))
                {
                    value.Apply(parameter.Value);
                }
            }
        }

        public void AbilityEnded(Ability ability)
        {
            foreach (CardParameter type in ability.AmplifierParameters)
            {
                if (_playerParametrs.TryGetValue(type.TypeParameter, out IUpgradeStats parameter))
                {
                    (parameter as IRevertStats).Revent(type.Value);
                }
            }
        }

        private void ApplyWeaponParameters(WeaponData weaponData, TypeAttackRange typeAttackRange)
        {
            _damageSource = new DamageSource(
                weaponData.DamageSource.TypeDamage,
                weaponData.DamageSource.DamageParameters,
                weaponData.DamageSource.PoolParticle,
                weaponData.DamageSource.Damage);

            foreach (var parameter in weaponData.WeaponParameters)
            {
                switch (parameter.SupportiveParameter)
                {
                    case TypeWeaponSupportiveParameter.CritChance:
                        _chanceCriticalDamage = parameter.Value;
                        break;
                    case TypeWeaponSupportiveParameter.CritDamage:
                        _criticalDamageMultiplier = parameter.Value;
                        break;
                    case TypeWeaponSupportiveParameter.LifeStealChance:
                        _chanceVampirism = parameter.Value;
                        break;
                    case TypeWeaponSupportiveParameter.LifeStealValue:
                        _vampirismValue = parameter.Value;
                        break;
                    case TypeWeaponSupportiveParameter.BonusArmor:
                        _armor += Convert.ToInt32(parameter.Value);
                        break;
                }
            }

            if (typeAttackRange == TypeAttackRange.Ranged)
                ApplyPlayerRange(_longAttackRange, _longSearchRadius);
            else
                ApplyPlayerRange(_defaultAttackRange, _defaultSearchRadius);
        }

        private void ApplyPlayerRange(float attackRange, float searchRange)
        {
            _attackRange = attackRange;
            _searchRadius = searchRange;
        }

        private void SetNewUpgradePoints(int level)
        {
            if (_upgradeLevels.TryGetValue(level, out int value))
            {
                if (_currentUpgradeExperience >= value)
                {
                    var difference = _currentUpgradeExperience - value;
                    _currentUpgradeLevel++;
                    _currentUpgradePoints++;
                    _currentUpgradeExperience = difference;
                    _upgradeLevels.TryGetValue(_currentUpgradeLevel, out int maxExperienceValue);
                    PlayerUpgradeLevelChanged?.Invoke(_currentUpgradeLevel,
                        maxExperienceValue, _currentUpgradeExperience);
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