using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class PlayerView : MonoBehaviour
    {
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
        [SerializeField] private Transform _playerEffectsContainer;
        [SerializeField] private Transform _weaponEffectsContainer;
        [SerializeField] private Transform _throwPoint;
        [Space(20)]
        [SerializeField] private Button _minimapButton;
        [SerializeField] private Button _minimapCloseButton;
        [SerializeField] private GameObject _minimap;
        [SerializeField] private Image _playerIcon;

        private Player _player;
        private ParticleSystem _abilityEffect;

        public event Action<AbilityView, ParticleSystem, Transform> AbilityViewCreated;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
            _player.PlayerHealth.HealthChanged -= OnChangeHealth;
            _player.PlayerStats.ExperienceValueChanged -= OnChangeExperience;
            _player.PlayerStats.UpgradeExperienceValueChanged -= OnChangeUpgradeExperience;
            _player.PlayerAbilityCaster.AbilityTaked -= OnAbilityTaked;
            _player.PlayerStats.KillCountChanged -= OnChangeKillCount;

            _minimapButton.onClick.RemoveListener(OnMinimapButtonClick);
            _minimapCloseButton.onClick.RemoveListener(OnMinimapButtonClick);
        }

        public void Initialize(Player player, Sprite iconPlayer)
        {
            _player = player;
            _playerIcon.sprite = iconPlayer;
            SubscribePlayerEvent();
            _sliderHP.maxValue = _player.PlayerHealth.MaxHealth;
            _sliderHP.value = _player.PlayerHealth.CurrentHealth;
            _sliderXP.maxValue = _player.PlayerStats.MaxLevelValue;
            _sliderXP.value = _player.PlayerStats.CurrentExperience;
            _sliderUpgradePoints.maxValue = _player.PlayerStats.MaxUpgradeValue;
            _sliderUpgradePoints.value = _player.PlayerStats.UpgradeExperience;
            _textPlayerLevel.text = _player.PlayerStats.CurrentLevel.ToString();
            _textUpgradePoints.text = _player.PlayerStats.UpgradePoints.ToString();
            _killCount.text = _player.PlayerStats.CountKillEnemy.ToString();
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

        private void SubscribePlayerEvent()
        {
            _player.PlayerHealth.HealthChanged += OnChangeHealth;
            _player.PlayerStats.ExperienceValueChanged += OnChangeExperience;
            _player.PlayerStats.UpgradeExperienceValueChanged += OnChangeUpgradeExperience;
            _player.PlayerAbilityCaster.AbilityTaked += OnAbilityTaked;
            _player.PlayerStats.KillCountChanged += OnChangeKillCount;

            _minimapButton.onClick.AddListener(OnMinimapButtonClick);
            _minimapCloseButton.onClick.AddListener(OnMinimapButtonClick);
        }

        private void OnMinimapButtonClick()
        {
            bool isActive = _minimap.activeSelf;
            _minimap.SetActive(!isActive);
        }

        private void OnAbilityTaked(AbilityAttributeData abilityAttributeData, int currentLevel)
        {
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