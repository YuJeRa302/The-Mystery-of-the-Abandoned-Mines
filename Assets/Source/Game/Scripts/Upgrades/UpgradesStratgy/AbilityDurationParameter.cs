using Assets.Source.Game.Scripts.Services;

namespace Assets.Source.Game.Scripts.Upgrades
{
    public class AbilityDurationParameter : IUpgradeStats
    {
        private float _currentDuration = 0;

        public float CurrentDuration => _currentDuration;

        public void Apply(float value)
        {
            _currentDuration = value;
        }
    }
}