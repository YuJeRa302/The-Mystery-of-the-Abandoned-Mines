using System;

namespace Assets.Source.Game.Scripts
{
    public class LeaderboardViewModel
    {
        private readonly LeaderboardModel _leaderboardModel;
        private readonly MenuModel _menuModel;

        public LeaderboardViewModel(LeaderboardModel leaderboardModel, MenuModel menuModel)
        {
            _leaderboardModel = leaderboardModel;
            _menuModel = menuModel;
            _menuModel.InvokedLeaderboardShowed += () => Showing?.Invoke();
            _menuModel.InvokedMainMenuShowed += () => Hiding?.Invoke();
        }

        public event Action Showing;
        public event Action Hiding;

        public void Hide() => _menuModel.InvokeLeaderboardHide();
        public int GetScore() => _leaderboardModel.GetScore();
    }
}