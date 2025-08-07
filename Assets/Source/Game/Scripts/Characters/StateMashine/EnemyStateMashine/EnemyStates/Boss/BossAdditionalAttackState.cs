namespace Assets.Source.Game.Scripts.Characters
{
    public class BossAdditionalAttackState : State
    {
        private Enemy _enemy;

        public BossAdditionalAttackState(StateMachine stateMachine, Enemy enemy)
            : base(stateMachine)
        {
            _enemy = enemy;
        }

        protected Enemy Enemy => _enemy;

        public override void EnterState()
        {
            SetTransitStatus(false);
        }

        public override void UpdateState()
        {
            if (CanTransit)
                StateMachine.SetState<EnemyMoveState>();
        }
    }
}