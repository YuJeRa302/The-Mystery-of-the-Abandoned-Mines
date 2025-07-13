using System;

namespace Assets.Source.Game.Scripts.Services
{
    public interface IGameLoopService
    {
        event Action<bool> GameEnded;
        event Action<bool> GamePaused;
        event Action<bool> GameResumed;
        event Action StageCompleted;

        void ResumeByRewarded();
        void PauseByRewarded();
        void ResumeByMenu();
        void PauseByMenu();
        void ResumeByFullscreenAd();
        void PauseByFullscreenAd();
    }
}