using UnityEngine;
using UnityEngine.AI;

namespace Assets.Source.Game.Scripts
{
    public class EnemyMoveState : State
    {
        private Player _target;
        private float _moveSpeed;
        private float _distanceToTransition;
        private Enemy _enemy;
        private NavMeshAgent _navMeshAgent;

        public EnemyMoveState(StateMashine stateMashine, Player player, NavMeshAgent meshAgent, Enemy enemy) : base(stateMashine)
        {
            _target = player;
            _enemy = enemy;
            _distanceToTransition = _enemy.AttackDistance;
            _moveSpeed = _enemy.Speed;
            _navMeshAgent = meshAgent;
        }

        public override void EnterState()
        {
            _moveSpeed = _enemy.Speed;
            _navMeshAgent.speed = _moveSpeed;
            MoveEvent();
        }

        public override void UpdateState()
        {
            Vector3 directionToTarget = _enemy.transform.position - _target.transform.position;
            float distance = directionToTarget.magnitude;

            if (distance <= _distanceToTransition)
            {
                if (_enemy.TryGetComponent(out Boss boss))
                    StateMashine.SetState<BossAttackState>();
                else if (_enemy.TryGetComponent(out RangeEnemy rangeEnemy))
                    StateMashine.SetState<EnemyRangeAttackState>();
                else
                    StateMashine.SetState<EnemyAttackState>();
            }

            Move();
        }

        public override void ExitState()
        {
            _navMeshAgent.speed = 0;
        }

        private void Move()
        {
            _navMeshAgent.SetDestination(Vector3.forward + _target.transform.position);
            _navMeshAgent.destination = _target.transform.position;
            _enemy.transform.LookAt(_target.transform.position);
        }
    }
}