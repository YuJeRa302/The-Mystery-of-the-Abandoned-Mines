using Assets.Source.Game.Scripts;
using System;

public class UpgradeViewModel
{
    private readonly UpgradeModel _upgradeModel;
    private readonly MenuModel _menuModel;

    public UpgradeViewModel(UpgradeModel upgradeModel, MenuModel menuModel)
    {
        _upgradeModel = upgradeModel;
        _menuModel = menuModel;
        _menuModel.InvokedUpgradesShow += () => InvokedShow?.Invoke();
        _menuModel.InvokedMainMenuShow += () => InvokedHide?.Invoke();
        _upgradeModel.InvokedStatsReset += () => InvokedStatsReset?.Invoke();
        _upgradeModel.InvokedStatsUpgrade += (upgradeState) => InvokedStatsUpgrade?.Invoke(upgradeState);
    }

    public event Action InvokedShow;
    public event Action InvokedHide;
    public event Action InvokedStatsReset;
    public event Action<UpgradeState> InvokedStatsUpgrade;

    public void Hide() => _menuModel.InvokeUpgradesHide();
    public int GetUpgradePoints() => _upgradeModel.UpgradePoints;
    public UpgradeState GetUpgradeState(UpgradeData upgradeData) => _upgradeModel.GetUpgradeState(upgradeData);
    public void ResetUpgrades(int value) => _upgradeModel.ResetUpgrade(value);
    public void SelectStats(UpgradeDataView upgradeDataView) => _upgradeModel.SelectStats(upgradeDataView);
    public void UpgradeStats() => _upgradeModel.UpgradeStats();
}
