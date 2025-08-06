using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Game.Scripts.Views;
using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class Enemy : PoolObject
    {
        [SerializeField] private Pool _pool;
        [SerializeField] private EnemyStateMachineExample _stateMachine;
        [SerializeField] private EnemyAnimation _animationController;
        [SerializeField] private Transform _damageEffectContainer;
        [SerializeField] private HealthBarView _healthView;

        private int _damage;
        private float _attackDistance;
        private float _attackDelay;
        private float _speed;
        private float _baseMoveSpeed;
        private int _currentLvlRoom;
        private int _experience;
        private int _score;
        private int _gold;
        private int _upgradeExperience;
        private Rigidbody _rigidbody;
        private AudioClip _deathAudio;
        private AudioClip _hitAudio;
        private int _tier;
        private EnemyHealth _health;
        private EnemyDamageHandler _damageHandler;

        public float AttackDelay => _attackDelay;
        public float Damage => _damage;
        public float Speed => _speed;
        public float AttackDistance => _attackDistance;
        public int ExperienceReward => _experience;
        public int Score => _score;
        public int GoldReward => _gold;
        public int UpgradeExperienceReward => _upgradeExperience;
        public EnemyAnimation AnimationStateController => _animationController;
        public AudioClip DeathAudio => _deathAudio;
        public Pool EnemyPool => _pool;
        public EnemyHealth EnemyHealth => _health;
        public Rigidbody Rigidbody => _rigidbody;

        public event Action<Enemy> Died;
        public event Action EnemyStuned;
        public event Action EnemyStunEnded;
        public event Action<AudioClip> PlayerAttacked;

        private void OnDisable()
        {
            _damageHandler.Disable();
        }

        private void OnDestroy()
        {
            _animationController.Attacked -= OnEnemyAttack;
            _damageHandler.Stuned -= OnEnemyStaned;
            _damageHandler.StunEnded -= OnStanEnded; 
            _damageHandler.MoveSpeedReduced -= OnMoveSpeedReduced;
            _damageHandler.MoveSpeedReseted -= OnMoveSpeedReset;
        }

        public virtual void Initialize(Player player, int lvlRoom, EnemyData data, int tire)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _currentLvlRoom = lvlRoom;
            _tier = tire;
            Fill(data);
            _stateMachine.InitializeStateMachine(player);
            _healthView.Initialize(this);
            _animationController.Attacked += OnEnemyAttack;
        }

        public virtual void ResetEnemy(int lvlRoom)
        {
            _health.ResetHealth();

            if (_currentLvlRoom < lvlRoom)
            {
                int healthBoost = _health.MaxHealth * (1 + lvlRoom / GameConstants.EnemyBoostDivider);
                _health.ChengeMaxHealth(healthBoost);
                _damage = _damage * (1 + lvlRoom / GameConstants.EnemyBoostDivider);
            }

            _stateMachine.ResetState();
        }

        public void TakeDamage(DamageSource damageSource)
        {
            if (damageSource == null)
                return;

            _health.ApplyDamage(damageSource.Damage);

            if (_health.IsDead)
            {
                Died?.Invoke(this);
                ReturnToPool();
                return;
            }

            _damageHandler.CreateDamageEffect(damageSource);
        }

        protected override void ReturnToPool()
        {
            _damageHandler.Disable();
            base.ReturnToPool();
            _speed = _baseMoveSpeed;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _health.ResetHealth();
        }

        private void Fill(EnemyData data)
        {
            _health = new EnemyHealth(data.EnemyStats[_tier].Health);
            _damageHandler = new EnemyDamageHandler(_pool, _damageEffectContainer, this);
            _damageHandler.Stuned += OnEnemyStaned;
            _damageHandler.StunEnded += OnStanEnded;
            _damageHandler.MoveSpeedReduced += OnMoveSpeedReduced;
            _damageHandler.MoveSpeedReseted += OnMoveSpeedReset;

            _damage = data.EnemyStats[_tier].Damage;
            _speed = data.EnemyStats[_tier].MoveSpeed;
            _attackDelay = data.EnemyStats[_tier].AttackDelay;
            _attackDistance = data.EnemyStats[_tier].AttackDistance;
            _gold = data.EnemyStats[_tier].GoldReward;
            _score = data.EnemyStats[_tier].Score;
            _experience = data.EnemyStats[_tier].ExperienceReward;
            _upgradeExperience = data.EnemyStats[_tier].UpgradeExperienceReward;
            _baseMoveSpeed = _speed;
            _deathAudio = data.AudioClipDie;
            _hitAudio = data.Hit;
        }

        private void OnEnemyAttack()
        {
            PlayerAttacked?.Invoke(_hitAudio);
        }

        private void OnEnemyStaned()
        {
            EnemyStuned?.Invoke();
        }

        private void OnStanEnded()
        {
            EnemyStunEnded?.Invoke();
        }

        private void OnMoveSpeedReset()
        {
            _speed = _baseMoveSpeed;
        }

        private void OnMoveSpeedReduced(float valueSlowed) 
        {
            _baseMoveSpeed = _speed;
            _speed = _speed* (1 - valueSlowed / 100);}
        }
}