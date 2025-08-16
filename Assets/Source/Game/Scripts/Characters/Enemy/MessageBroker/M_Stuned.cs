namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_Stuned
    {
        private bool _isStun;
        private EnemyStateMachineExample _enemyStateMachine;

        public M_Stuned(bool isStun, EnemyStateMachineExample enemyStateMachine)
        {
            _isStun = isStun;
            _enemyStateMachine = enemyStateMachine;
        }

        public EnemyStateMachineExample EnemyStateMachineExample => _enemyStateMachine;
        public bool IsStun => _isStun;
    }
}