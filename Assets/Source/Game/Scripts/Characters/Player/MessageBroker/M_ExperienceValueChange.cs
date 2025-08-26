namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_ExperienceValueChange
    {
        private int _value;

        public M_ExperienceValueChange(int value)
        {
            _value = value;
        }

        public int Value => _value;
    }
}