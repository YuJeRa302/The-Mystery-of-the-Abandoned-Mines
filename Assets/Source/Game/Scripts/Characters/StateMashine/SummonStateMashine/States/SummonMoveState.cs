using UnityEngine;
using UnityEngine.AI;

namespace Assets.Source.Game.Scripts.Characters
{
    public class SummonMoveState : State
    {
        private Player _player;
        private Summon _summon;
        private Transform _direction;
        private NavMeshAgent _navMeshAgent;

        public SummonMoveState(StateMachine stateMachine, Player player, Summon summon, NavMeshAgent navMeshAgent)
            : base(stateMachine)
        {
            _player = player;
            _summon = summon;
            _navMeshAgent = navMeshAgent;
        }

        public override void EnterState()
        {
            base.EnterState();
            _navMeshAgent.speed = _summon.MoveSpeed;
            MoveEvent();

            if (_summon.Target != null)
            {
                _direction = _summon.Target.transform;
            }
            else
            {
                _direction = _player.transform;
            }
        }

        public override void UpdateState()
        {
            if (_summon.FindEnemy(out Enemy target))
            {
                _summon.SetTarget(target);
                _direction = _summon.Target.transform;
            }

            Vector3 directionToTarget = _summon.transform.position - _direction.position;
            float distance = directionToTarget.magnitude;

            if (distance < _summon.DistanceToTarget)
            {
                if (_summon.Target != null)
                {
                    if (_summon.Target.isActiveAndEnabled != false)
                    {
                        StateMachine.SetState<SummonAttackState>();
                    }
                }
                else
                {
                    StateMachine.SetState<SummonIdleState>();
                }
            }

            Move();
        }

        private void Move()
        {
            _navMeshAgent.SetDestination(Vector3.forward + _direction.transform.position);
            _navMeshAgent.destination = _direction.transform.position;
            _summon.transform.LookAt(_direction.transform.position);
        }
    }
}