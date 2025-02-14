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

        private PlayerAbilityCaster _playerAbilityCaster;
        private PlayerStats _playerStats;
        private CardDeck _cardDeck;
        private PlayerWeapons _playerWeapons;
        private PlayerAttacker _playerAttacker;
        private PlayerAnimation _playerAnimation;
        private PlayerHealth _playerHealth;
        private PlayerMovement _playerMovment;

        public Pool Pool => _poolBullet;
        public Transform WeaponAbilityContainer => _weaponAbilityContainer;
        public Transform PlayerAbilityContainer => _playerAbilityContainer;
        public Transform ThrowAbilityPoint => _throwAbilityPoint;
        public CardDeck CardDeck => _cardDeck;
        public PlayerMovement PlayerMovment => _playerMovment;
        public PlayerAttacker PlayerAttacker => _playerAttacker;
        public PlayerStats PlayerStats => _playerStats;
        public Transform WeaponPoint => _weaponPoint;
        public Transform AdditionalWeaponPoint => _additionalWeaponPoint;
        public PlayerWeapons PlayerWeapons => _playerWeapons;
        public PlayerAnimation PlayerAnimation => _playerAnimation;
        public PlayerHealth PlayerHealth => _playerHealth;
        public PlayerAbilityCaster PlayerAbilityCaster => _playerAbilityCaster;
        public Transform ShotPoint => _shotPoint;

        private void OnDestroy()
        {
            _playerAttacker.Attacked -= OnAttack;
            _playerAttacker.EnemyFinded -= OnRotateToTarget;
            _playerAttacker.CritAttacked -= OnApplayCritDamage;
            _playerAttacker.HealedVampirism -= OnHealingVampirism;
            _cardDeck.SetNewAbility -= OnSetNewAbility;
            _cardDeck.RerollPointsUpdated -= OnUpdateRerollPoints;
            _cardDeck.PlayerStatsUpdated -= OnStatsUpdate;
            _playerStats.MaxHealthChanged -= OnMaxHealthChanged;
            _playerStats.RegenerationChanged -= OnRegenerationChanged;
            _playerStats.ArmorChanged -= OnArmorChenge;
            _playerAbilityCaster.AbilityUsed -= OnAbilityUsed;
            _playerAbilityCaster.AbilityEnded -= OnAbilityEnded;
            _playerStats.DamageChenged -= OnDamageChenged;
            _playerStats.MoveSpeedChanged -= OnMoveSpeedChanged;
            _playerStats.Healed -= OnHealing;
            _playerStats.HealthReduced -= OnReduceHealth;

            DisposeStats();
        }

        public void CreateStats(LevelObserver levelObserver, PlayerClassData playerClassData, WeaponData weaponData, AbilityFactory abilityFactory, 
            AbilityPresenterFactory abilityPresenter, TemporaryData temporaryData)
        {
            _miniMapIcon.sprite = playerClassData.Icon;
            _playerHealth = new PlayerHealth(levelObserver, this, weaponData, this);
            _playerAnimation = new PlayerAnimation(_animator, _rigidbody, 2, playerClassData, this);
            _playerAttacker = new PlayerAttacker(_shotPoint, this, weaponData, this, _poolBullet);
            _playerWeapons = new PlayerWeapons(this, weaponData, _poolBullet, _critDamageParticle, _vampirismParticle);
            _playerMovment = new PlayerMovement(levelObserver.CameraControiler.Camera, levelObserver.CameraControiler.VariableJoystick, _rigidbody, 2f, this);
            _cardDeck = new CardDeck();
            _playerStats = new PlayerStats(this, 1, null, levelObserver, abilityFactory, abilityPresenter);
            _playerAbilityCaster = new PlayerAbilityCaster(abilityFactory,abilityPresenter,this, levelObserver.PlayerView, temporaryData);

            SubscribeAction();
        }

        private void SubscribeAction()
        {
            _playerAttacker.Attacked += OnAttack;
            _playerAttacker.EnemyFinded += OnRotateToTarget;
            _playerAttacker.CritAttacked += OnApplayCritDamage;
            _playerAttacker.HealedVampirism += OnHealingVampirism;

            _cardDeck.SetNewAbility += OnSetNewAbility;
            _cardDeck.RerollPointsUpdated += OnUpdateRerollPoints;
            _cardDeck.PlayerStatsUpdated += OnStatsUpdate;

            _playerStats.MaxHealthChanged += OnMaxHealthChanged;
            _playerStats.RegenerationChanged += OnRegenerationChanged;
            _playerStats.ArmorChanged += OnArmorChenge;
            _playerStats.DamageChenged += OnDamageChenged;
            _playerStats.MoveSpeedChanged += OnMoveSpeedChanged;
            _playerStats.Healed += OnHealing;
            _playerStats.HealthReduced += OnReduceHealth;

            _playerStats.AbilityDurationChanged += OnAbilityDurationChange;
            _playerStats.AbilityDamageChanged += OnAbilityDamageChanged;
            _playerStats.AbilityCooldownReductionChanged += OnAbilityCooldownReductionChanged;

            _playerAbilityCaster.AbilityUsed += OnAbilityUsed;
            _playerAbilityCaster.AbilityEnded += OnAbilityEnded;
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

        private void OnMoveSpeedChanged(float value)
        {
            _playerMovment.ChangeMoveSpeed(value);
        }

        private void OnDamageChenged(int value)
        {
            _playerAttacker.ÑhangeDamage(value);
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