using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class SummonAttackState : State
    {
        protected float LastAttackTime = 0;
        protected float AttackDelay;
        protected float AttackRange;
        protected Vector3 DirectionToTarget;
        protected float DistanceToTarget;
        protected DamageSource Damage;
        protected SummonAnimation AnimationController;

        private Summon _summon;
        private Enemy _target;

        public SummonAttackState(StateMachine stateMashine, Summon summon) : base(stateMashine)
        {
            _summon = summon;
            AttackDelay = _summon.AttackDelay;
            AttackRange = _summon.DistanceToTarget;
            Damage = _summon.DamageSource;
            AnimationController = _summon.Animation;
            AnimationController.Attacked += ApplyDamage;
        }

        public override void EnterState()
        {
            base.EnterState();
            CanTransit = true;

            if (_summon.Target != null)
            {
                _target = _summon.Target;
            }
            else
            {
                StateMashine.SetState<SummonIdleState>();
            }
        }

        public override void UpdateState()
        {
            if (CanTransit)
            {
                if (_target == null || _target.isActiveAndEnabled == false)
                {
                    _summon.DisableTarget();
                    StateMashine.SetState<SummonIdleState>();
                }

                DirectionToTarget = _summon.transform.position - _target.transform.position;
                DistanceToTarget = DirectionToTarget.magnitude;

                if (DistanceToTarget > AttackRange)
                    StateMashine.SetState<SummonIdleState>();

                if (Attack())
                {
                    AttackEvent();
                }
            }
        }

        protected bool Attack()
        {
            if (DistanceToTarget <= AttackRange)
            {
                _summon.transform.LookAt(_target.transform.position);

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
            if (_target != null && _target.isActiveAndEnabled == true)
            {
                Vector3 directionToTarget = _summon.transform.position - _target.transform.position;
                float distance = directionToTarget.magnitude;

                if (distance <= AttackRange)
                    _target.TakeDamage(Damage);
            }

            CanTransit = true;
        }
    }
}