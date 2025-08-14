namespace Assets.Source.Game.Scripts.Upgrades
{
    public struct M_MaxHealthChanged
    {
        private float _value;

        public M_MaxHealthChanged(float value)
        {
            _value = value;
        }

        public float Value => _value;
    }
}