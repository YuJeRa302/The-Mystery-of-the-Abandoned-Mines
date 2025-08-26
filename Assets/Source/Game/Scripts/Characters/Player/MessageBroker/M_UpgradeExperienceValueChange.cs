namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_UpgradeExperienceValueChange
    {
        private int _value;

        public M_UpgradeExperienceValueChange(int value)
        {
            _value = value;
        }

        public int Value => _value;
    }
}