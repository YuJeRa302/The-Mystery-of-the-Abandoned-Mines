using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Player : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _shotPoint;
        [SerializeField] private Transform _weaponPoint;
        [SerializeField] private Transform _additionalWeaponPoint;
        [SerializeField] private Pool _poolBullet;
        [SerializeField] private SpriteRenderer _miniMapIcon;
        [SerializeField] private Transform _throwAbilityPoint;
        [SerializeField] private Transform _playerAbilityContainer;
        [SerializeField] private Transform _weaponAbilityContainer;
        [SerializeField] private PoolParticle _critDamageParticle;
        [SerializeField] private PoolParticle _vampirismParticle;
        [Header("[Default Player Parameters]")]
        [SerializeField] private int _currentLevel = 1;
        [SerializeField] private int _currentUpgradeLevel = 0;
        [SerializeField] private int _currentUpgradePoints = 0;
        [SerializeField] private int _currentExperience = 0;
        [SerializeField] private int _currentUpgradeExperience = 0;
        [SerializeField] private int _rerollPoints = 2;
        [SerializeField] private int _score = 0;
        [SerializeField] private int _armor = 2;
        [SerializeField] private int _regeneration = 1;
        [SerializeField] private int _countKillEnemy = 0;
        [SerializeField] private float _moveSpeed = 1.5f;
        [SerializeField] private int _currentHealth = 100;

        private PlayerView _playerView;
        private PlayerAbilityCaster _playerAbilityCaster;
        private PlayerStats _playerStats;
        private CardDeck _cardDeck;
        private PlayerWeapons _playerWeapons;
        private PlayerAttacker _playerAttacker;
        private PlayerAnimation _playerAnimation;
        private PlayerHealth _playerHealth;
        private PlayerWallet _wallet;
        private PlayerMovement _playerMovement;
        private AudioPlayer _audioPlayer;
        private Transform _spawnPoint;

        public event Action PlayerLevelChanged;
        public event Action PlayerDied;

        public Pool Pool => _poolBullet;
        public Transform PlayerAbilityContainer => _playerAbilityContainer;
        public Transform ThrowAbilityPoint => _throwAbilityPoint;
        public Transform WeaponPoint => _weaponPoint;
        public Transform AdditionalWeaponPoint => _additionalWeaponPoint;
        public Transform ShotPoint => _shotPoint;
        public PlayerAnimation PlayerAnimation => _playerAnimation;
        public WeaponData WeaponData => _playerWeapons.GetWeaponData();
        public DamageSource DamageSource => _playerStats.DamageSource;
        public int CurrentHealth => _playerHealth.GetCurrentHealth();
        public int Regeneration => _playerStats.Regeneration;
        public float MoveSpeed => _playerStats.MoveSpeed;
        public float MaxMoveSpeed => _playerStats.MaxMoveSpeed;
        public int Armor => _playerStats.Armor;
        public int Coins => _wallet.CurrentCoins;
        public int UpgradePoints => _playerStats.UpgradePoints;
        public int Score => _playerStats.Score;
        public int RerollPoints => _playerStats.RerollPoints;
        public int KillCount => _playerStats.CountKillEnemy;
        public float ChanceCriticalDamage => _playerStats.ChanceCriticalDamage;
        public float CriticalDamageMultiplier => _playerStats.CriticalDamageMultiplier;
        public float ChanceVampirism => _playerStats.ChanceVampirism;
        public float VampirismValue => _playerStats.VampirismValue;
        public float SearchRadius => _playerStats.SearchRadius;
        public float AttackRange => _playerStats.AttackRange;

        public void CreatePlayerEntities(
            AbilityFactory abilityFactory,
            AbilityPresenterFactory abilityPresenterFactory,
            PersistentDataService persistentDataService,
            GamePauseService gamePauseService,
            GameConfig gameConfig,
            CameraControiler cameraControiler,
            PlayerView playerView,
            PlayerClassData playerClassData,
            Transform spawnPoint,
            AudioPlayer audioPlayer) 
        {
            _spawnPoint = spawnPoint;
            _playerView = playerView;
            _audioPlayer = audioPlayer;
            _miniMapIcon.sprite = playerClassData.Icon;
            _playerHealth = new PlayerHealth(this, this, gamePauseService, _currentHealth);
            _playerAnimation = new PlayerAnimation(_animator, _rigidbody, _moveSpeed, playerClassData, this, this, gamePauseService);

            _playerWeapons = new PlayerWeapons(
                this,
                gameConfig.GetWeaponDataById(persistentDataService.PlayerProgress.WeaponService.CurrentWeaponId),
                _poolBullet,
                _critDamageParticle,
                _vampirismParticle);

            _playerStats = new PlayerStats(
                gameConfig.GetWeaponDataById(persistentDataService.PlayerProgress.WeaponService.CurrentWeaponId),
                playerClassData,
                _currentLevel,
                _rerollPoints,
                _armor,
                _moveSpeed,
                _regeneration,
                _countKillEnemy);

            _playerMovement = new PlayerMovement(
                cameraControiler.Camera,
                cameraControiler.VariableJoystick,
                _rigidbody,
                this,
                this,
                gamePauseService);

            _playerAttacker = new PlayerAttacker(
                _shotPoint,
                this,
                playerClassData.TypeAttackRange,
                gameConfig.GetWeaponDataById(persistentDataService.PlayerProgress.WeaponService.CurrentWeaponId),
                this,
                gamePauseService,
                _poolBullet);

            _wallet = new PlayerWallet();
            _cardDeck = new CardDeck(gameConfig.GetLevelData(persistentDataService.PlayerProgress.LevelService.CurrentLevelId).IsContractLevel);
            _playerAbilityCaster = new PlayerAbilityCaster(abilityFactory, abilityPresenterFactory, this, persistentDataService, playerClassData, _audioPlayer);
            _playerView.Initialize(playerClassData.Icon, _throwAbilityPoint);
            SetPlayerStats();
            AddListeners();
            _playerStats.SetPlayerUpgrades(gameConfig, persistentDataService);
            _playerAbilityCaster.Initialize();
        }

        public void Remove()
        {
            RemoveListeners();
            ReleaseUnmanagedResources();
            Destroy(this);
        }

        public void InitStateCardDeck(List<CardData> cardDatas)
        {
            foreach (var data in cardDatas)
            {
                _cardDeck.InitState(data);
            }
        }

        public void UpdateDeck()
        {
            _cardDeck.UpdateDeck();
        }

        public void TakeCard(CardView cardView)
        {
            _cardDeck.TakeCard(cardView);
        }

        public void ResetPosition() 
        {
            transform.position = _spawnPoint.transform.position;
        }

        public void ResetCardState()
        {
            _cardDeck.ResetCardState();
        }

        public void UpdateCardPanelByRerollPoints()
        {
            _playerStats.UpdateCardPanelByRerollPoints();
        }

        public CardState GetCardStateByData(CardData cardData)
        {
            return _cardDeck.GetCardStateByData(cardData);
        }

        public bool TryTakeAbilityCard(int id)
        {
            return _cardDeck.CanTakeAbilityCard(id);
        }

        public bool TryGetRerollPoints()
        {
            return _playerStats.TryGetRerollPoints();
        }

        public int GetMinWeightCards()
        {
            return _cardDeck.GetMinWeightCards();
        }

        public int GetCountUnlockedCards()
        {
            return _cardDeck.GetCountUnlockedCards();
        }

        public void GetRerollPointsReward(int reward)
        {
            _playerStats.GetReward(reward);
        }

        public void GetEndGameReward()
        {
            _wallet.AddCoins(Coins);
            _playerStats.GetReward(UpgradePoints);
        }

        public void TakeDamage(int value)
        {
            _playerHealth.TakeDamage(value);
        }

        public void GetReward(Enemy enemy)
        {
            _playerStats.EnemyDied(enemy);
        }

        public void GetLootRoomReward(int reward)
        {
            _playerStats.TakeLootRoomReward(reward);
        }

        private void AddListeners()
        {
            _playerAttacker.Attacked += OnAttack;
            _playerAttacker.EnemyFinded += OnRotateToTarget;
            _playerAttacker.CritAttacked += OnApplayCritDamage;
            _playerAttacker.HealedVampirism += OnHealingVampirism;
            _cardDeck.SetNewAbility += OnSetNewAbility;
            _cardDeck.RerollPointsUpdated += OnUpdateRerollPoints;
            _cardDeck.PlayerStatsUpdated += OnStatsUpdate;
            _cardDeck.TakedPassivAbility += OnTakedPassivAbility;
            _playerStats.ExperienceValueChanged += OnExperienceValueChanged;
            _playerStats.UpgradeExperienceValueChanged += OnUpgradeExperienceValueChanged;
            _playerStats.MaxHealthChanged += OnMaxHealthChanged;
            _playerStats.HealthUpgradeApplied += OnHealthUpgradeApplied;
            _playerStats.Healed += OnHealing;
            _playerStats.HealthReduced += OnReduceHealth;
            _playerStats.AbilityDurationChanged += OnAbilityDurationChange;
            _playerStats.AbilityDamageChanged += OnAbilityDamageChanged;
            _playerStats.AbilityCooldownReductionChanged += OnAbilityCooldownReductionChanged;
            _playerStats.KillCountChanged += OnKillCountChanged;
            _playerStats.PlayerLevelChanged += OnPlayerLevelChanged;
            _playerStats.PlayerUpgradeLevelChanged += OnPlayerUpgradeLevelChanged;
            _playerAbilityCaster.AbilityUsed += OnAbilityUsed;
            _playerAbilityCaster.AbilityEnded += OnAbilityEnded;
            _playerAbilityCaster.AbilityTaked += OnAbilityTaked;
            _playerAbilityCaster.PassiveAbilityTaked += OnPassiveAbilityTaked;
            _playerAbilityCaster.LegendaryAbilityTaked += OnLegendaryAbilityTaked;
            _playerAbilityCaster.ClassAbilityTaked += OnClassAbilityTaked;
            _playerHealth.HealthChanged += OnHealthChanged;
            _playerHealth.PlayerDied += OnPlayerDead;
            _playerView.AbilityViewCreated += OnAbilityViewCreated;
            _playerView.PassiveAbilityViewCreated += OnPassiveAbilityViewCreated;
            _playerView.LegendaryAbilityViewCreated += OnLegendaryAbilityViewCreated;
            _playerView.ClassAbilityViewCreated += OnClassAbilityViewCreated;
            _playerStats.CoinsAdding += OnAddCoins;
        }

        private void RemoveListeners()
        {
            _playerAttacker.Attacked -= OnAttack;
            _playerAttacker.EnemyFinded -= OnRotateToTarget;
            _playerAttacker.CritAttacked -= OnApplayCritDamage;
            _playerAttacker.HealedVampirism -= OnHealingVampirism;
            _cardDeck.SetNewAbility -= OnSetNewAbility;
            _cardDeck.RerollPointsUpdated -= OnUpdateRerollPoints;
            _cardDeck.PlayerStatsUpdated -= OnStatsUpdate;
            _cardDeck.TakedPassivAbility -= OnTakedPassivAbility;
            _playerStats.ExperienceValueChanged -= OnExperienceValueChanged;
            _playerStats.UpgradeExperienceValueChanged -= OnUpgradeExperienceValueChanged;
            _playerStats.MaxHealthChanged -= OnMaxHealthChanged;
            _playerStats.HealthUpgradeApplied -= OnHealthUpgradeApplied;
            _playerStats.Healed -= OnHealing;
            _playerStats.HealthReduced -= OnReduceHealth;
            _playerStats.CoinsAdding -= OnAddCoins;
            _playerStats.AbilityDurationChanged -= OnAbilityDurationChange;
            _playerStats.AbilityDamageChanged -= OnAbilityDamageChanged;
            _playerStats.AbilityCooldownReductionChanged -= OnAbilityCooldownReductionChanged;
            _playerStats.KillCountChanged -= OnKillCountChanged;
            _playerStats.PlayerLevelChanged -= OnPlayerLevelChanged;
            _playerStats.PlayerUpgradeLevelChanged -= OnPlayerUpgradeLevelChanged;
            _playerAbilityCaster.AbilityUsed -= OnAbilityUsed;
            _playerAbilityCaster.AbilityEnded -= OnAbilityEnded;
            _playerAbilityCaster.AbilityTaked -= OnAbilityTaked;
            _playerAbilityCaster.PassiveAbilityTaked -= OnPassiveAbilityTaked;
            _playerAbilityCaster.LegendaryAbilityTaked -= OnLegendaryAbilityTaked;
            _playerAbilityCaster.ClassAbilityTaked -= OnClassAbilityTaked;
            _playerHealth.HealthChanged -= OnHealthChanged;
            _playerHealth.PlayerDied -= OnPlayerDead;
            _playerView.AbilityViewCreated -= OnAbilityViewCreated;
            _playerView.PassiveAbilityViewCreated -= OnPassiveAbilityViewCreated;
            _playerView.LegendaryAbilityViewCreated -= OnLegendaryAbilityViewCreated;
            _playerView.ClassAbilityViewCreated -= OnClassAbilityViewCreated;
        }

        private void SetPlayerStats()
        {
            _playerView.ChangeUpgradeLevel(_currentUpgradeLevel, _playerStats.GetMaxUpgradeExperienceValue(_currentUpgradeLevel), _currentUpgradeExperience);
            _playerView.ChangePlayerLevel(_currentLevel, _playerStats.GetMaxExperienceValue(_currentLevel), _currentExperience);
            _playerView.ChangeKillCount(_countKillEnemy);
            _playerView.ChangeMaxHealthValue(_playerHealth.GetMaxHealth(), _playerHealth.GetCurrentHealth());
        }

        public void OnPlayerDead()
        {
            _playerHealth.DisableHealing();
            _playerMovement.DisableMovment();
            PlayerDied?.Invoke();
        }

        private void OnClassAbilityViewCreated(ClassAbilityData classAbilityData, ClassSkillButtonView classSkillButtonView, int currentLevel)
        {
            _playerAbilityCaster.CreateClassAbilityView(classAbilityData, classSkillButtonView, currentLevel);
        }

        private void OnLegendaryAbilityViewCreated(AbilityView abilityView, ParticleSystem particleSystem, Transform throwPoint, ActiveAbilityData abilityAttributeData)
        {
            _playerAbilityCaster.CreateLegendaryAbilityView(abilityView, particleSystem, throwPoint, abilityAttributeData);
        }

        private void OnPassiveAbilityViewCreated(PassiveAbilityView passiveAbilityView)
        {
            _playerAbilityCaster.CreatePassiveAbilityView(passiveAbilityView);
        }

        private void OnAbilityViewCreated(AbilityView abilityView, ParticleSystem particleSystem, Transform throwPoint)
        {
            _playerAbilityCaster.CreateAbilityView(abilityView, particleSystem, throwPoint);
        }

        private void OnClassAbilityTaked(ClassAbilityData abilityData, int currentLevel)
        {
            _playerView.TakeClassAbility(abilityData, currentLevel);
        }

        private void OnLegendaryAbilityTaked(ActiveAbilityData abilityAttributeData)
        {
            _playerView.TakeLegendaryAbility(abilityAttributeData);
        }

        private void OnPassiveAbilityTaked(PassiveAttributeData passiveAttributeData)
        {
            _playerView.TakePassiveAbility(passiveAttributeData);
        }

        private void OnAbilityTaked(ActiveAbilityData abilityAttributeData, int currentLevel)
        {
            _playerView.TakeAbility(abilityAttributeData, currentLevel);
        }

        private void OnPlayerUpgradeLevelChanged(int currenLevel, int maxExperienceValue, int currentExperience)
        {
            _playerView.ChangeUpgradeLevel(currenLevel, maxExperienceValue, currentExperience);
        }

        private void OnPlayerLevelChanged(int currenLevel, int maxExperienceValue, int currentExperience)
        {
            _playerView.ChangePlayerLevel(currenLevel, maxExperienceValue, currentExperience);
            PlayerLevelChanged?.Invoke();
        }

        private void OnKillCountChanged(int value)
        {
            _playerView.ChangeKillCount(value);
        }

        private void OnUpgradeExperienceValueChanged(int value)
        {
            _playerView.ChangeUpgradeExperience(value);
        }

        private void OnExperienceValueChanged(int value)
        {
            _playerView.ChangeExperience(value);
        }

        private void OnHealthChanged(int value)
        {
            _playerView.ChangeHealth(value);
        }

        private void OnAddCoins(int count)
        {
            _wallet.AddCoins(count);
        }

        private void OnHealingVampirism(float healing)
        {
            _playerHealth.TakeHealing(Convert.ToInt32(healing));
            _playerWeapons.InstantiateHealingEffect();
        }

        private void OnApplayCritDamage()
        {
            _playerWeapons.InstantiateCritEffect();
        }

        private void OnReduceHealth(float reduce)
        {
            _playerHealth.ReduceHealth(reduce);
        }

        private void OnHealing(int heal)
        {
            _playerHealth.TakeHealing(heal);
        }

        private void OnRotateToTarget(Transform transform)
        {
            _playerMovement.ChangeRotate(false);
            _playerMovement.LookAtEnemy(transform);
        }

        private void OnAbilityCooldownReductionChanged(int value)
        {
            _playerAbilityCaster.AbilityCooldownReductionChanged(value);
        }

        private void OnAbilityDamageChanged(int value)
        {
            _playerAbilityCaster.AbilityDamageChanged(value);
        }

        private void OnAbilityDurationChange(int value)
        {
            _playerAbilityCaster.AbilityDurationChanged(value);
        }

        private void OnAbilityEnded(Ability ability)
        {
            _playerStats.AbilityEnded(ability);
        }

        private void OnAbilityUsed(Ability ability)
        {
            _playerStats.AbilityUsed(ability);
        }

        private void OnMaxHealthChanged(int healthValue)
        {
            _playerHealth.ChangeMaxHealth(healthValue, out int currentHealthValue, out int maxHealth);
            _playerView.ChangeMaxHealthValue(maxHealth, currentHealthValue);
        }

        private void OnHealthUpgradeApplied(int healthValue)
        {
            _playerHealth.ApplyHealthUpgrade(healthValue, out int currentHealthValue, out int maxHealth);
            _playerView.ChangeMaxHealthValue(maxHealth, currentHealthValue);
        }

        private void OnSetNewAbility(CardView cardView)
        {
            _playerAbilityCaster.TakeAbility(cardView);
            cardView.CardState.CurrentLevel++;
            cardView.CardState.Weight++;
        }

        private void OnUpdateRerollPoints(CardView cardView)
        {
            _playerStats.UpdateRerollPoints(cardView);
            cardView.CardState.Weight++;
        }

        private void OnTakedPassivAbility(CardView cardView)
        {
            _playerStats.UpdatePlayerStats(cardView);
            _playerAbilityCaster.TakeAbility(cardView);
            cardView.CardState.CurrentLevel++;
            cardView.CardState.Weight++;
            cardView.CardState.IsLocked = true;
            cardView.CardState.IsCardUpgraded = true;
        }

        private void OnStatsUpdate(CardView cardView)
        {
            _playerStats.UpdatePlayerStats(cardView);
            cardView.CardState.Weight++;
        }

        private void TryAttackEnemy()
        {
            _playerAttacker.AttackEnemy();
            _playerWeapons.ChangeTrailEffect();
            _audioPlayer.PlayCharesterAudio(_playerStats.PlayerClassData.AttackEffect);
        }

        private void AttackEnd()
        {
            _playerMovement.ChangeRotate(true);
        }

        private void OnAttack()
        {
            _playerAnimation.AttackAnimation();
        }

        private void ReleaseUnmanagedResources()
        {
            _playerHealth.Dispose();
            _playerAnimation.Dispose();
            _playerAttacker.Dispose();
            _playerAbilityCaster.Dispose();
            _playerWeapons.Dispose();
            _playerMovement.Dispose();
            _playerStats.Dispose();
            _wallet.Dispose();
        }
    }
}