using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.Upgrades;
using UniRx;

namespace Assets.Source.Game.Scripts.Models
{
    public class UpgradeModel
    {
        private readonly PersistentDataService _persistentDataService;
        private readonly int _maxStatsLevel = 3;

        private UpgradeState _currentStats;
        private UpgradeData _currentUpgradeData;

        public UpgradeModel(PersistentDataService persistentDataService)
        {
            _persistentDataService = persistentDataService;
        }

        public int UpgradePoints => _persistentDataService.PlayerProgress.UpgradePoints;

        public void ResetUpgrade(int value)
        {
            _persistentDataService.PlayerProgress.UpgradePoints += value;
            MessageBroker.Default.Publish(new M_StatsReseted());
        }

        public UpgradeState GetUpgradeState(UpgradeData upgradeData)
        {
            return _persistentDataService.PlayerProgress.UpgradeService.GetUpgradeState(upgradeData);
        }

        public void SelectStats(UpgradeDataView upgradeDataView)
        {
            _currentStats = upgradeDataView.UpgradeState;
            _currentUpgradeData = upgradeDataView.UpgradeData;
        }

        public void UpgradeStats()
        {
            if (_currentStats == null)
                return;

            if (_currentStats.CurrentLevel >= _maxStatsLevel)
                return;

            if (!_persistentDataService.TrySpendUpgradePoints(
                    _currentUpgradeData.UpgradeParameters[_currentStats.CurrentLevel].Cost))
                return;

            _currentStats.ChangeCurrentLevel(_currentStats.CurrentLevel + 1);
            _persistentDataService.PlayerProgress.UpgradeService.SetUpgradeState(_currentStats);
            MessageBroker.Default.Publish(new M_StatsUpgraded(_currentStats));
        }
    }
}