namespace Assets.Source.Game.Scripts.Characters
{
    public class BossAdditionalAttackState : State
    {
        protected Player Target;
        protected Enemy Enemy;

        public BossAdditionalAttackState(StateMachine stateMashine, Player target, Enemy enemy)
            : base(stateMashine)
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