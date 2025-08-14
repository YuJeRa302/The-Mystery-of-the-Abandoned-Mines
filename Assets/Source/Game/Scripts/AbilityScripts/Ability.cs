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

        private Dictionary<TypeParameter, float> _abilityParameters = new ();
        private float _currentDuration;
        private float _currentCooldown;
        private float _spellRadius;
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
            FillAbilityParameters(abilityAttributeData, currentLevel);
            AbilityAttribute = abilityAttributeData;
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
            FillAbilityParameters(legendaryAbilityData, _minValue);
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
            FillAbilityParameters(classAbilityData, currentLvl);
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
        public float CurrentDuration => _abilityParameters.ContainsKey(TypeParameter.AbilityDuration) 
            ?_abilityParameters[TypeParameter.AbilityDuration]
            : 0f;
        public int CurrentAbilityValue => Convert.ToInt32(_abilityParameters[TypeParameter.AbilityValue]);
        public Dictionary<TypeParameter, float> AbilityParameters => _abilityParameters;
        public float SpellRadius => _spellRadius;
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

        public void Upgrade(
            ActiveAbilityData abilityAttributeData,
            int currentLevel,
            int abilityDuration,
            int abilityDamage,
            int abilityCooldownReduction)
        {
            FillAbilityParameters(abilityAttributeData, currentLevel);
            UpdateAbilityParameters(abilityDuration, abilityDamage, abilityCooldownReduction);

            if (abilityAttributeData is LegendaryAbilityData == false)
                ApplyDamageSource();

            CurrentLevel = currentLevel;
            AbilityUpgraded?.Invoke(_abilityParameters[TypeParameter.AbilityCooldown]);
        }

        public void UpdateAbilityParameters(
            float abilityDuration,
            int abilityDamage,
            float abilityCooldownReduction)
        {
            _abilityParameters.TryGetValue(TypeParameter.AbilityCooldown, out float defaultAbilityCooldown);
            _abilityParameters[TypeParameter.AbilityCooldown] = defaultAbilityCooldown - abilityCooldownReduction;

            _abilityParameters.TryGetValue(TypeParameter.AbilityDuration, out float defaultAbilityDuration);
            _abilityParameters[TypeParameter.AbilityDuration] = defaultAbilityDuration + abilityDuration;

            _abilityParameters.TryGetValue(TypeParameter.AbilityValue, out float defaultAbilityDamage);
            _abilityParameters[TypeParameter.AbilityValue] = defaultAbilityDamage + abilityDamage;
        }

        private void FillAbilityParameters(AttributeData attributeData, int currentLevel)
        {
            DamageSource damageSource = null;
            float damage = 0f;

            foreach (CardParameter parameter in attributeData.Parameters[currentLevel].CardParameters)
            {
                if (_abilityParameters.ContainsKey(parameter.TypeParameter) == false)
                    _abilityParameters.Add(parameter.TypeParameter, parameter.Value);
                else
                    _abilityParameters[parameter.TypeParameter] = parameter.Value;
            }

            if (attributeData is PassiveAttributeData || attributeData is PrimaryAttributeData)
                return;

            if (attributeData is ActiveAbilityData)
                damageSource = (attributeData as ActiveAbilityData).DamageSource;

            if (attributeData is ClassAbilityData)
                damageSource = (attributeData as ClassAbilityData).DamageSource;

            if (_abilityParameters.ContainsKey(TypeParameter.AbilityDamage))
                damage = _abilityParameters[TypeParameter.AbilityDamage];

            if (_abilityParameters.ContainsKey(TypeParameter.AbilityValue))
                damage = _abilityParameters[TypeParameter.AbilityValue];

            _damageSource = new DamageSource(
                damageSource.TypeDamage,
                damageSource.DamageParameters,
                damageSource.PoolParticle,
                damage);

            if (attributeData is LegendaryAbilityData == false)
                ApplyDamageSource();
        }

        private void ApplyDamageSource()
        {
            foreach (var parameter in _damageSource.DamageParameters)
            {
                if(parameter.TypeDamageParameter == TypeDamageParameter.Duration)
                    parameter.ChangeParameterValue(_abilityParameters[TypeParameter.AbilityDuration]);
            }
        }

        private void ApplyAbility()
        {
            _abilityParameters.TryGetValue(TypeParameter.AbilityCooldown, out float abilityCooldown);
            _currentCooldown = abilityCooldown;

            _abilityParameters.TryGetValue(TypeParameter.AbilityDuration, out float abilityDuration);
            _currentDuration = abilityDuration;

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