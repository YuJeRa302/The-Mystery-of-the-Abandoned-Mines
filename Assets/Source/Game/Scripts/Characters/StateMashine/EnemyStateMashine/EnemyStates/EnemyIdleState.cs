namespace Assets.Source.Game.Scripts
{
    public class EnemyIdleState : State
    {
        private Player _target;

        public EnemyIdleState(StateMashine stateMashine, Player player) : base(stateMashine)
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
                _stateMashine.SetState<EnemyMoveState>();
            }
        }

        public override void ExitState()
        {
        }
    }
}