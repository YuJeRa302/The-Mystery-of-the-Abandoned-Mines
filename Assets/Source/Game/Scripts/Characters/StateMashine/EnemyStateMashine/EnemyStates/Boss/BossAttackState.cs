using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class BossAttackState : EnemyAttackState
    {
        private BulletSpawner _bulletSpawner;
        private float _additionalAttackDelay = 7f;
        private float _specialAttackDelay = 7f;
        protected float _lastAdditionalAttackTime = 8;
        protected float _lastSpecialAttackTime = 0;

        public BossAttackState(StateMashine stateMashine, Player target, Enemy enemy) : base(stateMashine, target, enemy)
        {
            Boss boss = enemy as Boss;
            _additionalAttackDelay = boss.AdditionalAttackDelay;
            _specialAttackDelay = boss.SpecilaAttackDelay;
        }

        public override void SubscrabeIvent()
        {
            if (_enemy.TryGetComponent(out Beholder beholder))
            {
                _bulletSpawner = new BulletSpawner(beholder.Bullet, beholder.Pool, beholder.BaseShotPoint, _enemy);
                _animationController.Attacked += LaunchBullet;
            }
            else
            {
                _animationController.Attacked += ApplyDamage;
            }

            _animationController.AnimationCompleted += OnAllowTransition;
        }

        public override void UpdateState()
        {
            if (_canTransit)
            {
                _directionToTarget = _enemy.transform.position - _target.transform.position;
                _distanceToTarget = _directionToTarget.magnitude;

                if (_distanceToTarget > _attackRange)
                    _stateMashine.SetState<EnemyMoveState>();

                if (Attack())
                {
                    AttackEvent();
                }

                if (AdditionalAttack())
                {
                    if (_enemy.TryGetComponent(out GoldDragon goldDragon))
                    {
                        _stateMashine.SetState<AditionalAttackGoldDragon>();
                    }

                    if (_enemy.TryGetComponent(out Beholder beholder))
                    {
                        _stateMashine.SetState<BeholderAdditionalAttackState>();
                    }
                }

                if (SpecialAttack())
                {
                    if (_enemy.TryGetComponent(out GoldDragon goldDragon))
                    {
                        _stateMashine.SetState<SpecialAttackGoldDragon>();
                    }

                    if (_enemy.TryGetComponent(out Beholder beholder))
                    {
                        _stateMashine.SetState<BeholderSpecialAttackState>();
                    }
                }
            }
        }

        private bool SpecialAttack()
        {
            if (_lastSpecialAttackTime <= 0)
            {
                _lastSpecialAttackTime = _specialAttackDelay;
                _canTransit = false;
                return true;
            }

            _lastSpecialAttackTime -= Time.deltaTime;
            return false;
        }

        private bool AdditionalAttack()
        {
            if (_distanceToTarget <= _attackRange)
            {
                if (_lastAdditionalAttackTime <= 0)
                {
                    _lastAdditionalAttackTime = _additionalAttackDelay;
                    _canTransit = false;
                    return true;
                }
            }

            _lastAdditionalAttackTime -= Time.deltaTime;
            return false;
        }

        private void LaunchBullet()
        {
            _enemy.transform.LookAt(_target.transform.position);
            _bulletSpawner.SpawnBullet();
            _canTransit = true;
        }
    }
}