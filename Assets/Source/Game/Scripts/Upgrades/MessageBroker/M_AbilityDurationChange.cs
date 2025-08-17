namespace Assets.Source.Game.Scripts.Upgrades
{
    public struct M_AbilityDurationChange
    {
        private float _value;

        public M_AbilityDurationChange(float value)
        {
            _value = value;
        }

        public float Value => _value;
    }
}