using Assets.Source.Game.Scripts.Services;

namespace Assets.Source.Game.Scripts.Upgrades
{
    public class AbilityCooldownReductionParameter : IUpgradeStats
    {
        private float _currentReduction = 0;

        public float CurrentReduction => _currentReduction;

        public void Apply(float value)
        {
            _currentReduction = value;
        }
    }
}