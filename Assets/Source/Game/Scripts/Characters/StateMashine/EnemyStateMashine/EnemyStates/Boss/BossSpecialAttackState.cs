namespace Assets.Source.Game.Scripts
{
    public class BossSpecialAttackState : State
    {
        protected Player _target;
        protected Enemy _enemy;
        protected EnemyAnimation _animationController;

        public BossSpecialAttackState(StateMashine stateMashine, Player player, Enemy enemy) : base(stateMashine)
        {
            _target = player;
            _enemy = enemy;
            _animationController = _enemy.AnimationStateController;
        }

        public override void EnterState()
        {
            base.EnterState();
            _canTransit = false;
        }

        public override void UpdateState()
        {
            if (_canTransit)
                _stateMashine.SetState<EnemyMoveState>();
        }
    }
}