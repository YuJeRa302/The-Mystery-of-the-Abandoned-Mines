using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Characters
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
        [Space(20)]
        [SerializeField] private Image _playerMapIcon;
        [SerializeField] private Sprite _closeAbilityIcon;

        private Transform _throwPoint;
        private ParticleSystem _abilityEffect;

        public event Action<AbilityView, ParticleSystem, Transform> AbilityViewCreated;
        public event Action<ClassAbilityData, ClassSkillButtonView, int> ClassAbilityViewCreated;
        public event Action<AbilityView, ParticleSystem, Transform, ActiveAbilityData> LegendaryAbilityViewCreated;
        public event Action<PassiveAbilityView> PassiveAbilityViewCreated;

        public void Initialize(Sprite iconPlayer, Transform throwPoint)
        {
            _playerMapIcon.sprite = iconPlayer;
            _playerIcon.sprite = iconPlayer;
            _throwPoint = throwPoint;
        }

        public void TakeClassAbility(ClassAbilityData abilityData, int currentLevel)
        {
            ClassSkillButtonView abilityView;
            float currentAbilityCooldown = 0f;
            abilityView = Instantiate(abilityData.ButtonView, _classAbilityContainer);

            if (currentLevel == 0)
            {
                abilityView.Initialize(_closeAbilityIcon);
                abilityView.SetInteractableButton(false);
                return;
            }

            foreach (var parametr in abilityData.Parameters[currentLevel - 1].CardParameters)
            {
                if (parametr.TypeParameter == TypeParameter.AbilityCooldown)
                    currentAbilityCooldown = parametr.Value;
            }

            abilityView.Initialize(abilityData.Icon, currentAbilityCooldown);
            ClassAbilityViewCreated?.Invoke(abilityData, abilityView, currentLevel);
        }

        public void TakePassiveAbility(PassiveAttributeData passiveAttributeData)
        {
            PassiveAbilityView view = Instantiate(passiveAttributeData.AbilityView, _passiveAbilityContainer);
            view.Initialize(passiveAttributeData);
            PassiveAbilityViewCreated?.Invoke(view);
        }

        public void TakeLegendaryAbility(ActiveAbilityData abilityAttributeData)
        {
            float currentAbilityCooldown = 0f;
            _abilityEffect = abilityAttributeData.Particle;

            AbilityView abilityView = Instantiate(abilityAttributeData.AbilityView, _abilityObjectContainer);

            foreach (var parametr in abilityAttributeData.Parameters[0].CardParameters)
            {
                if (parametr.TypeParameter == TypeParameter.AbilityCooldown)
                    currentAbilityCooldown = parametr.Value;
            }

            abilityView.Initialize(abilityAttributeData.Icon, currentAbilityCooldown);
            LegendaryAbilityViewCreated?.Invoke(
                abilityView, abilityAttributeData.Particle, _throwPoint, abilityAttributeData);
        }

        public void TakeAbility(ActiveAbilityData abilityAttributeData, int currentLevel)
        {
            AbilityView abilityView;
            float currentAbilityCooldown = 0f;

            _abilityEffect = abilityAttributeData.Particle;

            foreach (var parametr in abilityAttributeData.Parameters[currentLevel].CardParameters)
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