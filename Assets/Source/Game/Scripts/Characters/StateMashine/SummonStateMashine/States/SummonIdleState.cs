using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class SummonIdleState : State
    {
        private Player _player;
        private Summon _summon;

        public SummonIdleState(StateMachine stateMachine, Player player, Summon summon) : base(stateMachine)
        {
            _player = player;
            _summon = summon;
        }

        public override void EnterState()
        {
            base.EnterState();

            SetIdleState();
        }

        public override void UpdateState()
        {
            if (_summon.FindEnemy(out Enemy target))
            {
                _summon.SetTarget(target);
                StateMachine.SetState<SummonMoveState>();
            }

            Vector3 directionToPlayer = _summon.transform.position - _player.transform.position;
            float distance = directionToPlayer.magnitude;

            if (distance > _summon.DistanceToTarget)
            {
                StateMachine.SetState<SummonMoveState>();
            }
        }
    }
}