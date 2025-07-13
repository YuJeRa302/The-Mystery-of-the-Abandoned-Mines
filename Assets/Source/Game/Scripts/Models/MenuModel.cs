using System;
using YG;

namespace Assets.Source.Game.Scripts.Models
{
    public class MenuModel : IDisposable
    {
        public MenuModel()
        {
            YG2.onFocusWindowGame += OnVisibilityWindowGame;
        }

        public event Action InvokedUpgradesShowed;
        public event Action InvokedLevelsShowed;
        public event Action InvokedSettingsShowed;
        public event Action InvokedWeaponsShowed;
        public event Action InvokedClassAbilityShowed;
        public event Action InvokedMainMenuShowed;
        public event Action InvokeKnowBaswShowed;
        public event Action InvokedLeaderboardShowed;
        public event Action<bool> GamePaused;
        public event Action<bool> GameResumed;

        public void InvokeUpgradesShow() => InvokedUpgradesShowed?.Invoke();
        public void InvokeUpgradesHide() => InvokedMainMenuShowed?.Invoke();
        public void InvokeLevelsShow() => InvokedLevelsShowed?.Invoke();
        public void InvokeLevelsHide() => InvokedMainMenuShowed?.Invoke();
        public void InvokeSettingsShow() => InvokedSettingsShowed?.Invoke();
        public void InvokeSettingsHide() => InvokedMainMenuShowed?.Invoke();
        public void InvokeWeaponsShow() => InvokedWeaponsShowed?.Invoke();
        public void InvokeWeaponsHide() => InvokedMainMenuShowed?.Invoke();
        public void InvokeClassAbilityShow() => InvokedClassAbilityShowed?.Invoke();
        public void InvokeClassAbilityHide() => InvokedMainMenuShowed?.Invoke();
        public void InvokeMainMenuShow() => InvokedMainMenuShowed?.Invoke();
        public void InvokeKnowledgeBaseShow() => InvokeKnowBaswShowed?.Invoke();
        public void InvokeKnowledgeBaseHide() => InvokedMainMenuShowed?.Invoke();
        public void InvokeLeaderboardShow() => InvokedLeaderboardShowed?.Invoke();
        public void InvokeLeaderboardHide() => InvokedMainMenuShowed?.Invoke();

        public void Dispose()
        {
            YG2.onFocusWindowGame -= OnVisibilityWindowGame;
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
}