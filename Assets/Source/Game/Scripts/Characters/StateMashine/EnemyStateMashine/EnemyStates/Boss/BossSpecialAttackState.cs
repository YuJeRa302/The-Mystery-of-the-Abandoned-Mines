namespace Assets.Source.Game.Scripts
{
    public class BossSpecialAttackState : State
    {
        protected Player Target;
        protected Enemy Enemy;
        protected EnemyAnimation AnimationController;

        public BossSpecialAttackState(StateMashine stateMashine, Player player, Enemy enemy) : base(stateMashine)
        {
            Target = player;
            Enemy = enemy;
            AnimationController = Enemy.AnimationStateController;
        }

        public override void EnterState()
        {
            base.EnterState();
            CanTransit = false;
        }

        public override void UpdateState()
        {
            if (CanTransit)
                StateMashine.SetState<EnemyMoveState>();
        }
    }
}