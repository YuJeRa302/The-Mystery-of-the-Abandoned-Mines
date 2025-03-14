using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class PlayerView : MonoBehaviour
    {
        private readonly int _firstIndex = 0;
        private readonly int _indexAbilityDelay = 2;

        [SerializeField] private GameObject _mobileInterface;
        [Space(20)]
        [SerializeField] private Slider _sliderHP;
        [SerializeField] private Slider _sliderXP;
        [SerializeField] private Slider _sliderUpgradePoints;
        [Space(20)]
        [SerializeField] private Text _textPlayerLevel;
        [SerializeField] private Text _textUpgradePoints;
        [SerializeField] private Text _killCount;
        [Space(20)]
        [SerializeField] private Transform _abilityObjectContainer;
        [SerializeField] private Transform _classSkillsContainer;
        [SerializeField] private Transform _passiveAbilityContainer;
        [SerializeField] private Transform _playerEffectsContainer;
        [SerializeField] private Transform _weaponEffectsContainer;
        [Space(20)]
        [SerializeField] private Button _minimapButton;
        [SerializeField] private Button _minimapCloseButton;
        [SerializeField] private GameObject _minimap;
        [SerializeField] private Image _playerIcon;

        private Transform _throwPoint;
        private Player _player;
        private ParticleSystem _abilityEffect;
        private float _delay;

        public event Action<AbilityView, ParticleSystem, Transform> AbilityViewCreated;
        public event Action<ClassAbilityData, ClassSkillButtonView, int> CreatedClassSkillView;
        public event Action<AbilityView, ParticleSystem, Transform, AbilityAttributeData> LegendaryAbilityViewCreated;
        public event Action<PassiveAbilityView> PassiveAbilityViewCreated;

        private void OnDestroy()
        {
            _player.PlayerHealth.HealthChanged -= OnChangeHealth;
            _player.PlayerStats.ExperienceValueChanged -= OnChangeExperience;
            _player.PlayerStats.UpgradeExperienceValueChanged -= OnChangeUpgradeExperience;
            _player.PlayerAbilityCaster.AbilityTaked -= OnAbilityTaked;
            _player.PlayerAbilityCaster.PassiveAbilityTaked -= OnPassiveAbilityTaked;
            _player.PlayerAbilityCaster.LegendaryAbilityTaked -= OnLegendaryAbilityTaked;
            _player.PlayerStats.KillCountChanged -= OnChangeKillCount;
            _player.PlayerAbilityCaster.ClassSkillInitialized -= OnClassSkillViewCreate;
            _minimapButton.onClick.RemoveListener(OnMinimapButtonClick);
            _minimapCloseButton.onClick.RemoveListener(OnMinimapButtonClick);
        }

        public void Initialize(Player player, Sprite iconPlayer)
        {
            _player = player;
            _playerIcon.sprite = iconPlayer;
            SubscribePlayerEvent();
            _weaponEffectsContainer = player.WeaponAbilityContainer;
            _playerEffectsContainer = player.PlayerAbilityContainer;
            _throwPoint = player.ThrowAbilityPoint;
            _sliderHP.maxValue = _player.PlayerHealth.MaxHealth;
            _sliderHP.value = _player.PlayerHealth.CurrentHealth;
            _sliderXP.maxValue = _player.PlayerStats.MaxLevelValue;
            _sliderXP.value = _player.PlayerStats.CurrentExperience;
            _sliderUpgradePoints.maxValue = _player.PlayerStats.MaxUpgradeValue;
            _sliderUpgradePoints.value = _player.PlayerStats.UpgradeExperience;
            _textPlayerLevel.text = _player.PlayerStats.CurrentLevel.ToString();
            _textUpgradePoints.text = _player.PlayerStats.UpgradePoints.ToString();
            _killCount.text = _player.PlayerStats.CountKillEnemy.ToString();
            _playerEffectsContainer = _player.PlayerAbilityContainer;
            _weaponEffectsContainer = _player.WeaponAbilityContainer;
            _throwPoint = _player.ThrowAbilityPoint;
        }

        public void SetNewLevelValue(int value)
        {
            _textPlayerLevel.text = value.ToString();
        }

        public void SetNewUpgradeLevelValue(int value)
        {
            _textUpgradePoints.text = value.ToString();
        }

        public void SetExperienceSliderValue(int maxSlidervalue, int currentValue)
        {
            _sliderXP.maxValue = maxSlidervalue;
            _sliderXP.value = currentValue;
        }

        public void SetUpgradeExperienceSliderValue(int maxSlidervalue, int currentValue)
        {
            _sliderUpgradePoints.maxValue = maxSlidervalue;
            _sliderUpgradePoints.value = currentValue;
        }

        public void SetMobileInterface()
        {
            _mobileInterface.SetActive(true);
        }

        private void OnClassSkillViewCreate(ClassAbilityData abilityData, int currentLvl)
        {
            ClassSkillButtonView abilityView;

            abilityView = Instantiate(abilityData.ButtonView, _classSkillsContainer);

            foreach (var parametr in abilityData.Parameters[currentLvl].CardParameters)
            {
                if (parametr.TypeParameter == TypeParameter.AbilityCooldown)
                {
                    _delay = parametr.Value;
                }
            }

            abilityView.Initialize(abilityData.Icon, _delay);
            CreatedClassSkillView?.Invoke(abilityData, abilityView, currentLvl);
        }

        private void SubscribePlayerEvent()
        {
            _player.PlayerHealth.HealthChanged += OnChangeHealth;
            _player.PlayerStats.ExperienceValueChanged += OnChangeExperience;
            _player.PlayerStats.UpgradeExperienceValueChanged += OnChangeUpgradeExperience;
            _player.PlayerAbilityCaster.AbilityTaked += OnAbilityTaked;
            _player.PlayerAbilityCaster.PassiveAbilityTaked += OnPassiveAbilityTaked;
            _player.PlayerAbilityCaster.LegendaryAbilityTaked += OnLegendaryAbilityTaked;
            _player.PlayerStats.KillCountChanged += OnChangeKillCount;
            _player.PlayerAbilityCaster.ClassSkillInitialized += OnClassSkillViewCreate;
            _minimapButton.onClick.AddListener(OnMinimapButtonClick);
            _minimapCloseButton.onClick.AddListener(OnMinimapButtonClick);
        }

        private void OnMinimapButtonClick()
        {
            bool isActive = _minimap.activeSelf;
            _minimap.SetActive(!isActive);
        }

        private void OnPassiveAbilityTaked(PassiveAttributeData passiveAttributeData)
        {
            PassiveAbilityView view = Instantiate(passiveAttributeData.PassiveAbilityView, _passiveAbilityContainer);
            view.Initialize(passiveAttributeData);
            PassiveAbilityViewCreated?.Invoke(view);
        }

        private void OnLegendaryAbilityTaked(AbilityAttributeData abilityAttributeData)
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

            foreach (var parametr in abilityAttributeData.LegendaryAbilityData.LegendaryAbilityParameters[_firstIndex].CardParameters)
            {
                if (parametr.TypeParameter == TypeParameter.AbilityCooldown)
                    currentAbilityCooldown = parametr.Value;
            }

            abilityView.Initialize(abilityAttributeData.LegendaryAbilityData.Icon, currentAbilityCooldown);
            LegendaryAbilityViewCreated?.Invoke(abilityView, _abilityEffect, _throwPoint, abilityAttributeData);
        }

        private void OnAbilityTaked(AbilityAttributeData abilityAttributeData, int currentLevel)
        {
            AbilityView abilityView;

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

            abilityView = Instantiate(abilityAttributeData.AbilityView, _abilityObjectContainer);
            abilityView.Initialize(abilityAttributeData.Icon, abilityAttributeData.CardParameters[currentLevel].CardParameters[_indexAbilityDelay].Value);
            AbilityViewCreated?.Invoke(abilityView, _abilityEffect, _throwPoint);
        }

        private void OnChangeExperience(int target)
        {
            _sliderXP.value += target;
        }

        private void OnChangeUpgradeExperience(int target)
        {
            _sliderUpgradePoints.value += target;
        }

        private void OnChangeHealth(int target)
        {
            _sliderHP.value = target;
        }

        private void OnChangeKillCount(int value)
        {
            _killCount.text = value.ToString();
        }
    }
}