namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_MoveSpeedReduced
    {
        private float _valueSlowed;
        private Enemy _enemy;

        public M_MoveSpeedReduced(float valueSlowed, Enemy enemy)
        {
            _valueSlowed = valueSlowed;
            _enemy = enemy;
        }

        public Enemy Enemy => _enemy;
        public float ValueSlowed => _valueSlowed;
    }
}