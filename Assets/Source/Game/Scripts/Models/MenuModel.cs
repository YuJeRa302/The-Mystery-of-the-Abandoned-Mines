using System;
using YG;

public class MenuModel : IDisposable
{
    public MenuModel() 
    {
        YandexGame.onVisibilityWindowGame += OnVisibilityWindowGame;
    }

    public event Action InvokedUpgradesShow;
    public event Action InvokedLevelsShow;
    public event Action InvokedSettingsShow;
    public event Action InvokedWeaponsShow;
    public event Action InvokedClassAbilityShow;
    public event Action InvokedMainMenuShow;
    public event Action InvokeKnowBaswShow;
    public event Action InvokedLeaderboardShow;
    public event Action<bool> GamePaused;
    public event Action<bool> GameResumed;

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
    public void InvokeKnowledgeBaseShow() => InvokeKnowBaswShow?.Invoke();
    public void InvokeKnowledgeBaseHide() => InvokedMainMenuShow?.Invoke();
    public void InvokeLeaderboardShow() => InvokedLeaderboardShow?.Invoke();
    public void InvokeLeaderboardHide() => InvokedMainMenuShow?.Invoke();

    public void Dispose()
    {
        YandexGame.onVisibilityWindowGame -= OnVisibilityWindowGame;
        GC.SuppressFinalize(this);
    }

    private void OnVisibilityWindowGame(bool state)
    {
        if (state == true)
            GameResumed?.Invoke(state);
        else
            GamePaused?.Invoke(state);
    }
}