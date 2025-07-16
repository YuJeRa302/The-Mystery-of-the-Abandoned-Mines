namespace Assets.Source.Game.Scripts.Characters
{
    public class BossAdditionalAttackState : State
    {
        private Player _target;
        private Enemy _enemy;

        public BossAdditionalAttackState(StateMachine stateMachine, Player target, Enemy enemy)
            : base(stateMachine)
        {
            _target = target;
            _enemy = enemy;
        }

        protected Player Target => _target;
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