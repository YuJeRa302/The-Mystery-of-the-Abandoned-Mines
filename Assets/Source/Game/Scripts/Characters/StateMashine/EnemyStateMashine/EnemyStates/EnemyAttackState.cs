using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class EnemyAttackState : State
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

        public EnemyAttackState(StateMashine stateMashine, Player target, Enemy enemy) : base(stateMashine)
        {
            _enemy = enemy;
            _target = target;
            _attackRange = _enemy.AttackDistance;
            _damage = _enemy.Damage;
            _attackDelay = _enemy.AttackDelay;
            _animationController = _enemy.AnimationStateController;
            SubscrabeIvent();
        }

        public override void EnterState()
        {
            base.EnterState();
            _canTransit = true;
        }

        public virtual void SubscrabeIvent()
        {
            _animationController.Attacked += ApplyDamage;
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
                    _canTransit = false;
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
                _target.PlayerHealth.TakeDamage(Convert.ToInt32(_damage));

            _canTransit = true;
        }
    }
}