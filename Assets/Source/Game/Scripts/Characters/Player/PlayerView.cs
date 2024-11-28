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
        [SerializeField] private Player _player;
        [SerializeField] private Text _textPlayerLevel;
        [SerializeField] private Text _textUpgradePoints;
        [SerializeField] private Text _killCount;
        [Space(20)]
        [SerializeField] private Transform _abilityObjectContainer;
        [SerializeField] private Transform _playerEffectsContainer;
        [SerializeField] private Transform _weaponEffectsContainer;
        [SerializeField] private Transform _throwPoint;

        private ParticleSystem _abilityEffect;

        public event Action<AbilityView, ParticleSystem, Transform> AbilityViewCreated;

        private void Awake()
        {
            _player.PlayerStats.PlayerHealth.HealthChanged += OnChangeHealth;
            _player.PlayerStats.ExperienceValueChanged += OnChangeExperience;
            _player.PlayerStats.UpgradeExperienceValueChanged += OnChangeUpgradeExperience;
            _player.PlayerStats.PlayerAbilityCaster.AbilityTaked += OnAbilityTaked;
            _player.PlayerStats.KillCountChanged += OnChangeKillCount;
        }

        private void OnDestroy()
        {
            _player.PlayerStats.PlayerHealth.HealthChanged -= OnChangeHealth;
            _player.PlayerStats.ExperienceValueChanged -= OnChangeExperience;
            _player.PlayerStats.UpgradeExperienceValueChanged -= OnChangeUpgradeExperience;
            _player.PlayerStats.PlayerAbilityCaster.AbilityTaked -= OnAbilityTaked;
            _player.PlayerStats.KillCountChanged -= OnChangeKillCount;
        }

        public void Initialize(int maxLevelValue, int levelExperience, int maxUpgradeValue, int upgradeExperience, int currentLevel, int currentUpgradePoints)
        {
            _sliderHP.maxValue = _player.PlayerStats.PlayerHealth.MaxHealth;
            _sliderHP.value = _player.PlayerStats.PlayerHealth.CurrentHealth;
            _sliderXP.maxValue = maxLevelValue;
            _sliderXP.value = levelExperience;
            _sliderUpgradePoints.maxValue = maxUpgradeValue;
            _sliderUpgradePoints.value = upgradeExperience;
            _textPlayerLevel.text = currentLevel.ToString();
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