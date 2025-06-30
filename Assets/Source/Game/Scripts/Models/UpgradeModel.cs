using Assets.Source.Game.Scripts;
using System;

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

    public event Action<UpgradeState> InvokedStatsUpgrade;
    public event Action InvokedStatsReset;

    public int UpgradePoints => _persistentDataService.PlayerProgress.UpgradePoints;

    public void ResetUpgrade(int value) 
    {
        _persistentDataService.PlayerProgress.UpgradePoints += value;
        InvokedStatsReset?.Invoke();
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
            _persistentDataService.TrySpendUpgradePoints(_currentUpgradeData.UpgradeParameters[_currentStats.CurrentLevel].Cost);

            if (_currentStats.CurrentLevel < _maxStatsLevel)
                _currentStats.CurrentLevel++;

            InvokedStatsUpgrade?.Invoke(_currentStats);
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