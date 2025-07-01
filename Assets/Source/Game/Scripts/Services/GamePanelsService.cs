using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;

public class GamePanelsService : IDisposable
{
    private readonly List<GamePanelsView> _gamePanelsViews = new ();

    public event Action ByMenuPaused;
    public event Action ByMenuResumed;
    public event Action PauseByRewarded;
    public event Action ResumeByRewarded;
    public event Action ByFullscreenAdPaused;
    public event Action ByFullscreenAdResumed;
    public event Action GameClosed;

    public GamePanelsService(List<GamePanelsView> gamePanelsViews)
    {
        _gamePanelsViews = gamePanelsViews;
    }

    public void Dispose()
    {
        RemoveListener();
        GC.SuppressFinalize(this);
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
    }

    private void AddListener()
    {
        foreach (var panel in _gamePanelsViews)
        {
            panel.PanelOpened += () => ByMenuPaused?.Invoke();
            panel.PanelClosed += () => ByMenuResumed?.Invoke();
            panel.GameClosed += () => GameClosed?.Invoke();
            panel.RewardAdOpened += () => PauseByRewarded.Invoke();
            panel.RewardAdClosed += () => ResumeByRewarded.Invoke();
            panel.FullscreenAdOpened += () => ByFullscreenAdPaused.Invoke();
            panel.FullscreenAdClosed += () => ByFullscreenAdResumed.Invoke();
        }
    }

    private void RemoveListener()
    {
        foreach (var panel in _gamePanelsViews)
        {
            if (panel == null)
                continue;

            panel.PanelOpened -= () => ByMenuPaused?.Invoke();
            panel.PanelClosed -= () => ByMenuResumed?.Invoke();
            panel.GameClosed -= () => GameClosed?.Invoke();
            panel.RewardAdOpened -= () => PauseByRewarded.Invoke();
            panel.RewardAdClosed -= () => ResumeByRewarded.Invoke();
            panel.FullscreenAdOpened -= () => ByFullscreenAdPaused.Invoke();
            panel.FullscreenAdClosed -= () => ByFullscreenAdResumed.Invoke();
        }
    }
}
