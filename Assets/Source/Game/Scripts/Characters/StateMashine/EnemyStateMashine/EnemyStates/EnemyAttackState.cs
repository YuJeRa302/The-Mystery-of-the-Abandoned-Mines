using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class EnemyAttackState : State
    {
        protected float LastAttackTime = 0;
        protected float AttackDelay;
        protected float AttackRange;
        protected Vector3 DirectionToTarget;
        protected float DistanceToTarget;
        protected float Damage;
        protected Player Target;
        protected Enemy Enemy;
        protected EnemyAnimation AnimationController;

        public EnemyAttackState(StateMachine stateMachine, Player target, Enemy enemy) : base(stateMachine)
        {
            Enemy = enemy;
            Target = target;
            AttackRange = Enemy.AttackDistance;
            Damage = Enemy.Damage;
            AttackDelay = Enemy.AttackDelay;
            AnimationController = Enemy.AnimationStateController;
            SubscrabeIvent();
        }

        public override void EnterState()
        {
            base.EnterState();
            CanTransit = true;
        }

        public virtual void SubscrabeIvent()
        {
            AnimationController.Attacked += ApplyDamage;
        }

        public override void UpdateState()
        {
            if (CanTransit)
            {
                DirectionToTarget = Enemy.transform.position - Target.transform.position;
                DistanceToTarget = DirectionToTarget.magnitude;

                if (DistanceToTarget > AttackRange)
                    StateMashine.SetState<EnemyMoveState>();

                if (Attack())
                {
                    AttackEvent();
                }
            }
        }

        protected virtual bool Attack()
        {
            if (DistanceToTarget <= AttackRange)
            {
                Enemy.transform.LookAt(Target.transform.position);

                if (LastAttackTime <= 0)
                {
                    LastAttackTime = AttackDelay;
                    CanTransit = false;
                    return true;
                }
            }

            LastAttackTime -= Time.deltaTime;
            return false;
        }

        protected void ApplyDamage()
        {
            Vector3 directionToTarget = Enemy.transform.position - Target.transform.position;
            float distance = directionToTarget.magnitude;

            if (distance <= AttackRange)
                Target.TakeDamage(Convert.ToInt32(Damage));

            CanTransit = true;
        }
    }
}