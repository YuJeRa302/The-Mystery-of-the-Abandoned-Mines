namespace Assets.Source.Game.Scripts.Characters
{
    public class BossAdditionalAttackState : State
    {
        protected Player Target;
        protected Enemy Enemy;

        public BossAdditionalAttackState(StateMachine stateMachine, Player target, Enemy enemy)
            : base(stateMachine)
        {
            Target = target;
            Enemy = enemy;
        }

        public override void EnterState()
        {
            CanTransit = false;
        }

        public override void UpdateState()
        {
            if (CanTransit)
                StateMashine.SetState<EnemyMoveState>();
        }
    }
}