namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_CoinsAdd
    {
        private int _value;

        public M_CoinsAdd(int value)
        {
            _value = value;
        }

        public int Value => _value;
    }
}