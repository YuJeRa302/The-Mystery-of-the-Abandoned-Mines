using Assets.Source.Game.Scripts.SpawnersScripts;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class BossAttackState : EnemyAttackState
    {
        protected float LastAdditionalAttackTime = 8;
        protected float LastSpecialAttackTime = 0;

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
                    beholder.EnemyBulletPool,
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
                DirectionToTarget = Enemy.transform.position - Target.transform.position;
                DistanceToTarget = DirectionToTarget.magnitude;

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
            if (LastSpecialAttackTime <= 0)
            {
                LastSpecialAttackTime = _specialAttackDelay;
                SetTransitStatus(false);
                return true;
            }

            LastSpecialAttackTime -= Time.deltaTime;
            return false;
        }

        private bool AdditionalAttack()
        {
            if (DistanceToTarget <= AttackRange)
            {
                if (LastAdditionalAttackTime <= 0)
                {
                    LastAdditionalAttackTime = _additionalAttackDelay;
                    SetTransitStatus(false);
                    return true;
                }
            }

            LastAdditionalAttackTime -= Time.deltaTime;
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