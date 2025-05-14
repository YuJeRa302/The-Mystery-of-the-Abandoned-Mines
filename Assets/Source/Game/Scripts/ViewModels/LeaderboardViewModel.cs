using System;

public class LeaderboardViewModel
{
    private readonly LeaderboardModel _leaderboardModel;
    private readonly MenuModel _menuModel;

    public LeaderboardViewModel(LeaderboardModel leaderboardModel, MenuModel menuModel)
    {
        _leaderboardModel = leaderboardModel;
        _menuModel = menuModel;
        _menuModel.InvokedLeaderboardShow += () => InvokedShow?.Invoke();
        _menuModel.InvokedMainMenuShow += () => InvokedHide?.Invoke();
    }

    public event Action InvokedShow;
    public event Action InvokedHide;

    public void Hide() => _menuModel.InvokeLeaderboardHide();
    public int GetScore() => _leaderboardModel.GetScore();
}