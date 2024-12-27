using System;

public class MainMenuViewModel
{
    private readonly MenuModel _menuModel;

    public MainMenuViewModel(MenuModel menuModel)
    {
        _menuModel = menuModel;
        _menuModel.InvokedMainMenuShow += () => InvokedShow?.Invoke();
    }

    public event Action InvokedShow;

    public void InvokeLevelsShow() => _menuModel.InvokeLevelsShow();
    public void InvokeSettingsShow() => _menuModel.InvokeSettingsShow();
    public void InvokeUpgradesShow() => _menuModel.InvokeUpgradesShow();
    public void InvokeWeaponsShow() => _menuModel.InvokeWeaponsShow();
    public void InvokeMainMenuShow() => _menuModel.InvokeMainMenuShow();
}
