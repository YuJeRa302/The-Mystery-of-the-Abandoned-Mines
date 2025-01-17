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

        private PlayerAbilityCaster _playerAbilityCaster;
        private PlayerStats _playerStats;
        private CardDeck _cardDeck;
        private PlayerWeapons _playerWeapons;
        private PlayerAttacker _playerAttacker;
        private PlayerAnimation _playerAnimation;
        private PlayerHealth _playerHealth;
        private PlayerMovement _playerMovment;

        public Transform WeaponAbilityContainer => _weaponAbilityContainer;
        public Transform PlayerAbilityContainer => _playerAbilityContainer;
        public Transform ThrowAbilityPoint => _throwAbilityPoint;
        public CardDeck CardDeck => _cardDeck;
        public PlayerAttacker PlayerAttacker => _playerAttacker;
        public PlayerStats PlayerStats => _playerStats;
        public Transform WeaponPoint => _weaponPoint;
        public Transform AdditionalWeaponPoint => _additionalWeaponPoint;
        public PlayerWeapons PlayerWeapons => _playerWeapons;
        public PlayerAnimation PlayerAnimation => _playerAnimation;
        public PlayerHealth PlayerHealth => _playerHealth;
        public PlayerAbilityCaster PlayerAbilityCaster => _playerAbilityCaster;

        private void OnDestroy()
        {
            _playerAttacker.Attacked -= OnAttack;
            _playerAttacker.EnemyFinded -= OnRotateToTarget;
            _cardDeck.SetNewAbility -= OnSetNewAbility;
            _cardDeck.RerollPointsUpdated -= OnUpdateRerollPoints;
            _cardDeck.PlayerStatsUpdated -= OnStatsUpdate;
            _playerStats.MaxHealthChanged -= OnMaxHealthChanged;
            _playerStats.RegenerationChanged -= OnRegenerationChanged;
            _playerStats.ArmorChanged -= OnArmorChenge;
            _playerAbilityCaster.AbilityUsed -= OnAbilityUsed;
            _playerAbilityCaster.AbilityEnded -= OnAbilityEnded;

            DisposeStats();
        }

        public void CreateStats(LevelObserver levelObserver, PlayerClassData playerClassData, WeaponData weaponData, AbilityFactory abilityFactory, AbilityPresenterFactory abilityPresenter)
        {
            _miniMapIcon.sprite = playerClassData.Icon;
            _playerHealth = new PlayerHealth(levelObserver, this,this);
            _playerAnimation = new PlayerAnimation(_animator, _rigidbody, 2, playerClassData, this);
            _playerAttacker = new PlayerAttacker(_shotPoint, this, weaponData, this, _poolBullet);
            _playerWeapons = new PlayerWeapons(this, weaponData, _poolBullet);
            _playerMovment = new PlayerMovement(levelObserver.CameraControiler.Camera, levelObserver.CameraControiler.VariableJoystick, _rigidbody, 2f, this);
            _cardDeck = new CardDeck();
            _playerStats = new PlayerStats(this, 1, null, levelObserver, abilityFactory, abilityPresenter);
            _playerAbilityCaster = new PlayerAbilityCaster(abilityFactory,abilityPresenter,this, levelObserver.PlayerView);

            SubscribeAction();
        }

        private void SubscribeAction()
        {
            _playerAttacker.Attacked += OnAttack;
            _playerAttacker.EnemyFinded += OnRotateToTarget;

            _cardDeck.SetNewAbility += OnSetNewAbility;
            _cardDeck.RerollPointsUpdated += OnUpdateRerollPoints;
            _cardDeck.PlayerStatsUpdated += OnStatsUpdate;

            _playerStats.MaxHealthChanged += OnMaxHealthChanged;
            _playerStats.RegenerationChanged += OnRegenerationChanged;
            _playerStats.ArmorChanged += OnArmorChenge;

            _playerStats.AbilityDurationChanged += OnAbilityDurationChange;
            _playerStats.AbilityDamageChanged += OnAbilityDamageChanged;
            _playerStats.AbilityCooldownReductionChanged += OnAbilityCooldownReductionChanged;

            _playerAbilityCaster.AbilityUsed += OnAbilityUsed;
            _playerAbilityCaster.AbilityEnded += OnAbilityEnded;
        }

        private void OnRotateToTarget(Transform transform)
        {
            _playerMovment.ChengeRotate();
            _playerMovment.LookAtEnemy(transform);
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

        private void OnMaxHealthChanged(int value)
        {
            _playerHealth.MaxHealthChanged(value);
        }

        private void OnRegenerationChanged(int regeneration)
        {
            _playerHealth.ChangeRegeniration(regeneration);
        }

        private void OnArmorChenge(int armor)
        {
            _playerHealth.ChangeArmor(armor);
        }

        private void OnSetNewAbility(CardView cardView)
        {
            _playerAbilityCaster.TakeAbility(cardView);
            _playerStats.SetNewAbility(cardView);
        }

        private void OnUpdateRerollPoints(CardView cardView)
        {
            _playerStats.UpdateRerollPoints(cardView);
        }

        private void OnStatsUpdate(CardView cardView)
        {
            _playerStats.UpdatePlayerStats(cardView);
        }

        private void TryAttackEnemy()
        {
            _playerAttacker.AttackEnemy();
            _playerWeapons.ChangeTrailEffect();
        }

        private void AttackEnd()
        {
            _playerMovment.ChengeRotate();
        }

        private void OnAttack()
        {
            _playerAnimation.AttackAnimation();
        }

        private void DisposeStats()
        {
            _playerHealth.Dispose();
            _playerAnimation.Dispose();
            _playerAttacker.Dispose();
            _playerWeapons.Dispose();
            _playerMovment.Dispose();
            _playerStats.Dispose();
        }
    }
}