namespace Assets.Source.Game.Scripts.Characters
{
    public class EnemyIdleState : State
    {
        private Player _target;

        public EnemyIdleState(StateMachine stateMashine, Player player) : base(stateMashine)
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
                StateMashine.SetState<EnemyMoveState>();
            }
        }

        public override void ExitState()
        {
        }
    }
}