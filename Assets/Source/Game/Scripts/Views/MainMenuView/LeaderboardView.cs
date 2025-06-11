using UnityEngine;
using UnityEngine.UI;
using YG;

public class LeaderboardView : MonoBehaviour
{
    [SerializeField] private Button _openAuthorize;
    [SerializeField] private Button _closeAuthorize;
    [SerializeField] private Button _closeButton;
    [SerializeField] private GameObject _authorizationPanel;
    [SerializeField] private LeaderboardYG _leaderboardYG;

    private LeaderboardViewModel _leaderboardViewModel;

    private void OnDestroy()
    {
        RemoveListeners();
    }

    public void Initialize(LeaderboardViewModel leaderboardViewModel)
    {
        _leaderboardViewModel = leaderboardViewModel;
        AddListeners();
        gameObject.SetActive(false);
        _leaderboardYG.gameObject.SetActive(false);
    }

    private void AddListeners()
    {
        _leaderboardViewModel.InvokedShow += Show;
        _leaderboardViewModel.InvokedHide += Hide;
        _closeButton.onClick.AddListener(OnExitButtonClicked);
        _closeAuthorize.onClick.AddListener(OnCloseAuthorizeClicked);
        _openAuthorize.onClick.AddListener(Authorize);
    }

    private void RemoveListeners()
    {
        _leaderboardViewModel.InvokedShow -= Show;
        _leaderboardViewModel.InvokedHide -= Hide;
        _closeButton.onClick.RemoveListener(OnExitButtonClicked);
        _closeAuthorize.onClick.RemoveListener(OnCloseAuthorizeClicked);
        _openAuthorize.onClick.RemoveListener(Authorize);
    }

    private void Authorize()
    {
        YG2.OpenAuthDialog();

        if (YG2.player.auth == true)
            CreateLeaderboard(_leaderboardViewModel.GetScore());
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
        _leaderboardViewModel.Hide();
    }

    private void OnExitButtonClicked() => _leaderboardViewModel.Hide();

    private void Show()
    {
        gameObject.SetActive(true);

        if (YG2.player.auth == true)
            CreateLeaderboard(_leaderboardViewModel.GetScore());
        else
            _authorizationPanel.gameObject.SetActive(true);
    }

    private void Hide() 
    {
        gameObject.SetActive(false);
        _leaderboardYG.gameObject.SetActive(false);
    }
}
