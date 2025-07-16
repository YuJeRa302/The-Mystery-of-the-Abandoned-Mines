using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.Upgrades;
using System;

namespace Assets.Source.Game.Scripts.Models
{
    public class UpgradeModel : IDisposable
    {
        private readonly PersistentDataService _persistentDataService;
        private readonly int _maxStatsLevel = 3;

        private UpgradeState _currentStats;
        private UpgradeData _currentUpgradeData;

        public UpgradeModel(PersistentDataService persistentDataService)
        {
            _persistentDataService = persistentDataService;
        }

        public event Action<UpgradeState> InvokedStatsUpgraded;
        public event Action InvokedStatsReseted;

        public int UpgradePoints => _persistentDataService.PlayerProgress.UpgradePoints;

        public void ResetUpgrade(int value)
        {
            _persistentDataService.PlayerProgress.UpgradePoints += value;
            InvokedStatsReseted?.Invoke();
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

            if (_currentStats.CurrentLevel >= _currentUpgradeData.UpgradeParameters.Count)
                return;

            if (UpgradePoints >= _currentUpgradeData.UpgradeParameters[_currentStats.CurrentLevel].Cost)
            {
                _persistentDataService.TrySpendUpgradePoints(
                    _currentUpgradeData.UpgradeParameters[_currentStats.CurrentLevel].Cost);

                if (_currentStats.CurrentLevel < _maxStatsLevel)
                {
                    int nextLevel = _currentStats.CurrentLevel + 1;
                    _currentStats.ChangeCurrentLevel(nextLevel);
                }

                InvokedStatsUpgraded?.Invoke(_currentStats);
                _persistentDataService.PlayerProgress.UpgradeService.SetUpgradeState(_currentStats);
            }
            else
            {
                return;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}