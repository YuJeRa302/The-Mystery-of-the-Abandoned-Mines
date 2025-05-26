using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;

public class UpgradeModel : IDisposable
{
    private readonly TemporaryData _temporaryData;
    private readonly int _minValue = 0;
    private readonly int _maxStatsLevel = 3;

    private UpgradeState _currentStats;
    private UpgradeData _currentUpgradeData;
    private List<UpgradeState> _upgradeStates = new ();

    public UpgradeModel(TemporaryData temporaryData) 
    {
        _temporaryData = temporaryData;
        SetUpgradePoints(_temporaryData.UpgradePoints);

        if (_temporaryData.UpgradeStates != null)
            SetUpgradeState(_temporaryData.UpgradeStates);
    }

    public event Action<UpgradeState> InvokedStatsUpgrade;
    public event Action InvokedStatsReset;

    public int UpgradePoints => _temporaryData.UpgradePoints;

    public void SetUpgradePoints(int value) 
    {
        _temporaryData.SetUpgradePoints(value);
    }

    public void ResetUpgrade(int value) 
    {
        _temporaryData.AddUpgradePoints(value);
        InvokedStatsReset?.Invoke();
    }

    public UpgradeState GetUpgradeState(UpgradeData upgradeData) 
    {
        UpgradeState upgradeState = _temporaryData.GetUpgradeState(upgradeData.Id);

        if (upgradeState == null)
            upgradeState = InitState(upgradeData);

        return upgradeState;
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
            int cost = _currentUpgradeData.UpgradeParameters[_currentStats.CurrentLevel].Cost;
            _temporaryData.TrySpendUpgradePoints(cost);

            if (_currentStats.CurrentLevel < _maxStatsLevel)
                _currentStats.CurrentLevel++;

            InvokedStatsUpgrade?.Invoke(_currentStats);
            _temporaryData.SetUpgradeState(_upgradeStates);
        }
        else
        {
            return;
        }
    }

    public void UpdateTemporaryData() 
    {
        _temporaryData.SetUpgradeState(_upgradeStates);
    }

    private void SetUpgradeState(UpgradeState[] upgradeStates) 
    {
        foreach (UpgradeState state in upgradeStates)
        {
            _upgradeStates.Add(state);
        }
    }

    private UpgradeState InitState(UpgradeData upgradeData)
    {
        UpgradeState upgradeState = new(upgradeData.Id, _minValue);
        _upgradeStates.Add(upgradeState);
        return upgradeState;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}