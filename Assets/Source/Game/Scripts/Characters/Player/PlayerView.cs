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

        private void Awake()
        {
            //_player.PlayerStats.PlayerHealth.HealthChanged += OnChangeHealth;
            //_player.PlayerStats.ExperienceValueChanged += OnChangeExperience;
            //_player.PlayerStats.UpgradeExperienceValueChanged += OnChangeUpgradeExperience;
            //_player.PlayerStats.PlayerAbilityCaster.AbilityTaked += OnAbilityTaked;
            //_player.PlayerStats.PlayerAbilityCaster.AbilityRemoved += OnAbilityRemoved;
            //_player.PlayerStats.PlayerAbilityCaster.AbilityEnded += OnAbilityEnded;
            //_player.PlayerStats.PlayerAbilityCaster.AbilityUsed += OnAbilityUsed;
            //_player.PlayerStats.KillCountChanged += OnChangeKillCount;
        }

        private void OnDestroy()
        {
            if(_player != null)
            {
                _player.PlayerStats.PlayerHealth.HealthChanged -= OnChangeHealth;
                _player.PlayerStats.ExperienceValueChanged -= OnChangeExperience;
                _player.PlayerStats.UpgradeExperienceValueChanged -= OnChangeUpgradeExperience;
                _player.PlayerStats.PlayerAbilityCaster.AbilityTaked -= OnAbilityTaked;
                _player.PlayerStats.PlayerAbilityCaster.AbilityRemoved -= OnAbilityRemoved;
                _player.PlayerStats.PlayerAbilityCaster.AbilityEnded -= OnAbilityEnded;
                _player.PlayerStats.PlayerAbilityCaster.AbilityUsed -= OnAbilityEnded;
                _player.PlayerStats.KillCountChanged -= OnChangeKillCount;
            }
        }

        public void Initialize(Player player, int maxLevelValue, int levelExperience, int maxUpgradeValue, int upgradeExperience, int currentLevel, int currentUpgradePoints)
        {
            _player = player;
            SubscribePlayerEvent();
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

        private void SubscribePlayerEvent()
        {
            _player.PlayerStats.PlayerHealth.HealthChanged += OnChangeHealth;
            _player.PlayerStats.ExperienceValueChanged += OnChangeExperience;
            _player.PlayerStats.UpgradeExperienceValueChanged += OnChangeUpgradeExperience;
            _player.PlayerStats.PlayerAbilityCaster.AbilityTaked += OnAbilityTaked;
            _player.PlayerStats.PlayerAbilityCaster.AbilityRemoved += OnAbilityRemoved;
            _player.PlayerStats.PlayerAbilityCaster.AbilityEnded += OnAbilityEnded;
            _player.PlayerStats.PlayerAbilityCaster.AbilityUsed += OnAbilityUsed;
            _player.PlayerStats.KillCountChanged += OnChangeKillCount;
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

            Ability ability = Instantiate(abilityAttributeData.Ability, _abilityObjectContainer);
            ability.Initialize(_player, _throwPoint, abilityAttributeData, currentLevel, _abilityEffect);
            _player.PlayerStats.PlayerAbilityCaster.AddAbility(ability);
        }

        private void OnAbilityRemoved(Ability ability)
        {
            if (ability != null)
                Destroy(ability.gameObject);

            if (ability.TypeAbility != TypeAbility.AttackAbility)
            {
                if (ability.ParticleSystem != null)
                    Destroy(ability.ParticleSystem.gameObject);
            }
        }

        private void OnAbilityUsed(Ability ability)
        {
            ability.ParticleSystem.Play();
        }

        private void OnAbilityEnded(Ability ability)
        {
            ability.ParticleSystem.Stop();
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