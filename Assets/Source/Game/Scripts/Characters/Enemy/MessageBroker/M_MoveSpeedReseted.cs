namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_MoveSpeedReseted
    {
        private Enemy _enemy;

        public M_MoveSpeedReseted(Enemy enemy)
        {
            _enemy = enemy;
        }

        public Enemy Enemy => _enemy;
    }
}