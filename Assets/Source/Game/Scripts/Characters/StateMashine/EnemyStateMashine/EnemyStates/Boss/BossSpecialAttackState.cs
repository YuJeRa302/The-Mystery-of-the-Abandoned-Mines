namespace Assets.Source.Game.Scripts.Characters
{
    public class BossSpecialAttackState : State
    {
        protected Enemy _enemy;
        protected EnemyAnimation _animationController;

        public BossSpecialAttackState(StateMachine stateMachine, Enemy enemy) : base(stateMachine)
        {
            _enemy = enemy;
            _animationController = Enemy.AnimationStateController;
        }

        protected Enemy Enemy => _enemy;
        protected EnemyAnimation AnimationController => _animationController;

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