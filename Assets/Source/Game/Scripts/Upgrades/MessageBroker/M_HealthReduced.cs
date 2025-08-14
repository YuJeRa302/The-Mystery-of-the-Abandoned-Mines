namespace Assets.Source.Game.Scripts.Upgrades
{
    public struct M_HealthReduced
    {
        private float _reduction;

        public M_HealthReduced(float reduction)
        {
            _reduction = reduction;
        }

        public float Reduction => _reduction;
    }
}