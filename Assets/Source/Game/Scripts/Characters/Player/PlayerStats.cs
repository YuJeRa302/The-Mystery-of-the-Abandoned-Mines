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
        private float _maxMoveSpeed;
        private float _ownSpeed;
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
            _currentLevel = currentLevel;
            _rerollPoints = rerollPoints;
            _rerollPoints = 100;
            _armor = armor;
            _moveSpeed = moveSpeed;
            _maxMoveSpeed = _moveSpeed * _moveSpeedAmplifier;
            _regeneration = regeneration;
            _countKillEnemy = countKillEnemy;
            _classData = classData;
            GenerateLevelPlayer(_maxPlayerLevel);
            GenerateUpgradeLevel(_maxUpgradeLevel);
            ApplyWeaponParameters(weaponData, classData.TypeAttackRange);
        }

        public event Action<int> MaxHealthChanged;
        public event Action<int> HealthUpgradeApplied;
        public event Action<float> HealthReduced;
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
        public int Armor => _armor;
        public float MoveSpeed => _moveSpeed;
        public float MaxMoveSpeed => _maxMoveSpeed;
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
            CoinsAdding?.Invoke(enemy.GoldReward);
            KillCountChanged?.Invoke(_countKillEnemy);
            SetNewPlayerLevel(_currentLevel);
            SetNewUpgradePoints(_currentUpgradeLevel);
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
            _rerollPoints = Mathf.Clamp(_rerollPoints--, _minValue, _rerollPoints);
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
                switch (parameter.TypeParameter)
                {
                    case TypeParameter.Armor:
                        _armor += parameter.Value;
                        break;
                    case TypeParameter.Damage:
                        _damageSource.ChangeDamage(_damageSource.Damage + parameter.Value);
                        break;
                    case TypeParameter.Regeneration:
                        _regeneration += parameter.Value;
                        break;
                    case TypeParameter.Health:
                        MaxHealthChanged?.Invoke(parameter.Value);
                        break;
                    case TypeParameter.MoveSpeed:
                        IncreaseMoveSpeed(parameter.Value);
                        break;
                }
            }
        }

        public void UpdateRerollPoints(CardView cardView)
        {
            _rerollPoints +=
                cardView.CardData.AttributeData.Parameters[cardView.CardState.CurrentLevel].CardParameters[0].Value;
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

                    switch (upgradeData.TypeParameter)
                    {
                        case TypeParameter.Armor:
                            _armor += upgradeData.GetUpgradeParameterByCurrentLevel(upgradeState.CurrentLevel).Value;
                            break;
                        case TypeParameter.Damage:
                            _damageSource.ChangeDamage(
                                _damageSource.Damage +
                                upgradeData.GetUpgradeParameterByCurrentLevel(upgradeState.CurrentLevel).Value);
                            break;
                        case TypeParameter.Regeneration:
                            _regeneration += upgradeData.GetUpgradeParameterByCurrentLevel(
                                upgradeState.CurrentLevel).Value;
                            break;
                        case TypeParameter.Reroll:
                            _rerollPoints += upgradeData.GetUpgradeParameterByCurrentLevel(
                                upgradeState.CurrentLevel).Value;
                            break;
                        case TypeParameter.Health:
                            HealthUpgradeApplied?.Invoke(upgradeData.GetUpgradeParameterByCurrentLevel(
                                upgradeState.CurrentLevel).Value);
                            break;
                        case TypeParameter.AbilityCooldown:
                            AbilityCooldownReductionChanged?.Invoke(upgradeData.GetUpgradeParameterByCurrentLevel(
                                upgradeState.CurrentLevel).Value);
                            break;
                        case TypeParameter.AbilityDuration:
                            AbilityDurationChanged?.Invoke(upgradeData.GetUpgradeParameterByCurrentLevel(
                                upgradeState.CurrentLevel).Value);
                            break;
                        case TypeParameter.AbilityValue:
                            AbilityDamageChanged?.Invoke(upgradeData.GetUpgradeParameterByCurrentLevel(
                                upgradeState.CurrentLevel).Value);
                            break;
                    }
                }
            }
        }

        public void AbilityUsed(Ability ability)
        {
            if (ability.IsAutoCast)
            {
                switch (ability.TypeAbility)
                {
                    case TypeAbility.DamageAmplifier:
                        _damageSource.ChangeDamage(_damageSource.Damage + ability.CurrentAbilityValue);
                        break;
                    case TypeAbility.ArmorAmplifier:
                        _armor += ability.CurrentAbilityValue;
                        break;
                    case TypeAbility.RegenerationAmplifier:
                        _regeneration += ability.CurrentAbilityValue;
                        break;
                    case TypeAbility.MoveSpeedAmplifier:
                        IncreaseMoveSpeed(ability.CurrentAbilityValue);
                        break;
                    case TypeAbility.Healing:
                        _regeneration += ability.CurrentAbilityValue;
                        break;
                }
            }
            else
            {
                foreach (CardParameter parameter in ability.AmplifierParameters)
                {
                    switch (parameter.TypeParameter)
                    {
                        case TypeParameter.Damage:
                            _damageSource.ChangeDamage(_damageSource.Damage + parameter.Value);
                            break;
                        case TypeParameter.Armor:
                            _armor += parameter.Value;
                            break;
                        case TypeParameter.MoveSpeed:
                            IncreaseMoveSpeed(parameter.Value);
                            break;
                        case TypeParameter.HealthReduce:
                            HealthReduced?.Invoke(parameter.Value);
                            break;
                        case TypeParameter.Healing:
                            Healed?.Invoke(parameter.Value);
                            break;
                        case TypeParameter.Regeneration:
                            _regeneration += parameter.Value;
                            break;
                        case TypeParameter.TargetMoveSpeed:
                            ChangeMoveSpeed(parameter.Value);
                            break;
                    }
                }
            }
        }

        public void AbilityEnded(Ability ability)
        {
            if (ability.IsAutoCast)
            {
                switch (ability.TypeAbility)
                {
                    case TypeAbility.DamageAmplifier:
                        _damageSource.ChangeDamage(_damageSource.Damage - ability.CurrentAbilityValue);
                        break;
                    case TypeAbility.ArmorAmplifier:
                        _armor -= ability.CurrentAbilityValue;
                        break;
                    case TypeAbility.RegenerationAmplifier:
                        _regeneration -= ability.CurrentAbilityValue;
                        break;
                    case TypeAbility.MoveSpeedAmplifier:
                        DecreaseMoveSpeed(ability.CurrentAbilityValue);
                        break;
                    case TypeAbility.Healing:
                        _regeneration -= ability.CurrentAbilityValue;
                        break;
                }
            }
            else
            {
                foreach (CardParameter parameter in ability.AmplifierParameters)
                {
                    switch (parameter.TypeParameter)
                    {
                        case TypeParameter.Damage:
                            _damageSource.ChangeDamage(_damageSource.Damage - parameter.Value);
                            break;
                        case TypeParameter.Armor:
                            _armor -= parameter.Value;
                            break;
                        case TypeParameter.MoveSpeed:
                            DecreaseMoveSpeed(parameter.Value);
                            break;
                        case TypeParameter.Regeneration:
                            _regeneration -= parameter.Value;
                            break;
                        case TypeParameter.TargetMoveSpeed:
                            ChangeMoveSpeed(_ownSpeed);
                            break;
                    }
                }
            }
        }

        private void ChangeMoveSpeed(float value)
        {
            _ownSpeed = _moveSpeed;
            _moveSpeed = value;
            _maxMoveSpeed = _moveSpeed * _moveSpeedAmplifier;
        }

        private void IncreaseMoveSpeed(float value)
        {
            _moveSpeed += value;
            _maxMoveSpeed = _moveSpeed * _moveSpeedAmplifier;
        }

        private void DecreaseMoveSpeed(float value)
        {
            _moveSpeed -= value;
            _maxMoveSpeed = _moveSpeed * _moveSpeedAmplifier;
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