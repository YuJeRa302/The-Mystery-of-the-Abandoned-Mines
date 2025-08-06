using Assets.Source.Game.Scripts.SpawnersScripts;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class BossAttackState : EnemyAttackState
    {
        private float _lastAdditionalAttackTime = 8;
        private float _lastSpecialAttackTime = 0;
        private BulletSpawner _bulletSpawner;
        private float _additionalAttackDelay = 7f;
        private float _specialAttackDelay = 7f;

        public BossAttackState(StateMachine stateMachine, Player target, Enemy enemy)
            : base(stateMachine, target, enemy)
        {
            Boss boss = enemy as Boss;
            _additionalAttackDelay = boss.AdditionalAttackDelay;
            _specialAttackDelay = boss.SpecialAttackDelay;
        }

        public override void SubscrabeIvent()
        {
            if (Enemy.TryGetComponent(out Beholder beholder))
            {
                _bulletSpawner = new BulletSpawner(beholder.Bullet,
                    beholder.EnemyPool,
                    beholder.BaseShotPoint, Enemy);
                AnimationController.Attacked += LaunchBullet;
            }
            else
            {
                AnimationController.Attacked += ApplyDamage;
            }

            AnimationController.AnimationCompleted += OnAllowTransition;
        }

        public override void UpdateState()
        {
            if (CanTransit)
            {
                Vector3 directionToTarget = Enemy.transform.position - Target.transform.position;
                float distanceToTarget = DirectionToTarget.magnitude;

                SetDirectionToTarget(directionToTarget, distanceToTarget);

                if (DistanceToTarget > AttackRange)
                    StateMachine.SetState<EnemyMoveState>();

                if (Attack())
                    AttackEvent();

                if (AdditionalAttack())
                {
                    if (Enemy.TryGetComponent(out GoldDragon goldDragon))
                        StateMachine.SetState<AditionalAttackGoldDragon>();

                    if (Enemy.TryGetComponent(out Beholder beholder))
                        StateMachine.SetState<BeholderAdditionalAttackState>();
                }

                if (SpecialAttack())
                {
                    if (Enemy.TryGetComponent(out GoldDragon goldDragon))
                        StateMachine.SetState<SpecialAttackGoldDragon>();

                    if (Enemy.TryGetComponent(out Beholder beholder))
                        StateMachine.SetState<BeholderSpecialAttackState>();
                }
            }
        }

        private bool SpecialAttack()
        {
            if (_lastSpecialAttackTime <= 0)
            {
                _lastSpecialAttackTime = _specialAttackDelay;
                SetTransitStatus(false);
                return true;
            }

            _lastSpecialAttackTime -= Time.deltaTime;
            return false;
        }

        private bool AdditionalAttack()
        {
            if (DistanceToTarget <= AttackRange)
            {
                if (_lastAdditionalAttackTime <= 0)
                {
                    _lastAdditionalAttackTime = _additionalAttackDelay;
                    SetTransitStatus(false);
                    return true;
                }
            }

            _lastAdditionalAttackTime -= Time.deltaTime;
            return false;
        }

        private void LaunchBullet()
        {
            Enemy.transform.LookAt(Target.transform.position);
            _bulletSpawner.SpawnBullet();
            SetTransitStatus(true);
        }
    }
}