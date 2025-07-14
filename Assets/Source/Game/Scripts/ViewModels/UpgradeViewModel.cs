using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Upgrades;
using System;

namespace Assets.Source.Game.Scripts.ViewModels
{
    public class UpgradeViewModel : IDisposable
    {
        private readonly UpgradeModel _upgradeModel;
        private readonly MenuModel _menuModel;

        public UpgradeViewModel(UpgradeModel upgradeModel, MenuModel menuModel)
        {
            _upgradeModel = upgradeModel;
            _menuModel = menuModel;
            _menuModel.InvokedUpgradesShowed += () => Showing?.Invoke();
            _menuModel.InvokedMainMenuShowed += () => Hiding?.Invoke();
            _upgradeModel.InvokedStatsReseted += () => InvokedStatsReseted?.Invoke();
            _upgradeModel.InvokedStatsUpgraded += (upgradeState) => InvokedStatsUpgraded?.Invoke(upgradeState);
        }

        public event Action Showing;
        public event Action Hiding;
        public event Action InvokedStatsReseted;
        public event Action<UpgradeState> InvokedStatsUpgraded;

        public void Hide() => _menuModel.InvokeUpgradesHide();
        public int GetUpgradePoints() => _upgradeModel.UpgradePoints;
        public UpgradeState GetUpgradeState(UpgradeData upgradeData) => _upgradeModel.GetUpgradeState(upgradeData);
        public void ResetUpgrades(int value) => _upgradeModel.ResetUpgrade(value);
        public void SelectStats(UpgradeDataView upgradeDataView) => _upgradeModel.SelectStats(upgradeDataView);
        public void UpgradeStats() => _upgradeModel.UpgradeStats();

        public void Dispose()
        {
            _upgradeModel.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}