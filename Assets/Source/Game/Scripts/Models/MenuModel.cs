using System;

public class MenuModel : IDisposable
{
    public event Action InvokedUpgradesShow;
    public event Action InvokedLevelsShow;
    public event Action InvokedSettingsShow;
    public event Action InvokedWeaponsShow;
    public event Action InvokedClassAbilityShow;
    public event Action InvokedMainMenuShow;

    public void InvokeUpgradesShow() => InvokedUpgradesShow?.Invoke();
    public void InvokeUpgradesHide() => InvokedMainMenuShow?.Invoke();
    public void InvokeLevelsShow() => InvokedLevelsShow?.Invoke();
    public void InvokeLevelsHide() => InvokedMainMenuShow?.Invoke();
    public void InvokeSettingsShow() => InvokedSettingsShow?.Invoke();
    public void InvokeSettingsHide() => InvokedMainMenuShow?.Invoke();
    public void InvokeWeaponsShow() => InvokedWeaponsShow?.Invoke();
    public void InvokeWeaponsHide() => InvokedMainMenuShow?.Invoke();
    public void InvokeClassAbilityShow() => InvokedClassAbilityShow?.Invoke();
    public void InvokeClassAbilityHide() => InvokedMainMenuShow?.Invoke();
    public void InvokeMainMenuShow() => InvokedMainMenuShow?.Invoke();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}