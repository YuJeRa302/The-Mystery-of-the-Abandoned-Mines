using System;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts.Services
{
    public class GamePauseService : IDisposable
    {
        private readonly PersistentDataService _persistentDataService;
        private readonly GamePanelsService _gamePanelsService;
        private readonly float _pauseValue = 0;
        private readonly float _resumeValue = 1;

        public GamePauseService(GamePanelsService gamePanelsService, PersistentDataService persistentDataService)
        {
            _persistentDataService = persistentDataService;
            _gamePanelsService = gamePanelsService;
            AddListener();
        }

        public event Action<bool> GamePaused;
        public event Action<bool> GameResumed;

        public void Dispose()
        {
            RemoveListener();
        }

        private void AddListener()
        {
            YG2.onFocusWindowGame += OnVisibilityWindowGame;
            _gamePanelsService.ResumeByRewarded += OnResumeByReward;
            _gamePanelsService.PauseByRewarded += OnPauseByReward;
            _gamePanelsService.ByMenuResumed += OnResumeByMenu;
            _gamePanelsService.ByMenuPaused += OnPauseByMenu;
            _gamePanelsService.ByFullscreenAdResumed += OnResumeByFullscreenAd;
            _gamePanelsService.ByFullscreenAdPaused += OnPauseByFullscreenAd;
        }

        private void RemoveListener()
        {
            YG2.onFocusWindowGame -= OnVisibilityWindowGame;
            _gamePanelsService.ResumeByRewarded -= OnResumeByReward;
            _gamePanelsService.PauseByRewarded -= OnPauseByReward;
            _gamePanelsService.ByMenuResumed -= OnResumeByMenu;
            _gamePanelsService.ByMenuPaused -= OnPauseByMenu;
            _gamePanelsService.ByFullscreenAdResumed -= OnResumeByFullscreenAd;
            _gamePanelsService.ByFullscreenAdPaused -= OnPauseByFullscreenAd;
        }

        private void OnResumeByReward()
        {
            if (_persistentDataService.PlayerProgress.IsGamePause == true)
                Time.timeScale = _pauseValue;
            else
                Time.timeScale = _resumeValue;

            GameResumed?.Invoke(_persistentDataService.PlayerProgress.IsMuted);
        }

        private void OnPauseByReward()
        {
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke(true);
        }

        private void OnResumeByMenu()
        {
            Time.timeScale = _resumeValue;
            GameResumed?.Invoke(_persistentDataService.PlayerProgress.IsMuted);
            _persistentDataService.PlayerProgress.IsGamePause = false;
        }

        private void OnPauseByMenu()
        {
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke(_persistentDataService.PlayerProgress.IsMuted);
            _persistentDataService.PlayerProgress.IsGamePause = true;
        }

        private void OnResumeByFullscreenAd()
        {
            Time.timeScale = _resumeValue;
            GameResumed?.Invoke(_persistentDataService.PlayerProgress.IsMuted);
        }

        private void OnPauseByFullscreenAd()
        {
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke(true);
        }

        private void PauseGameByVisibilityWindow(bool state)
        {
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke(!state);
        }

        private void ResumeGameByVisibilityWindow(bool state)
        {
            if (_persistentDataService.PlayerProgress.IsGamePause == true)
                Time.timeScale = _pauseValue;
            else
                Time.timeScale = _resumeValue;

            GameResumed?.Invoke(state);
        }

        private void OnVisibilityWindowGame(bool state)
        {
            if (state == true)
                ResumeGameByVisibilityWindow(state);
            else
                PauseGameByVisibilityWindow(state);
        }
    }
}