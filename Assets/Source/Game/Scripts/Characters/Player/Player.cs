using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.Factories;
using Assets.Source.Game.Scripts.Items;
using Assets.Source.Game.Scripts.Menu;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
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
        [SerializeField] private int _currentExperience = 0;
        [SerializeField] private int _currentUpgradeExperience = 0;
        [SerializeField] private int _rerollPoints = 2;
        [SerializeField] private int _armor = 2;
        [SerializeField] private int _regeneration = 1;
        [SerializeField] private int _countKillEnemy = 0;
        [SerializeField] private float _moveSpeed = 1.5f;
        [SerializeField] private int _currentHealth = 100;
        [SerializeReference] private List<ITakeCardStrategy> _takeCardStrategies;

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

        public Pool Pool => _poolBullet;
        public Transform PlayerAbilityContainer => _playerAbilityContainer;
        public Transform ThrowAbilityPoint => _throwAbilityPoint;
        public Transform WeaponPoint => _weaponPoint;
        public Transform AdditionalWeaponPoint => _additionalWeaponPoint;
        public Transform ShotPoint => _shotPoint;
        public PlayerAnimation PlayerAnimation => _playerAnimation;
        public WeaponData WeaponData => _playerWeapons.GetWeaponData();
        public DamageSource DamageSource => _playerStats.DamageSource;
        public CardDeck CardDeck => _cardDeck;
        public PlayerAbilityCaster PlayerAbilityCaster => _playerAbilityCaster;
        public PlayerStats PlayerStats => _playerStats;
        public int CurrentHealth => _playerHealth.GetCurrentHealth();
        public int Coins => _wallet.CurrentCoins;

        public void CreatePlayerEntities(
            AbilityFactory abilityFactory,
            PersistentDataService persistentDataService,
            GamePauseService gamePauseService,
            GameConfig gameConfig,
            CameraScripts.CameraController cameraController,
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

            _playerAnimation = new PlayerAnimation(
                _animator,
                _rigidbody,
                _moveSpeed, playerClassData,
                this,
                this,
                gamePauseService);

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
                cameraController.Camera,
                cameraController.VariableJoystick,
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

            _cardDeck = new CardDeck(
                gameConfig.GetLevelData(
                    persistentDataService.PlayerProgress.LevelService.CurrentLevelId).IsContractLevel,
                _takeCardStrategies);

            _playerAbilityCaster = new PlayerAbilityCaster(
                abilityFactory,
                this,
                persistentDataService,
                playerClassData,
                _audioPlayer);

            _playerView.Initialize(playerClassData.Icon, _playerHealth);
            _playerStats.SetPlayerUpgrades(gameConfig, persistentDataService);
        }

        public void Remove()
        {
            ReleaseUnmanagedResources();
            Destroy(this);
        }

        public void ResetPosition()
        {
            transform.position = _spawnPoint.transform.position;
        }

        public void TakeDamage(int value)
        {
            _playerHealth.TakeDamage(value);
        }

        private void TryAttackEnemy()
        {
            _playerAttacker.AttackEnemy();
            _playerWeapons.ChangeTrailEffect();
            _audioPlayer.PlayCharacterAudio(_playerStats.PlayerClassData.AttackEffect);
        }

        private void AttackEnd()
        {
            _playerMovement.ChangeRotate(true);
        }

        private void ReleaseUnmanagedResources()
        {
            _playerStats.Dispose();
            _playerHealth.Dispose();
            _playerAnimation.Dispose();
            _playerAttacker.Dispose();
            _playerAbilityCaster.Dispose();
            _playerMovement.Dispose();
            _wallet.Dispose();
        }
    }
}