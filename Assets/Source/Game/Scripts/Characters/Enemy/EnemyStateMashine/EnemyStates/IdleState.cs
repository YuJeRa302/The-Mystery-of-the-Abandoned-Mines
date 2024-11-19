using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class IdleState : State
    {
        private Player _target;

        public IdleState(StateMashine stateMashine, Player player) : base(stateMashine)
        {
            _target = player;
        }

        public override void EnterState()
        {
        }

        public override void UpdateState()
        {
            if (_target != null)
            {
                _stateMashine.SetState<MoveState>();
            }
        }

        public override void ExitState()
        {
        }
    }
}