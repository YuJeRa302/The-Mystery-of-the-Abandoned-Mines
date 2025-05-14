using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private GameObject _mobileInterface;
        [Space(20)]
        [SerializeField] private Slider _sliderHP;
        [SerializeField] private Slider _sliderXP;
        [SerializeField] private Slider _sliderUpgradePoints;
        [SerializeField] private Image _playerIcon;
        [Space(20)]
        [SerializeField] private Text _textPlayerLevel;
        [SerializeField] private Text _textUpgradePoints;
        [SerializeField] private Text _killCount;
        [Space(20)]
        [SerializeField] private Transform _abilityObjectContainer;
        [SerializeField] private Transform _classAbilityContainer;
        [SerializeField] private Transform _passiveAbilityContainer;
        [SerializeField] private Transform _playerEffectsContainer;
        [SerializeField] private Transform _weaponEffectsContainer;
        [Space(20)]
        [SerializeField] private Image _playerMapIcon;

        private Transform _throwPoint;
        private ParticleSystem _abilityEffect;

        public event Action<AbilityView, ParticleSystem, Transform> AbilityViewCreated;
        public event Action<ClassAbilityData, ClassSkillButtonView, int> ClassAbilityViewCreated;
        public event Action<AbilityView, ParticleSystem, Transform, AbilityAttributeData> LegendaryAbilityViewCreated;
        public event Action<PassiveAbilityView> PassiveAbilityViewCreated;

        public void Initialize(Sprite iconPlayer, Transform throwPoint, Transform playerEffectsContainer, Transform weaponEffectsContainer)
        {
            _playerMapIcon.sprite = iconPlayer;
            _playerIcon.sprite = iconPlayer;
            _weaponEffectsContainer = weaponEffectsContainer;
            _playerEffectsContainer = playerEffectsContainer;
            _throwPoint = throwPoint;
        }

        public void SetMobileInterface()
        {
            _mobileInterface.SetActive(true);
        }

        public void TakeClassAbility(ClassAbilityData abilityData, int currentLevel)
        {
            if (currentLevel == 0)
                return;

            ClassSkillButtonView abilityView;
            float currentAbilityCooldown = 0f;
            abilityView = Instantiate(abilityData.ButtonView, _classAbilityContainer);

            foreach (var parametr in abilityData.Parameters[currentLevel-1].CardParameters)
            {
                if (parametr.TypeParameter == TypeParameter.AbilityCooldown)
                    currentAbilityCooldown = parametr.Value;
            }

            if (currentLevel <= 0)
            {
                abilityView.Initialize(abilityData.Icon, currentAbilityCooldown);
                abilityView.SetInerectableButton(false);
            }
            else
            {
                abilityView.Initialize(abilityData.Icon, currentAbilityCooldown);
                ClassAbilityViewCreated?.Invoke(abilityData, abilityView, currentLevel);
            }
        }

        public void TakePassiveAbility(PassiveAttributeData passiveAttributeData)
        {
            PassiveAbilityView view = Instantiate(passiveAttributeData.PassiveAbilityView, _passiveAbilityContainer);
            view.Initialize(passiveAttributeData);
            PassiveAbilityViewCreated?.Invoke(view);
        }

        public void TakeLegendaryAbility(AbilityAttributeData abilityAttributeData)
        {
            float currentAbilityCooldown = 0f;

            if (abilityAttributeData.TypeAbility != TypeAbility.AttackAbility)
            {
                if ((abilityAttributeData as DefaultAbilityData).TypeEffect == TypeEffect.Weapon)
                    _abilityEffect = Instantiate(abilityAttributeData.ParticleSystem, _weaponEffectsContainer);
                else
                    _abilityEffect = Instantiate(abilityAttributeData.ParticleSystem, _playerEffectsContainer);
            }
            else
            {
                _abilityEffect = abilityAttributeData.ParticleSystem;
            }

            AbilityView abilityView = Instantiate(abilityAttributeData.AbilityView, _abilityObjectContainer);

            foreach (var parametr in abilityAttributeData.LegendaryAbilityData.LegendaryAbilityParameters[0].CardParameters)
            {
                if (parametr.TypeParameter == TypeParameter.AbilityCooldown)
                    currentAbilityCooldown = parametr.Value;
            }

            abilityView.Initialize(abilityAttributeData.LegendaryAbilityData.Icon, currentAbilityCooldown);
            LegendaryAbilityViewCreated?.Invoke(abilityView, _abilityEffect, _throwPoint, abilityAttributeData);
        }

        public void TakeAbility(AbilityAttributeData abilityAttributeData, int currentLevel)
        {
            AbilityView abilityView;
            float currentAbilityCooldown = 0f;

            if (abilityAttributeData.TypeAbility != TypeAbility.AttackAbility)
            {
                if ((abilityAttributeData as DefaultAbilityData).TypeEffect == TypeEffect.Weapon)
                    _abilityEffect = Instantiate(abilityAttributeData.ParticleSystem, _weaponEffectsContainer);
                else
                    _abilityEffect = Instantiate(abilityAttributeData.ParticleSystem, _playerEffectsContainer);
            }
            else
            {
                _abilityEffect = abilityAttributeData.ParticleSystem;
            }

            foreach (var parametr in abilityAttributeData.CardParameters[currentLevel].CardParameters)
            {
                if (parametr.TypeParameter == TypeParameter.AbilityCooldown)
                    currentAbilityCooldown = parametr.Value;
            }

            abilityView = Instantiate(abilityAttributeData.AbilityView, _abilityObjectContainer);
            abilityView.Initialize(abilityAttributeData.Icon, currentAbilityCooldown);
            AbilityViewCreated?.Invoke(abilityView, _abilityEffect, _throwPoint);
        }

        public void ChangePlayerLevel(int currenLevel, int maxExperienceValue, int currentExperience)
        {
            _textPlayerLevel.text = currenLevel.ToString();
            _sliderXP.maxValue = maxExperienceValue;
            _sliderXP.value = currentExperience;
        }

        public void ChangeUpgradeLevel(int currenLevel, int maxExperienceValue, int currentExperience)
        {
            _textUpgradePoints.text = currenLevel.ToString();
            _sliderUpgradePoints.maxValue = maxExperienceValue;
            _sliderUpgradePoints.value = currentExperience;
        }

        public void ChangeMaxHealthValue(int maxHealthValue, int currentHealthValue) 
        {
            _sliderHP.maxValue = maxHealthValue;
            _sliderHP.value = currentHealthValue;
        }

        public void ChangeExperience(int target)
        {
            _sliderXP.value += target;
        }

        public void ChangeUpgradeExperience(int target)
        {
            _sliderUpgradePoints.value += target;
        }

        public void ChangeHealth(int target)
        {
            _sliderHP.value = target;
        }

        public void ChangeKillCount(int value)
        {
            _killCount.text = value.ToString();
        }
    }
}