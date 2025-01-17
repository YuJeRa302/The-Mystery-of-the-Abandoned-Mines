using Assets.Source.Game.Scripts;
using System;

public class ClassAbilityViewModel
{
    private readonly ClassAbilityModel _classAbilityModel;
    private readonly MenuModel _menuModel;

    public ClassAbilityViewModel(ClassAbilityModel classAbilityModel, MenuModel menuModel)
    {
        _classAbilityModel = classAbilityModel;
        _menuModel = menuModel;
        _menuModel.InvokedClassAbilityShow += () => InvokedShow?.Invoke();
        _menuModel.InvokedMainMenuShow += () => InvokedHide?.Invoke();
        _classAbilityModel.InvokedAbilityReset += (playerClassData) => InvokedAbilityReset?.Invoke(playerClassData);
        _classAbilityModel.InvokedAbilityUpgrade += (classAbilityState) => InvokedAbilityUpgrade?.Invoke(classAbilityState);
    }

    public event Action InvokedShow;
    public event Action InvokedHide;
    public event Action<PlayerClassData> InvokedAbilityReset;
    public event Action<ClassAbilityState> InvokedAbilityUpgrade;

    public void Hide() => _menuModel.InvokeClassAbilityHide();
    public int GetCoins() => _classAbilityModel.Coins;
    public ClassAbilityState GetClassAbilityState(ClassAbilityData classAbilityData) => _classAbilityModel.GetClassAbilityState(classAbilityData);
    public void ResetAbilities(int value) => _classAbilityModel.ResetAbilities(value);
    public void SelectClassAbility(ClassAbilityDataView classAbilityDataView) => _classAbilityModel.SelectClassAbility(classAbilityDataView);
    public void SelectPlayerClass(PlayerClassData playerClassData) => _classAbilityModel.SelectPlayerClass(playerClassData);
    public void UpgradeAbility() => _classAbilityModel.UpgradeAbility();
    public void UpdateTemporaryData() => _classAbilityModel.UpdateTemporaryData();
}