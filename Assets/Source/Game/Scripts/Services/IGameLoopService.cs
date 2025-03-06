using System;

namespace Assets.Source.Game.Scripts
{
    public interface IGameLoopService
    {
        event Action GamePaused;
        event Action GameResumed;
        event Action GameClosed;
        event Action StageCompleted;
        bool IsPaused { get; }
        void ResumeByRewarded();
        void PauseByRewarded();
        void ResumeByMenu();
        void PauseByMenu();
        void ResumeByInterstitial();
        void PauseByInterstitial();
    }
}