using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class BossAttackState : AttackState
    {
        private float _additionalAttackDelay = 7f;
        protected float _lastAdditionalAttackTime = 0;

        public BossAttackState(StateMashine stateMashine, Player target, Enemy enemy) : base(stateMashine, target, enemy)
        {
            _enemy = enemy;
            _target = target;
            _attackRange = _enemy.AttackDistance;
            _damage = _enemy.Damage;
            _attackDelay = _enemy.AttackDelay;
            _animationController = _enemy.AnimationStateController;
            Boss boss = enemy as Boss;
            _additionalAttackDelay = boss.AdditionalAttackDelay;
            _animationController.Attacked += ApplyDamage;
            _animationController.AdditionalAttacked += AditionalAttackAppalyDamage;
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

                if (AdditionalAttack())
                {
                    AdditionalAttackEvent();
                }
            }
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

        private void AditionalAttackAppalyDamage()
        {
            _canTransit = true;
        }
    }
}