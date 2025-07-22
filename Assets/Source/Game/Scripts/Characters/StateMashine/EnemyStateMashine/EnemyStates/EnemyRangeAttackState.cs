using Assets.Source.Game.Scripts.SpawnersScripts;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class EnemyRangeAttackState : EnemyAttackState
    {
        private BulletSpawner _bulletSpawner;

        public EnemyRangeAttackState(StateMachine stateMachine,
            Player target,
            Enemy enemy,
            BulletSpawner bulletSpawner)
            : base(stateMachine, target, enemy)
        {
            _bulletSpawner = bulletSpawner;
        }

        public override void SubscrabeIvent()
        {
            AnimationController.Attacked += LaunchBullet;
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
            }
        }

        private void LaunchBullet()
        {
            _bulletSpawner.SpawnBullet();
            SetTransitStatus(true);
        }
    }
}