using System;

public class MainMenuViewModel : IDisposable
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
    public void InvokeClassAbilityShow() => _menuModel.InvokeClassAbilityShow();
    public void InvokeMainMenuShow() => _menuModel.InvokeMainMenuShow();
    public void InvokeKnowledgeBaseShow() => _menuModel.InvokeKnowledgeBaseShow();
    public void InvokeLeaderboardShow() => _menuModel.InvokeLeaderboardShow();

    public void Dispose()
    {
        _menuModel.Dispose();
        GC.SuppressFinalize(this);
    }
}
