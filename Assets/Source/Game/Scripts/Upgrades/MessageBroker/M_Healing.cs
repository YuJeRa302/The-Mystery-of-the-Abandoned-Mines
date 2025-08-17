namespace Assets.Source.Game.Scripts.Upgrades
{
    public struct M_Healing
    {
        private float _value;

        public M_Healing(float value)
        {
            _value = value;
        }

        public float Value => _value;
    }
}