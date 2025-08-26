using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.GamePanels;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Views;
using System;
using UniRx;
using UnityEditor.Playables;
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

        private ParticleSystem _abilityEffect;
        private CompositeDisposable _disposables = new();

        public event Action<ClassAbilityData, ClassSkillButtonView, int> ClassAbilityViewCreated;
        public event Action<AbilityView, ParticleSystem, ActiveAbilityData> LegendaryAbilityViewCreated;
        public event Action<PassiveAbilityView> PassiveAbilityViewCreated;

        private void OnDestroy()
        {
            if (_disposables != null)
                _disposables.Dispose();
        }

        public void Initialize(Sprite iconPlayer, PlayerHealth playerHealth)
        {
            _playerMapIcon.sprite = iconPlayer;
            _playerIcon.sprite = iconPlayer;
            _sliderHP.maxValue = playerHealth.GetMaxHealth();
            _sliderHP.value = playerHealth.GetCurrentHealth();

            playerHealth.MaxHealthChanged
                .Subscribe(maxHealth => _sliderHP.maxValue = maxHealth)
                .AddTo(this);

            playerHealth.CurrentHealthChanged
                .Subscribe(currentHealth => _sliderHP.value = currentHealth)
                .AddTo(this);

            AddListeners();
        }

        private void TakeClassAbility(ClassAbilityData abilityData, int currentLevel)
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

            foreach (var parameter in abilityData.Parameters[currentLevel - 1].CardParameters)
            {
                if (parameter.TypeParameter == TypeParameter.AbilityCooldown)
                    currentAbilityCooldown = parameter.Value;
            }

            abilityView.Initialize(abilityData.Icon, currentAbilityCooldown);
            ClassAbilityViewCreated?.Invoke(abilityData, abilityView, currentLevel);
        }

        private void TakePassiveAbility(PassiveAttributeData passiveAttributeData)
        {
            PassiveAbilityView view = Instantiate(passiveAttributeData.AbilityView, _passiveAbilityContainer);
            view.Initialize(passiveAttributeData);
            PassiveAbilityViewCreated?.Invoke(view);
        }

        private void TakeLegendaryAbility(ActiveAbilityData abilityAttributeData)
        {
            float currentAbilityCooldown = 0f;
            _abilityEffect = abilityAttributeData.Particle;

            AbilityView abilityView = Instantiate(abilityAttributeData.AbilityView, _abilityObjectContainer);

            foreach (var parameter in abilityAttributeData.Parameters[0].CardParameters)
            {
                if (parameter.TypeParameter.ToString() == TypeParameter.AbilityCooldown.ToString())
                    currentAbilityCooldown = parameter.Value;
            }

            abilityView.Initialize(abilityAttributeData.Icon, currentAbilityCooldown);

            LegendaryAbilityViewCreated?.Invoke(
                abilityView, 
                abilityAttributeData.Particle, 
                abilityAttributeData);
        }

        private void TakeAbility(ActiveAbilityData abilityAttributeData, int currentLevel)
        {
            AbilityView abilityView;
            float currentAbilityCooldown = 0f;

            _abilityEffect = abilityAttributeData.Particle;

            foreach (var parameter in abilityAttributeData.Parameters[currentLevel].CardParameters)
            {
                if (parameter.TypeParameter.ToString() == TypeParameter.AbilityCooldown.ToString())
                    currentAbilityCooldown = parameter.Value;
            }

            abilityView = Instantiate(abilityAttributeData.AbilityView, _abilityObjectContainer);
            abilityView.Initialize(abilityAttributeData.Icon, currentAbilityCooldown);
            MessageBroker.Default.Publish(new M_AbilityViewCreat(abilityView, _abilityEffect));
        }

        private void ChangePlayerLevel(int currentLevel, int maxExperienceValue, int currentExperience)
        {
            _textPlayerLevel.text = currentLevel.ToString();
            _sliderXP.maxValue = maxExperienceValue;
            _sliderXP.value = currentExperience;
        }

        private void ChangeUpgradeLevel(int currentLevel, int maxExperienceValue, int currentExperience)
        {
            _textUpgradePoints.text = currentLevel.ToString();
            _sliderUpgradePoints.maxValue = maxExperienceValue;
            _sliderUpgradePoints.value = currentExperience;
        }

        private void ChangeExperience(int target)
        {
            _sliderXP.value += target;
        }

        private void ChangeUpgradeExperience(int target)
        {
            _sliderUpgradePoints.value += target;
        }

        private void ChangeKillCount(int value)
        {
            _killCount.text = value.ToString();
        }

        private void AddListeners()
        {
            MessageBroker.Default
              .Receive<M_ClassAbilityTake>()
              .Subscribe(m => TakeClassAbility(
                  m.ClassAbilityData,
              m.CurrentLvl))
              .AddTo(_disposables);

            MessageBroker.Default
               .Receive<M_PassiveAbilityTake>()
               .Subscribe(m => TakePassiveAbility(
                   m.PassiveAttributeData))
               .AddTo(_disposables);

            MessageBroker.Default
              .Receive<M_LegendaryAbilityTake>()
              .Subscribe(m => TakeLegendaryAbility(
                  m.ActiveAbilityData))
              .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_KillCountChange>()
                .Subscribe(m => ChangeKillCount(m.Value))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_UpgradeExperienceValueChange>()
                .Subscribe(m => ChangeUpgradeExperience(m.Value))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_ExperienceValueChange>()
                .Subscribe(m => ChangeExperience(m.Value))
                .AddTo(_disposables);

            MessageBroker.Default
                 .Receive<M_PlayerLevelChange>()
                .Subscribe(m => ChangePlayerLevel(
                    m.CurrentLevel,
                    m.MaxExperienceValue,
                    m.CurrentExperience))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_PlayerUpgradeLevelChange>()
                .Subscribe(m => ChangeUpgradeLevel(
                    m.CurrentUpgradeLevel,
                    m.MaxExperienceValue,
                    m.CurrentUpgradeExperience))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_AbilityTake>()
                .Subscribe(m => TakeAbility(
                    m.ActiveAbilityData,
                    m.CurrentLvl))
                .AddTo(_disposables);
        }
    }
}