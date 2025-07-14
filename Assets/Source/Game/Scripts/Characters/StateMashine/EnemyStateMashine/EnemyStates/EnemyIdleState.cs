namespace Assets.Source.Game.Scripts.Characters
{
    public class EnemyIdleState : State
    {
        private Player _target;

        public EnemyIdleState(StateMachine stateMachine, Player player) : base(stateMachine)
        {
            _target = player;
        }

        public override void UpdateState()
        {
            if (_target != null)
            {
                StateMashine.SetState<EnemyMoveState>();
            }
        }
    }
}