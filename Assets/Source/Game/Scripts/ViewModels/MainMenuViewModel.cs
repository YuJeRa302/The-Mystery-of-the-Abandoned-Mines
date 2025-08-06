using Assets.Source.Game.Scripts.Models;
using System;

namespace Assets.Source.Game.Scripts.ViewModels
{
    public class MainMenuViewModel : IDisposable
    {
        private readonly MenuModel _menuModel;

        public MainMenuViewModel(MenuModel menuModel)
        {
            _menuModel = menuModel;
            _menuModel.InvokedMainMenuShowed += () => Showing?.Invoke();
            _menuModel.GamePaused += (state) => GamePaused?.Invoke(state);
            _menuModel.GameResumed += (state) => GameResumed?.Invoke(state);
        }

        public event Action Showing;
        public event Action<bool> GamePaused;
        public event Action<bool> GameResumed;

        public void InvokeLevelsShow() => _menuModel.InvokeLevelsShow();
        public void InvokeSettingsShow() => _menuModel.InvokeSettingsShow();
        public void InvokeUpgradesShow() => _menuModel.InvokeUpgradesShow();
        public void InvokeWeaponsShow() => _menuModel.InvokeWeaponsShow();
        public void InvokeClassAbilityShow() => _menuModel.InvokeClassAbilityShow();
        public void InvokeKnowledgeBaseShow() => _menuModel.InvokeKnowledgeBaseShow();
        public void InvokeLeaderboardShow() => _menuModel.InvokeLeaderboardShow();

        public void Dispose()
        {
            _menuModel.Dispose();
        }
    }
}