namespace Assets.Source.Game.Scripts.Characters
{
    public class BossSpecialAttackState : State
    {
        protected Enemy Enemy;
        protected EnemyAnimation AnimationController;

        public BossSpecialAttackState(StateMachine stateMashine, Enemy enemy) : base(stateMashine)
        {
            Enemy = enemy;
            AnimationController = Enemy.AnimationStateController;
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