namespace Assets.Source.Game.Scripts.Upgrades
{
    public struct M_AbilityDamageChange
    {
        private float _value;

        public M_AbilityDamageChange(float value)
        {
            _value = value;
        }

        public float Value => _value;
    }
}