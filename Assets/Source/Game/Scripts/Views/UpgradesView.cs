using Assets.Source.Game.Scripts;
using DG.Tweening;
using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesView : MonoBehaviour
{
    private readonly string _upgradeNameText = "UpgradeName";
    private readonly string _buttonResetText = "ButtonReset";
    private readonly string _nameText = "Name";
    private readonly string _descriptionText = "Description";
    private readonly float _delay = 0.25f;
    private readonly float _duration = 1f;
    private readonly int _breakValue = -1;
    private readonly int _shiftValue = 1;

    [SerializeField] private LeanLocalizedText _description;
    [SerializeField] private LeanLocalizedText _nameStats;
    [SerializeField] private LeanLocalizedText _namePanel;
    [SerializeField] private LeanLocalizedText _resetButtonText;
    [SerializeField] private Text _countUpgradePoints;
    [Space(20)]
    [SerializeField] private Image _statsImage;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private UpgradeDataView _upgradeDataView;
    [SerializeField] private Transform _upgradesContainer;
    [Space(20)]
    [SerializeField] private List<UpgradeData> _defaultUpgradeData;
    [Space(20)]
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _closeButton;

    private List<UpgradeDataView> _upgradeDataViews = new ();
    private UpgradeViewModel _upgradeViewModel;
    private IAudioPlayerService _audioPlayerService;

    private void OnDestroy()
    {
        RemoveListener();
    }

    public void Initialize(UpgradeViewModel upgradeViewModel, IAudioPlayerService audioPlayerService)
    {
        _audioPlayerService = audioPlayerService;
        _upgradeViewModel = upgradeViewModel;
        _countUpgradePoints.text = _upgradeViewModel.GetUpgradePoints().ToString();
        //_statsImage.sprite = _defaultSprite;
        //_nameStats.TranslationName = _nameText;
        //_description.TranslationName = _descriptionText;
        //_namePanel.TranslationName = _upgradeNameText;
        //_resetButtonText.TranslationName = _buttonResetText;
        AddListener();
        gameObject.SetActive(false);
    }

    private void AddListener()
    {
        _upgradeViewModel.InvokedShow += Show;
        _upgradeViewModel.InvokedStatsUpgrade += OnStatsUpgraded;
        _closeButton.onClick.AddListener(OnExitButtonClicked);
        _upgradeButton.onClick.AddListener(Upgrade);
        _resetButton.onClick.AddListener(ResetUpgrades);
    }

    private void RemoveListener()
    {
        _upgradeViewModel.InvokedShow -= Show;
        _upgradeViewModel.InvokedStatsUpgrade -= OnStatsUpgraded;
        _closeButton.onClick.RemoveListener(OnExitButtonClicked);
        _upgradeButton.onClick.RemoveListener(Upgrade);
        _resetButton.onClick.RemoveListener(ResetUpgrades);
    }

    private void Fill()
    {
        foreach (UpgradeData upgradeData in _defaultUpgradeData)
        {
            UpgradeDataView view = Instantiate(_upgradeDataView, _upgradesContainer);
            _upgradeDataViews.Add(view);
            UpgradeState upgradeState = _upgradeViewModel.GetUpgradeState(upgradeData);
            view.Initialize(upgradeData, upgradeState, _upgradeViewModel, _audioPlayerService);
            view.StatsSelected += OnStatsSelected;
        }
    }

    private void Clear()
    {
        foreach (UpgradeDataView view in _upgradeDataViews)
        {
            view.StatsSelected -= OnStatsSelected;
            Destroy(view.gameObject);
        }

        _upgradeDataViews.Clear();
    }

    private void ResetUpgrades()
    {
        int currentUpgradePoints = 0;

        foreach (UpgradeDataView view in _upgradeDataViews)
        {
            for (int index = view.UpgradeState.CurrentLevel - _shiftValue; index > _breakValue; index--)
            {
                currentUpgradePoints += view.UpgradeData.UpgradeParameters[index].Cost;
            }
        }

        _upgradeViewModel.ResetUpgrades(currentUpgradePoints);
        _countUpgradePoints.text = _upgradeViewModel.GetUpgradePoints().ToString();
        Clear();
        Fill();
    }

    private void Upgrade() 
    {
        _upgradeViewModel.UpgradeStats();
    }

    private void OnStatsSelected(UpgradeDataView upgradeDataView)
    {
        _statsImage.sprite = upgradeDataView.UpgradeData.Icon;
        _nameStats.TranslationName = upgradeDataView.UpgradeData.Name;
        _description.TranslationName = upgradeDataView.UpgradeData.Description;
        _upgradeViewModel.SelectStats(upgradeDataView);
    }

    private void OnStatsUpgraded(UpgradeState upgradeState)
    {
        _countUpgradePoints.text = _upgradeViewModel.GetUpgradePoints().ToString();
    }

    private IEnumerator SetUpgradeViewsAnimation()
    {
        foreach (var view in _upgradeDataViews)
        {
            view.transform.localScale = Vector3.zero;
        }

        foreach (var view in _upgradeDataViews)
        {
            _audioPlayerService.PlayOneShotPopupSound();
            view.transform.DOScale(_duration, _duration).SetEase(Ease.OutBounce).SetLink(view.gameObject);
            yield return new WaitForSeconds(_delay);
        }
    }

    private void OnExitButtonClicked()
    {
        gameObject.SetActive(false);
        StopCoroutine(SetUpgradeViewsAnimation());
        Clear();
        _upgradeViewModel.Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        Fill();
        StartCoroutine(SetUpgradeViewsAnimation());
    }
}
