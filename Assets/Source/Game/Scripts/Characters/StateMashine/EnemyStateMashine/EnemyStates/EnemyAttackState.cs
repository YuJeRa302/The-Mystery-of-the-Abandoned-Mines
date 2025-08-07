using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class EnemyAttackState : State
    {
        private float _lastAttackTime = 0;
        private float _attackDelay;
        private float _attackRange;
        private Vector3 _directionToTarget;
        private float _distanceToTarget;
        private float _damage;
        private Player _target;
        private Enemy _enemy;
        private EnemyAnimation _animationController;

        public EnemyAttackState(StateMachine stateMachine, Player target, Enemy enemy) : base(stateMachine)
        {
            _enemy = enemy;
            _target = target;
            _attackRange = _enemy.AttackDistance;
            _damage = _enemy.Damage;
            _attackDelay = _enemy.AttackDelay;
            _animationController = _enemy.AnimationStateController;
            SubscrabeIvent();
        }

        public float AttackRange => _attackRange;
        public Vector3 DirectionToTarget => _directionToTarget;
        public float DistanceToTarget => _distanceToTarget;
        public Player Target => _target;
        public Enemy Enemy => _enemy;
        public EnemyAnimation AnimationController => _animationController;

        public override void EnterState()
        {
            base.EnterState();
            SetTransitStatus(true);
        }

        public virtual void SubscrabeIvent()
        {
            _animationController.Attacked += ApplyDamage;
        }

        public override void UpdateState()
        {
            if (CanTransit)
            {
                _directionToTarget = _enemy.transform.position - _target.transform.position;
                _distanceToTarget = _directionToTarget.magnitude;

                if (_distanceToTarget > _attackRange)
                    StateMachine.SetState<EnemyMoveState>();

                if (Attack())
                {
                    AttackEvent();
                }
            }
        }

        protected virtual bool Attack()
        {
            if (_distanceToTarget <= _attackRange)
            {
                _enemy.transform.LookAt(_target.transform.position);

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
            Vector3 directionToTarget = _enemy.transform.position - _target.transform.position;
            float distance = directionToTarget.magnitude;

            if (distance <= _attackRange)
                _target.TakeDamage(Convert.ToInt32(_damage));

            SetTransitStatus(true);
        }

        protected void SetDirectionToTarget(Vector3 direction, float distance)
        {
            _directionToTarget = direction;
            _distanceToTarget = distance;
        }
    }
}