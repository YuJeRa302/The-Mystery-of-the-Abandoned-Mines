using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class SummonAttackState : State
    {
        private float _lastAttackTime = 0;
        private float _attackDelay;
        private float _attackRange;
        private Vector3 _directionToTarget;
        private float _distanceToTarget;
        private DamageSource _damage;
        private SummonAnimation _animationController;

        private Summon _summon;
        private Enemy _target;

        public SummonAttackState(StateMachine stateMachine, Summon summon) : base(stateMachine)
        {
            _summon = summon;
            _attackDelay = _summon.AttackDelay;
            _attackRange = _summon.DistanceToTarget;
            _damage = _summon.DamageSource;
            _animationController = _summon.Animation;
            _animationController.Attacked += ApplyDamage;
        }

        public override void EnterState()
        {
            base.EnterState();
            SetTransitStatus(true);

            if (_summon.Target != null)
            {
                _target = _summon.Target;
            }
            else
            {
                StateMachine.SetState<SummonIdleState>();
            }
        }

        public override void UpdateState()
        {
            if (CanTransit)
            {
                if (_target == null || _target.isActiveAndEnabled == false)
                {
                    _summon.DisableTarget();
                    StateMachine.SetState<SummonIdleState>();
                }

                _directionToTarget = _summon.transform.position - _target.transform.position;
                _distanceToTarget = _directionToTarget.magnitude;

                if (_distanceToTarget > _attackRange)
                    StateMachine.SetState<SummonIdleState>();

                if (Attack())
                {
                    AttackEvent();
                }
            }
        }

        protected bool Attack()
        {
            if (_distanceToTarget <= _attackRange)
            {
                _summon.transform.LookAt(_target.transform.position);

                if (_lastAttackTime <= 0)
                {
                    _lastAttackTime = _attackDelay;
                    SetTransitStatus(false);
                    return true;
                }
            }

            _lastAttackTime -= Time.deltaTime;
            return false;
        }

        protected void ApplyDamage()
        {
            if (_target != null && _target.isActiveAndEnabled == true)
            {
                Vector3 directionToTarget = _summon.transform.position - _target.transform.position;
                float distance = directionToTarget.magnitude;

                if (distance <= _attackRange)
                    _target.TakeDamage(_damage);
            }

            SetTransitStatus(true);
        }
    }
}