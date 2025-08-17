using Assets.Source.Game.Scripts.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Source.Game.Scripts.Views
{
    public class LeaderboardView : MonoBehaviour
    {
        [SerializeField] private Button _openAuthorize;
        [SerializeField] private Button _closeAuthorize;
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _authorizationPanel;
        [SerializeField] private LeaderboardYG _leaderboardYG;

        private LeaderboardModel _leaderboardModel;
        private CompositeDisposable _disposables = new ();

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(LeaderboardModel leaderboardModel)
        {
            _leaderboardModel = leaderboardModel;
            AddListeners();
            gameObject.SetActive(false);
            _leaderboardYG.gameObject.SetActive(false);
        }

        private void AddListeners()
        {
            MessageBroker.Default
                .Receive<M_LeaderboardShowed>()
                .Subscribe(m => Show())
                .AddTo(_disposables);

            _closeButton.onClick.AddListener(OnExitButtonClicked);
            _closeAuthorize.onClick.AddListener(OnCloseAuthorizeClicked);
            _openAuthorize.onClick.AddListener(Authorize);
        }

        private void RemoveListeners()
        {
            if (_disposables != null)
                _disposables.Dispose();

            _closeButton.onClick.RemoveListener(OnExitButtonClicked);
            _closeAuthorize.onClick.RemoveListener(OnCloseAuthorizeClicked);
            _openAuthorize.onClick.RemoveListener(Authorize);
        }

        private void Authorize()
        {
            YG2.OpenAuthDialog();

            if (YG2.player.auth == true)
                CreateLeaderboard(_leaderboardModel.GetScore());
            else
                return;
        }

        private void CreateLeaderboard(int score)
        {
            _authorizationPanel.gameObject.SetActive(false);
            _leaderboardYG.gameObject.SetActive(true);
            _leaderboardYG.SetLeaderboard(score);
            _leaderboardYG.UpdateLB();
        }

        private void OnCloseAuthorizeClicked()
        {
            _authorizationPanel.gameObject.SetActive(false);
            MessageBroker.Default.Publish(new M_Hide());
        }

        private void OnExitButtonClicked() 
        {
            MessageBroker.Default.Publish(new M_Hide());
            gameObject.SetActive(false);
            _leaderboardYG.gameObject.SetActive(false);
        }

        private void Show()
        {
            gameObject.SetActive(true);

            if (YG2.player.auth == true)
                CreateLeaderboard(_leaderboardModel.GetScore());
            else
                _authorizationPanel.gameObject.SetActive(true);
        }
    }
}