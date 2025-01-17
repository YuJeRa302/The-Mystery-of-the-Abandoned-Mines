using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;

public class UpgradeModel
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

    public int UpgradePoints { get; private set; }

    public void SetUpgradePoints(int value) 
    {
        UpgradePoints = value;
    }

    public void ResetUpgrade(int value) 
    {
        UpgradePoints += value;
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
            UpgradePoints -= _currentUpgradeData.UpgradeParameters[_currentStats.CurrentLevel].Cost;

            if (_currentStats.CurrentLevel < _maxStatsLevel)
                _currentStats.CurrentLevel++;

            InvokedStatsUpgrade?.Invoke(_currentStats);
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
        UpgradeState upgradeState = new();
        upgradeState.Id = upgradeData.Id;
        upgradeState.CurrentLevel = _minValue;
        _upgradeStates.Add(upgradeState);
        return upgradeState;
    }
}