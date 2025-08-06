using Assets.Source.Game.Scripts.GamePanels;
using Assets.Source.Game.Scripts.ViewModels;
using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts.Services
{
    public class GamePanelsService : IDisposable
    {
        private readonly List<GamePanelsView> _gamePanelsViews = new();

        public GamePanelsService(List<GamePanelsView> gamePanelsViews)
        {
            _gamePanelsViews = gamePanelsViews;
        }

        public event Action ByMenuPaused;
        public event Action ByMenuResumed;
        public event Action PauseByRewarded;
        public event Action ResumeByRewarded;
        public event Action ByFullscreenAdPaused;
        public event Action ByFullscreenAdResumed;
        public event Action GameClosed;

        public void Dispose()
        {
            RemoveListener();
        }

        public void ClosePanels()
        {
            foreach (var panel in _gamePanelsViews)
                panel.gameObject.SetActive(false);
        }

        public void InitGamePanels(GamePanelsViewModel gamePanelsViewModel)
        {
            foreach (var panel in _gamePanelsViews)
            {
                panel.Initialize(gamePanelsViewModel);
            }

            AddListener();
            gamePanelsViewModel.OpenCardPanel();
        }

        private void AddListener()
        {
            foreach (var panel in _gamePanelsViews)
            {
                panel.PanelOpened += OnPanelOpened;
                panel.PanelClosed += OnPanelClosed;
                panel.GameClosed += OnGameClosed;
                panel.RewardAdOpened += OnRewardAdOpened;
                panel.RewardAdClosed += OnRewardAdClosed;
                panel.FullscreenAdOpened += OnFullscreenAdOpened;
                panel.FullscreenAdClosed += OnFullscreenAdClosed;
            }
        }

        private void RemoveListener()
        {
            foreach (var panel in _gamePanelsViews)
            {
                if (panel == null)
                    continue;

                panel.PanelOpened -= OnPanelOpened;
                panel.PanelClosed -= OnPanelClosed;
                panel.GameClosed -= OnGameClosed;
                panel.RewardAdOpened -= OnRewardAdOpened;
                panel.RewardAdClosed -= OnRewardAdClosed;
                panel.FullscreenAdOpened -= OnFullscreenAdOpened;
                panel.FullscreenAdClosed -= OnFullscreenAdClosed;
            }
        }

        private void OnPanelOpened() => ByMenuPaused?.Invoke();
        private void OnPanelClosed() => ByMenuResumed?.Invoke();
        private void OnGameClosed() => GameClosed?.Invoke();
        private void OnRewardAdOpened() => PauseByRewarded.Invoke();
        private void OnRewardAdClosed() => ResumeByRewarded.Invoke();
        private void OnFullscreenAdOpened() => ByFullscreenAdPaused.Invoke();
        private void OnFullscreenAdClosed() => ByFullscreenAdResumed.Invoke();
    }
}