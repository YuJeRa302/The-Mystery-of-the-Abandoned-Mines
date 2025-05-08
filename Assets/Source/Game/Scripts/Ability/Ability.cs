using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Ability : IDisposable
    {
        private readonly int _minValue = 0;
        private readonly ICoroutineRunner _coroutineRunner;

        private float _currentDuration;
        private float _defaultDuration;
        private int _currentAbilityValue;
        private float _abilityCooldownReduction;
        private float _abilityDuration;
        private float _moveSpeed;
        private float _reduceHealth;
        private float _chance;
        private int _abilityValue;
        private int _regeneration;
        private int _defailtDamage;
        private int _abilityDamage;
        private int _defailtArmor;
        private int _defailyHealing;
        private float _defaultCooldown;
        private float _currentCooldown;
        private float _spellRadius;
        private int _quantily;
        private AudioClip _audioClip;
        private Coroutine _coolDown;
        private Coroutine _duration;
        private bool _isAbilityUsed = false;
        private bool _isAutoCast = false;
        private DamageSource _damageSource;

        public event Action AbilityRemoved;
        public event Action<Ability> AbilityUsed;
        public event Action<Ability> AbilityEnded;
        public event Action<float> AbilityUpgraded;
        public event Action<float> CooldownValueChanged;
        public event Action<float> CooldownValueReseted;

        public List<CardParameter> AmplifierParametrs { get; private set; } = new List<CardParameter>();
        public bool IsAbilityEnded { get; private set; } = false;
        public bool IsAutoCast => _isAutoCast;
        public float CurrentDuration => _currentDuration;
        public float MoveSpeed => _moveSpeed;
        public float ReduceHealth => _reduceHealth;
        public int CurrentAbilityValue => _currentAbilityValue;
        public int DefailtDamage => _defailtDamage;
        public int Regeneration => _regeneration;
        public int DefailtArmor => _defailtArmor;
        public int AbilityDamage => _abilityDamage;
        public int DefailyHealing => _defailyHealing;
        public int Quantily => _quantily;
        public float SpellRadius => _spellRadius;
        public bool IsAbilityUsed => _isAbilityUsed;

        public DamageSource DamageSource => _damageSource;
        public int CurrentLevel { get; private set; }
        public int MaxLevel { get; private set; }
        public TypeUpgradeAbility TypeUpgradeMagic { get; private set; }
        public TypeMagic TypeMagic { get; private set; }
        public TypeAbility TypeAbility { get; private set; }
        public TypeAttackAbility TypeAttackAbility { get; private set; }
        public AbilityAttributeData AbilityAttribute { get; private set; }

        public Ability(
            AbilityAttributeData abilityAttributeData,
            int currentLevel,
            float abilityCooldownReduction,
            float abilityDuration,
            int abilityValue,
            bool isAutoCast,
            ICoroutineRunner coroutineRunner)
        {
            AbilityAttribute = abilityAttributeData;
            _abilityValue = abilityValue;
            FillAbilityParameters(abilityAttributeData, currentLevel);
            _audioClip = abilityAttributeData.AudioClip;
            _coroutineRunner = coroutineRunner;
            _abilityCooldownReduction = abilityCooldownReduction;
            _abilityDuration = abilityDuration;
            _isAutoCast = isAutoCast;
            TypeAbility = abilityAttributeData.TypeAbility;
            TypeAttackAbility = (abilityAttributeData as AttackAbilityData) != null ? (abilityAttributeData as AttackAbilityData).TypeAttackAbility : 0;
            TypeUpgradeMagic = abilityAttributeData.TypeUpgradeMagic;
            TypeMagic = abilityAttributeData.TypeMagic;
            MaxLevel = abilityAttributeData.CardParameters.Count;
            _spellRadius = (abilityAttributeData as AttackAbilityData).SpellRadius;
            UpdateAbilityParamters();
        }

        public Ability(
            LegendaryAbilityData legendaryAbilityData,
            AbilityAttributeData abilityAttributeData,
            float abilityCooldownReduction,
            float abilityDuration,
            int abilityValue,
            bool isAutoCast,
            ICoroutineRunner coroutineRunner)
        {
            _abilityValue = abilityValue;
            FillLegendaryAbilityParameters(legendaryAbilityData);
            _audioClip = abilityAttributeData.AudioClip;
            _coroutineRunner = coroutineRunner;
            _abilityCooldownReduction = abilityCooldownReduction;
            _abilityDuration = abilityDuration;
            _isAutoCast = isAutoCast;
            TypeAbility = abilityAttributeData.TypeAbility;
            TypeAttackAbility = (abilityAttributeData as AttackAbilityData) != null ? (abilityAttributeData as AttackAbilityData).TypeAttackAbility : 0;
            TypeUpgradeMagic = abilityAttributeData.TypeUpgradeMagic;
            TypeMagic = abilityAttributeData.TypeMagic;
            MaxLevel = abilityAttributeData.CardParameters.Count;
            UpdateAbilityParamters();
        }

        public Ability(ClassAbilityData classAbilityData, bool isAutoCast, int currentLvl, ICoroutineRunner coroutineRunner)
        {
            FillClassSkillParametr(classAbilityData, currentLvl);
            TypeAbility = classAbilityData.AbilityType;
            _isAutoCast = isAutoCast;
            _coroutineRunner = coroutineRunner;
            AmplifierParametrs = classAbilityData.Parameters[currentLvl].CardParameters;
        }

        public void Dispose()
        {
            AbilityRemoved?.Invoke();
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
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

        public void Upgrade(AbilityAttributeData abilityAttributeData, int currentLevel)
        {
            FillAbilityParameters(abilityAttributeData, currentLevel);
            UpdateAbilityParamters();
            CurrentLevel = currentLevel;
            AbilityUpgraded?.Invoke(_defaultCooldown);
        }

        private void FillAbilityParameters(AbilityAttributeData abilityAttributeData, int currentLevel)
        {
            foreach (CardParameter parameter in abilityAttributeData.CardParameters[currentLevel].CardParameters)
            {
                if (parameter.TypeParameter == TypeParameter.AbilityCooldown)
                    _defaultCooldown = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityValue)
                    _currentAbilityValue = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityDuration)
                    _defaultDuration = parameter.Value;
            }

            if (abilityAttributeData as AttackAbilityData)
                _damageSource = (abilityAttributeData as AttackAbilityData).DamageSource;

            if (_damageSource != null)
                ApplyDamageSource();
        }

        private void FillLegendaryAbilityParameters(LegendaryAbilityData legendaryAbilityData)
        {
            foreach (CardParameter parameter in legendaryAbilityData.LegendaryAbilityParameters[_minValue].CardParameters)
            {
                if (parameter.TypeParameter == TypeParameter.AbilityCooldown)
                    _defaultCooldown = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityValue)
                    _currentAbilityValue = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityDuration)
                    _defaultDuration = parameter.Value;
            }

            _damageSource = legendaryAbilityData.DamageParametr;

            if (_damageSource != null)
                ApplyDamageSource();
        }

        private void FillClassSkillParametr(ClassAbilityData abilityAttributeData, int currentLevel)
        {
            foreach (CardParameter parameter in abilityAttributeData.Parameters[currentLevel].CardParameters)
            {
                if (parameter.TypeParameter == TypeParameter.AbilityCooldown)
                    _defaultCooldown = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityValue)
                    _currentAbilityValue = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityDuration)
                    _defaultDuration = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.Damage)
                    _defailtDamage = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.Armor)
                    _defailtArmor = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.MoveSpeed)
                    _moveSpeed = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.Healing)
                    _defailyHealing = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityDamage)
                    _abilityDamage = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.HealtReduce)
                    _reduceHealth = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.Regeneration)
                    _regeneration = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.Chance)
                    _chance = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.Quantity)
                    _quantily = parameter.Value;
            }

            _damageSource = abilityAttributeData.DamageParametr;

            if (_damageSource != null)
                ApplyDamageSource();

            _damageSource.ChangeDamage(_abilityDamage);
        }

        private void UpdateAbilityParamters()
        {
            _defaultCooldown -= _abilityCooldownReduction;
            
            if (_defaultCooldown <= 0)
                _defaultCooldown = 1f;

            _defaultDuration += _abilityDuration;
            _currentAbilityValue += _abilityValue;
        }

        private void ApplyDamageSource() 
        {
            _damageSource.ChangeDamage(_currentAbilityValue);

            foreach (var parametr in _damageSource.DamageParameters)
            {
                switch (parametr.TypeDamageParameter)
                {
                    case TypeDamageParameter.Chance:
                        parametr.ChangeParameterValue(_chance); // откуда береться значение chance?
                        break;
                    case TypeDamageParameter.Duration:
                        parametr.ChangeParameterValue(_defaultDuration);
                        break;
                }
            }
        }

        private void ApplyAbility()
        {
            _currentCooldown = _defaultCooldown;
            _currentDuration = _defaultDuration;
            IsAbilityEnded = false;
            //Player.PlayerSounds.PlayAbilityAudio(_audioClip);
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