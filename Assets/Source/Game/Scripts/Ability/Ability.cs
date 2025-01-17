using System;
using System.Collections;
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
        private int _abilityValue;
        private TypeAbility _typeAbility;
        private TypeAttackAbility _typeAttackAbility;
        private float _defaultDelay;
        private float _currentDelay;
        private AudioClip _audioClip;
        private Coroutine _coolDown;
        private Coroutine _duration;
        private bool _isAbilityUsed = false;
        private bool _isAutoCast = false;

        public event Action AbilityRemoved;
        public event Action<Ability> AbilityUsed;
        public event Action<Ability> AbilityEnded;
        public event Action<float> AbilityUpgraded;
        public event Action<float> CooldownValueChanged;
        public event Action<float> CooldownValueReseted;

        public bool IsAbilityEnded { get; private set; } = false;
        public float CurrentDuration => _currentDuration;
        public int CurrentAbilityValue => _currentAbilityValue;
        public TypeAbility TypeAbility => _typeAbility;
        public TypeAttackAbility TypeAttackAbility => _typeAttackAbility;

        public Ability(
            AbilityAttributeData abilityAttributeData,
            int currentLevel,
            float abilityCooldownReduction,
            float abilityDuration,
            int abilityValue,
            bool isAutoCast,
            ICoroutineRunner coroutineRunner)
        {
            FillAbilityParameters(abilityAttributeData, currentLevel);
            _typeAbility = abilityAttributeData.TypeAbility;
            _audioClip = abilityAttributeData.AudioClip;
            _typeAttackAbility = (abilityAttributeData as AttackAbilityData) != null ? (abilityAttributeData as AttackAbilityData).TypeAttackAbility : 0;
            _coroutineRunner = coroutineRunner;
            _abilityCooldownReduction = abilityCooldownReduction;
            _abilityDuration = abilityDuration;
            _abilityValue = abilityValue;
            _isAutoCast = isAutoCast;
            UpdateAbilityParamters();
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
            AbilityUpgraded?.Invoke(_defaultDelay);
        }

        private void FillAbilityParameters(AbilityAttributeData abilityAttributeData, int currentLevel)
        {
            foreach (CardParameter parameter in abilityAttributeData.CardParameters[currentLevel].CardParameters)
            {
                if (parameter.TypeParameter == TypeParameter.AbilityCooldown)
                    _defaultDelay = parameter.Value;
                else if (parameter.TypeParameter == TypeParameter.AbilityValue)
                    _currentAbilityValue = parameter.Value;
                else
                    _defaultDuration = parameter.Value;
            }
        }

        private void UpdateAbilityParamters() 
        {
            _defaultDelay -= _abilityCooldownReduction;
            _defaultDuration += _abilityDuration;
            _currentAbilityValue += _abilityValue;
        }

        private void ApplyAbility()
        {
            _currentDelay = _defaultDelay;
            _currentDuration = _defaultDuration;
            IsAbilityEnded = false;
            //Player.PlayerSounds.PlayAbilityAudio(_audioClip);
            UpdateAbility(true, _currentDelay);
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

            if(IsAbilityEnded == false)
                AbilityEnded?.Invoke(this);

            IsAbilityEnded = true;
        }

        private IEnumerator CoolDown()
        {
            while (_currentDelay > _minValue)
            {
                _currentDelay -= Time.deltaTime;
                CooldownValueChanged?.Invoke(_currentDelay);
                yield return null;
            }

            UpdateAbility(false, _minValue);

            if (_isAutoCast)
                Use();
        }
    }
}