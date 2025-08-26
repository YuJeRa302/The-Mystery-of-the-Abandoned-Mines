namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_KillCountChange
    {
        private int _value;

        public M_KillCountChange(int value)
        {
            _value = value;
        }

        public int Value => _value;
    }
}