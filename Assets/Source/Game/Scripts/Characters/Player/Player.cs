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

        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private CardDeck _cardDeck;

        private PlayerWeapons _playerWeapons;
        private PlayerAttacker _playerAttacker;
        private PlayerAnimation _playerAnimation;
        private PlayerHealth _playerHealth;
        private PlayerMovement _playerMovment;

        public CardDeck CardDeck => _cardDeck;
        public PlayerAttacker PlayerAttacker => _playerAttacker;
        public PlayerStats PlayerStats => _playerStats;
        public Transform WeaponPoint => _weaponPoint;
        public Transform AdditionalWeaponPoint => _additionalWeaponPoint;
        public PlayerWeapons PlayerWeapons => _playerWeapons;
        public PlayerAnimation PlayerAnimation => _playerAnimation;
        public PlayerHealth PlayerHealth => _playerHealth;

        private void OnDestroy()
        {
            _playerAttacker.Attacked -= OnAttack;
            DisposeStats();
        }

        public void CreateStats(LevelObserver levelObserver, PlayerClassData playerClassData, WeaponData weaponData)
        {
            _playerHealth = new PlayerHealth(levelObserver, this,this);
            _playerAnimation = new PlayerAnimation(_animator, _rigidbody, 2, playerClassData, this);
            _playerAttacker = new PlayerAttacker(_shotPoint, this, weaponData, this, _poolBullet);
            _playerWeapons = new PlayerWeapons(this, weaponData);
            _playerMovment = new PlayerMovement(levelObserver.CameraControiler.Camera, levelObserver.CameraControiler.VariableJoystick, _rigidbody, 1f, this);

            SubscribeAction();
        }

        private void SubscribeAction()
        {
            _playerAttacker.Attacked += OnAttack;
        }

        private void TryAttackEnemy()
        {
            _playerAttacker.AttackEnemy();
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
        }
    }
}