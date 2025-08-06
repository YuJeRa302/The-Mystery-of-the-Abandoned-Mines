using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class Ability : IDisposable
    {
        private readonly int _minValue = 0;
        private readonly ICoroutineRunner _coroutineRunner;

        private float _currentDuration;
        private float _defaultDuration;
        private int _currentAbilityValue;
        private float _chance;
        private int _abilityDamage;
        private float _defaultCooldown;
        private float _currentCooldown;
        private float _spellRadius;
        private int _quantity;
        private Coroutine _coolDown;
        private Coroutine _duration;
        private bool _isAbilityUsed = false;
        private bool _isAutoCast = false;
        private DamageSource _damageSource;

        public Ability(
            ActiveAbilityData abilityAttributeData,
            int currentLevel,
            float abilityCooldownReduction,
            float abilityDuration,
            int abilityValue,
            bool isAutoCast,
            ICoroutineRunner coroutineRunner)
        {
            AbilityAttribute = abilityAttributeData;
            FillAbilityParameters(abilityAttributeData, currentLevel);
            AudioClip = abilityAttributeData.AudioClip;
            _coroutineRunner = coroutineRunner;
            _isAutoCast = isAutoCast;
            TypeAbility = abilityAttributeData.TypeAbility;
            TypeAttackAbility = (abilityAttributeData as AttackAbilityData) !=
                null ? (abilityAttributeData as AttackAbilityData).TypeAttackAbility : 0;
            TypeUpgradeMagic = abilityAttributeData.UpgradeType;
            TypeMagic = abilityAttributeData.MagicType;
            MaxLevel = abilityAttributeData.Parameters.Count;
            _spellRadius = (abilityAttributeData as AttackAbilityData).Radius;
            UpdateAbilityParameters(abilityDuration, abilityValue, abilityCooldownReduction);
        }

        public Ability(
            LegendaryAbilityData legendaryAbilityData,
            ActiveAbilityData abilityAttributeData,
            float abilityCooldownReduction,
            float abilityDuration,
            int abilityValue,
            bool isAutoCast,
            ICoroutineRunner coroutineRunner)
        {
            FillLegendaryAbilityParameters(legendaryAbilityData);
            AudioClip = legendaryAbilityData.AudioClip;
            _coroutineRunner = coroutineRunner;
            _isAutoCast = isAutoCast;
            TypeAbility = abilityAttributeData.TypeAbility;
            TypeAttackAbility = (abilityAttributeData as AttackAbilityData) !=
                null ? (abilityAttributeData as AttackAbilityData).TypeAttackAbility : 0;
            TypeUpgradeMagic = abilityAttributeData.UpgradeType;
            TypeMagic = abilityAttributeData.MagicType;
            MaxLevel = abilityAttributeData.Parameters.Count;
            UpdateAbilityParameters(abilityDuration, abilityValue, abilityCooldownReduction);
        }

        public Ability(ClassAbilityData classAbilityData,
            bool isAutoCast,
            int currentLvl,
            ICoroutineRunner coroutineRunner)
        {
            FillClassSkillParameter(classAbilityData, currentLvl);
            TypeAbility = classAbilityData.AbilityType;
            _isAutoCast = isAutoCast;
            _coroutineRunner = coroutineRunner;
            AmplifierParameters = classAbilityData.Parameters[currentLvl].CardParameters;
        }

        public event Action AbilityRemoved;
        public event Action<Ability> AbilityUsed;
        public event Action<Ability> AbilityEnded;
        public event Action<float> AbilityUpgraded;
        public event Action<float> CooldownValueChanged;
        public event Action<float> CooldownValueReseted;

        public List<CardParameter> AmplifierParameters { get; private set; } = new List<CardParameter>();
        public bool IsAbilityEnded { get; private set; } = true;
        public bool IsAutoCast => _isAutoCast;
        public float CurrentDuration => _currentDuration;
        public int CurrentAbilityValue => _currentAbilityValue;
        public int Quantity => _quantity;
        public float SpellRadius => _spellRadius;
        public bool IsAbilityUsed => _isAbilityUsed;
        public DamageSource DamageSource => _damageSource;
        public int CurrentLevel { get; private set; }
        public int MaxLevel { get; private set; }
        public TypeUpgradeAbility TypeUpgradeMagic { get; private set; }
        public TypeMagic TypeMagic { get; private set; }
        public TypeAbility TypeAbility { get; private set; }
        public TypeAttackAbility TypeAttackAbility { get; private set; }
        public ActiveAbilityData AbilityAttribute { get; private set; }
        public AudioClip AudioClip { get; private set; }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            AbilityRemoved?.Invoke();
        }

        public void Use()
        {
            if (_isAbilityUsed == false)
                ApplyAbility();
            else
                ResumeCoroutine();
        }

        public void StopCoroutine()
        {
            if (_duration != null)
                _coroutineRunner.StopCoroutine(_duration);

            if (_coolDown != null)
                _coroutineRunner.StopCoroutine(_coolDown);
        }

        public void Upgrade(ActiveAbilityData abilityAttributeData,
            int currentLevel,
            int abilityDuration,
            int abilityDamage,
            int abilityCooldownReduction)
        {
            FillAbilityParameters(abilityAttributeData, currentLevel);
            UpdateAbilityParameters(abilityDuration, abilityDamage, abilityCooldownReduction);
            CurrentLevel = currentLevel;
            AbilityUpgraded?.Invoke(_defaultCooldown);
        }

        public void UpdateAbilityParameters(float abilityDuration, 
           int abilityDamage,
           float abilityCooldownReduction)
        {
            _defaultCooldown -= abilityCooldownReduction;
            _defaultDuration += abilityDuration;
            _currentAbilityValue += abilityDamage;
            ApplyDamageSource();
        }

        private void FillAbilityParameters(ActiveAbilityData abilityAttributeData, int currentLevel)
        {
            foreach (CardParameter parameter in abilityAttributeData.Parameters[currentLevel].CardParameters)
            {
                if (parameter.TypeParameter == TypeParameter.AbilityCooldown)
                    _defaultCooldown = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityValue)
                    _currentAbilityValue = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityDuration)
                    _defaultDuration = parameter.Value;
            }

            if (abilityAttributeData as AttackAbilityData)
                _damageSource = (abilityAttributeData as AttackAbilityData).Damage;
        }

        private void FillLegendaryAbilityParameters(LegendaryAbilityData legendaryAbilityData)
        {
            foreach (CardParameter parameter in legendaryAbilityData.Parameters[_minValue].CardParameters)
            {
                if (parameter.TypeParameter == TypeParameter.AbilityCooldown)
                    _defaultCooldown = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityValue)
                    _currentAbilityValue = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityDuration)
                    _defaultDuration = parameter.Value;
            }

            _damageSource = legendaryAbilityData.Damage;
        }

        private void FillClassSkillParameter(ClassAbilityData abilityAttributeData, int currentLevel)
        {
            foreach (CardParameter parameter in abilityAttributeData.Parameters[currentLevel].CardParameters)
            {
                if (parameter.TypeParameter == TypeParameter.AbilityCooldown)
                    _defaultCooldown = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityValue)
                    _currentAbilityValue = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityDuration)
                    _defaultDuration = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityDamage)
                    _abilityDamage = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.Chance)
                    _chance = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.Quantity)
                    _quantity = parameter.Value;
            }

            _damageSource = abilityAttributeData.DamageParameter;

            if (_damageSource != null)
                ApplyDamageSource();

            _damageSource.ChangeDamage(_abilityDamage);
        }

        private void ApplyDamageSource() 
        {
            _damageSource.ChangeDamage(_currentAbilityValue);

            foreach (var parameter in _damageSource.DamageParameters)
            {
                switch (parameter.TypeDamageParameter)
                {
                    case TypeDamageParameter.Chance:
                        parameter.ChangeParameterValue(_chance);
                        break;
                    case TypeDamageParameter.Duration:
                        parameter.ChangeParameterValue(_defaultDuration);
                        break;
                }
            }
        }

        private void ApplyAbility()
        {
            _currentCooldown = _defaultCooldown;
            _currentDuration = _defaultDuration;
            IsAbilityEnded = false;
            UpdateAbility(true, _currentCooldown);
            _duration = _coroutineRunner.StartCoroutine(DurationAbility());
            ResumeCoroutine();
            AbilityUsed?.Invoke(this);
        }

        public void ResumeCoroutine()
        {
            if (_coolDown != null)
                _coroutineRunner.StopCoroutine(_coolDown);

            if (_duration != null)
                _coroutineRunner.StopCoroutine(_duration);

            _coolDown = _coroutineRunner.StartCoroutine(CoolDown());
            _duration = _coroutineRunner.StartCoroutine(DurationAbility());
        }

        private void UpdateAbility(bool state, float delay)
        {
            _isAbilityUsed = state;
            CooldownValueReseted?.Invoke(delay);
        }

        private void ReleaseUnmanagedResources()
        {
            if (_duration != null)
                _coroutineRunner.StopCoroutine(_duration);

            if (_coolDown != null)
                _coroutineRunner.StopCoroutine(_coolDown);
        }

        private IEnumerator DurationAbility()
        {
            while (_currentDuration > _minValue)
            {
                _currentDuration -= Time.deltaTime;
                yield return null;
            }

            if (IsAbilityEnded == false)
                AbilityEnded?.Invoke(this);

            IsAbilityEnded = true;
        }

        private IEnumerator CoolDown()
        {
            while (_currentCooldown > _minValue)
            {
                _currentCooldown -= Time.deltaTime;
                CooldownValueChanged?.Invoke(_currentCooldown);
                yield return null;
            }

            UpdateAbility(false, _minValue);

            if (_isAutoCast)
                Use();
        }
    }
}