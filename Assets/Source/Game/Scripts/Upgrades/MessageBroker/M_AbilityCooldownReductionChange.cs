namespace Assets.Source.Game.Scripts.Upgrades
{
    public struct M_AbilityCooldownReductionChange
    {
        private float _value;

        public M_AbilityCooldownReductionChange(float value)
        {
            _value = value;
        }

        public float Value => _value;
    }
}