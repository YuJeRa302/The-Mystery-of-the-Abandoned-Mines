using Assets.Source.Game.Scripts.GamePanels;
using System;
using UniRx;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts.Services
{
    public class GamePauseService : IDisposable
    {
        private readonly PersistentDataService _persistentDataService;
        private readonly float _pauseValue = 0;
        private readonly float _resumeValue = 1;

        private CompositeDisposable _disposables = new ();

        public GamePauseService(PersistentDataService persistentDataService)
        {
            _persistentDataService = persistentDataService;
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
            MessageBroker.Default.Receive<M_CloseAdReward>().Subscribe(m => OnResumeByReward()).AddTo(_disposables);
            MessageBroker.Default.Receive<M_OpenAdReward>().Subscribe(m => OnPauseByReward()).AddTo(_disposables);
            MessageBroker.Default.Receive<M_ClosePanel>().Subscribe(m => OnResumeByMenu()).AddTo(_disposables);
            MessageBroker.Default.Receive<M_OpenPanel>().Subscribe(m => OnPauseByMenu()).AddTo(_disposables);

            MessageBroker
                .Default
                .Receive<M_CloseFullscreenAd>()
                .Subscribe(m => OnResumeByFullscreenAd())
                .AddTo(_disposables);

            MessageBroker
                .Default
                .Receive<M_OpenFullscreenAd>()
                .Subscribe(m => OnPauseByFullscreenAd())
                .AddTo(_disposables);
        }

        private void RemoveListener()
        {
            YG2.onFocusWindowGame -= OnVisibilityWindowGame;

            if (_disposables != null)
                _disposables.Dispose();
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