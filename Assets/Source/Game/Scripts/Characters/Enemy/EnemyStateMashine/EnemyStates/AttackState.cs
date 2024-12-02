using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class AttackState : State
    {
        protected float _lastAttackTime = 0;
        protected float _attackDelay;
        protected float _attackRange;
        protected Vector3 _directionToTarget;
        protected float _distanceToTarget;
        protected float _damage;
        protected Player _target;
        protected Enemy _enemy;
        protected EnemyAnimation _animationController;

        protected bool _canTransit = true;

        public AttackState(StateMashine stateMashine, Player target, Enemy enemy) : base(stateMashine)
        {
            _enemy = enemy;
            _target = target;
            _attackRange = _enemy.AttackDistance;
            _damage = _enemy.Damage;
            _attackDelay = _enemy.AttackDelay;
            _animationController = _enemy.AnimationStateController;
            _animationController.Attacked += ApplyDamage;
        }

        public override void EnterState()
        {
            base.EnterState();
            _canTransit = true;
        }

        public override void UpdateState()
        {
            if (_canTransit)
            {
                _directionToTarget = _enemy.transform.position - _target.transform.position;
                _distanceToTarget = _directionToTarget.magnitude;

                if (_distanceToTarget > _attackRange)
                    _stateMashine.SetState<MoveState>();

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
                _enemy.transform.LookAt(_target.transform.position);

                if (_lastAttackTime <= 0)
                {
                    _lastAttackTime = _attackDelay;
                    _canTransit = false;
                    return true;
                }
            }

            _lastAttackTime -= Time.deltaTime;
            return false;
        }

        protected void ApplyDamage()
        {
            _canTransit = true;
        }
    }
}