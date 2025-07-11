using System;

namespace Assets.Source.Game.Scripts
{
    public class ClassAbilityViewModel
    {
        private readonly ClassAbilityModel _classAbilityModel;
        private readonly MenuModel _menuModel;

        public ClassAbilityViewModel(ClassAbilityModel classAbilityModel, MenuModel menuModel)
        {
            _classAbilityModel = classAbilityModel;
            _menuModel = menuModel;
            _menuModel.InvokedClassAbilityShowed += () => Showing?.Invoke();
            _menuModel.InvokedMainMenuShowed += () => Hiding?.Invoke();
            _classAbilityModel.InvokedAbilityReseted += (playerClassData) => InvokedAbilityReseted?.Invoke(playerClassData);
            _classAbilityModel.InvokedAbilityUpgraded += (classAbilityState) => InvokedAbilityUpgraded?.Invoke(classAbilityState);
        }

        public event Action Showing;
        public event Action Hiding;
        public event Action<PlayerClassData> InvokedAbilityReseted;
        public event Action<ClassAbilityState> InvokedAbilityUpgraded;

        public void Hide() => _menuModel.InvokeClassAbilityHide();
        public int GetCoins() => _classAbilityModel.Coins;
        public ClassAbilityState GetClassAbilityState(ClassAbilityData classAbilityData) => _classAbilityModel.GetClassAbilityState(classAbilityData);
        public void ResetAbilities(int value) => _classAbilityModel.ResetAbilities(value);
        public void SelectClassAbility(ClassAbilityDataView classAbilityDataView) => _classAbilityModel.SelectClassAbility(classAbilityDataView);
        public void SelectPlayerClass(PlayerClassData playerClassData) => _classAbilityModel.SelectPlayerClass(playerClassData);
        public void UpgradeAbility() => _classAbilityModel.UpgradeAbility();
    }
}