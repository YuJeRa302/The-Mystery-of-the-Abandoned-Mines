using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public abstract class Ability : MonoBehaviour
    {
        private readonly int _minValue = 0;
        private readonly int _indexAbilityDuration = 1;
        private readonly int _indexAbilityDelay = 2;
        private readonly int _indexAbilityDamage = 0;

        [SerializeField] private Image _reloadingImage;
        [SerializeField] private Image _abilityIcon;

        protected Spell Spell;
        protected bool IsAbilityUsed = false;
        protected bool IsAbilityEnded = false;
        protected Player Player;
        private Transform _throwPoint;
        private float _currentDuration;
        private float _defaultDuration;
        private int _currentAbilityValue;
        private TypeAbility _typeAbility;
        private TypeAttackAbility _typeAttackAbility;
        private float _defaultDelay;
        private float _currentDelay;
        private ParticleSystem _particleSystem;
        private Coroutine _coolDown;
        private AudioClip _audioClip;

        public event Action<Ability> AbilityUsed;
        public event Action<Ability> AbilityEnded;

        public float CurrentDuration => _currentDuration;
        public Transform ThrowPoint => _throwPoint;
        public int CurrentAbilityValue => _currentAbilityValue;
        public TypeAbility TypeAbility => _typeAbility;
        public TypeAttackAbility TypeAttackAbility => _typeAttackAbility;
        public ParticleSystem ParticleSystem => _particleSystem;

        private void OnEnable()
        {
            Use();
        }

        private void OnDisable()
        {
            if (_coolDown != null)
                StopCoroutine(_coolDown);
        }

        private void OnDestroy()
        {
            if (_coolDown != null)
                StopCoroutine(_coolDown);

            AbilityEnded?.Invoke(this);
        }

        public void Initialize(Player player, Transform throwPoint, AbilityAttributeData abilityAttributeData, int currentLevel, ParticleSystem particleSystem)
        {
            Player = player;
            FillCardParameters(abilityAttributeData, currentLevel);
            _typeAbility = abilityAttributeData.TypeAbility;
            _particleSystem = particleSystem;
            _abilityIcon.sprite = abilityAttributeData.Icon;
            _defaultDelay = abilityAttributeData.CardParameters[currentLevel].CardParameters[_indexAbilityDelay].Value;
            _currentDuration = abilityAttributeData.CardParameters[currentLevel].CardParameters[_indexAbilityDuration].Value;
            _currentAbilityValue = abilityAttributeData.CardParameters[currentLevel].CardParameters[_indexAbilityDamage].Value;

            if (_typeAbility == TypeAbility.AttackAbility)
            {
                _throwPoint = throwPoint;
                Spell = (abilityAttributeData as AttackAbilityData).Spell;
                _typeAttackAbility = (abilityAttributeData as AttackAbilityData).TypeAttackAbility;
            }

            _audioClip = abilityAttributeData.AudioClip;
        }

        protected virtual void ApplyAbility()
        {
            AbilityUsed?.Invoke(this);
            IsAbilityEnded = false;
            _currentDelay = _defaultDelay;
            _currentDuration = _defaultDuration;
            StartCoroutine(DurationAbility());
            //Player.PlayerSounds.PlayAbilityAudio(_audioClip);
            UpdateAbility(true, _currentDelay);
            ResumeCooldown();
        }

        private void Use()
        {
            if (IsAbilityUsed == false)
            {
                ApplyAbility();
            }
            else
            {
                if (_coolDown != null)
                    StopCoroutine(_coolDown);

                _coolDown = StartCoroutine(CoolDown());
            }
        }

        private void FillCardParameters(AbilityAttributeData abilityAttributeData, int currentLevel)
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

        private void ResumeCooldown()
        {
            if (gameObject.activeSelf == true)
            {
                if (_coolDown != null)
                    StopCoroutine(_coolDown);

                _coolDown = StartCoroutine(CoolDown());
            }
        }

        private IEnumerator DurationAbility()
        {
            while (_currentDuration > _minValue)
            {
                _currentDuration -= Time.deltaTime;
                yield return null;
            }

            IsAbilityEnded = true;
            AbilityEnded?.Invoke(this);
        }

        private IEnumerator CoolDown()
        {
            while (_currentDelay > _minValue)
            {
                _currentDelay -= Time.deltaTime;
                _reloadingImage.fillAmount = _currentDelay / _defaultDelay;
                yield return null;
            }

            UpdateAbility(false, _minValue);
            Use();
        }

        private void UpdateAbility(bool state, float delay)
        {
            IsAbilityUsed = state;
            _reloadingImage.fillAmount = delay;
        }
    }
}