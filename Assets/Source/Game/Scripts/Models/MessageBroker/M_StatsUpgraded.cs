using Assets.Source.Game.Scripts.Upgrades;

namespace Assets.Source.Game.Scripts.Models
{
    public struct M_StatsUpgraded
    {
        private UpgradeState _upgradeState;

        public M_StatsUpgraded(UpgradeState upgradeState)
        {
            _upgradeState = upgradeState;
        }

        public UpgradeState UpgradeState => _upgradeState;
    }
}