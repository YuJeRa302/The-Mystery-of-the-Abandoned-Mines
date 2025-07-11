using DG.Tweening;
using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class UpgradesView : MonoBehaviour
    {
        private readonly string _upgradeNameText = "UpgradeName";
        private readonly string _buttonResetText = "ButtonReset";
        private readonly string _nameText = "Name";
        private readonly string _descriptionText = "DescriptionsStats";
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
        [SerializeField] private Transform _statsConteiner;
        [SerializeField] private ClassAbilityStatsView _statsViewPrafab;
        [SerializeField] private Text _currentPrice;
        [Space(20)]
        [SerializeField] private List<UpgradeData> _defaultUpgradeData;
        [Space(20)]
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _closeButton;
        [Space(20)]
        [SerializeField] private GameObject _parametrPanel;

        private List<UpgradeDataView> _upgradeDataViews = new();
        private UpgradeViewModel _upgradeViewModel;
        private IAudioPlayerService _audioPlayerService;
        private Coroutine _coroutine;
        private List<ClassAbilityStatsView> _classAbilityStatsViews = new();

        public List<UpgradeData> UpgradeDatas => _defaultUpgradeData;

        private void OnDestroy()
        {
            RemoveListener();
            _upgradeViewModel.Dispose();
        }

        private void OnEnable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
        }

        private void OnDisable()
        {
            _parametrPanel.SetActive(false);
        }

        public void Initialize(UpgradeViewModel upgradeViewModel, IAudioPlayerService audioPlayerService)
        {
            _audioPlayerService = audioPlayerService;
            _upgradeViewModel = upgradeViewModel;
            _countUpgradePoints.text = _upgradeViewModel.GetUpgradePoints().ToString();
            _statsImage.sprite = _defaultSprite;
            _nameStats.TranslationName = _nameText;
            _description.TranslationName = _descriptionText;
            _namePanel.TranslationName = _upgradeNameText;
            _resetButtonText.TranslationName = _buttonResetText;
            AddListener();
            _parametrPanel.SetActive(false);
            gameObject.SetActive(false);
        }

        private void AddListener()
        {
            _upgradeViewModel.Showing += Show;
            _upgradeViewModel.InvokedStatsUpgraded += OnStatsUpgraded;
            _closeButton.onClick.AddListener(OnExitButtonClicked);
            _upgradeButton.onClick.AddListener(Upgrade);
            _resetButton.onClick.AddListener(ResetUpgrades);
        }

        private void RemoveListener()
        {
            _upgradeViewModel.Showing -= Show;
            _upgradeViewModel.InvokedStatsUpgraded -= OnStatsUpgraded;
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
            if (_upgradeDataViews.Count == 0)
                return;

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

            _parametrPanel.SetActive(false);
            _upgradeViewModel.ResetUpgrades(currentUpgradePoints);
            _countUpgradePoints.text = _upgradeViewModel.GetUpgradePoints().ToString();
            Clear();
            Fill();
            _audioPlayerService?.PlayOneShotButtonClickSound();
        }

        private void Upgrade()
        {
            _upgradeViewModel.UpgradeStats();
            _audioPlayerService?.PlayOneShotButtonClickSound();
        }

        private void RenderCurrentUpgrade(UpgradeDataView upgradeDataView)
        {
            _statsImage.sprite = upgradeDataView.UpgradeData.Icon;
            _nameStats.TranslationName = upgradeDataView.UpgradeData.Name;
            _description.TranslationName = upgradeDataView.UpgradeData.Description;
        }

        private void ClearStats()
        {
            foreach (var view in _classAbilityStatsViews)
            {
                Destroy(view.gameObject);
            }

            _classAbilityStatsViews.Clear();
        }

        private void OnStatsSelected(UpgradeDataView upgradeDataView)
        {
            _parametrPanel.SetActive(true);
            RenderCurrentUpgrade(upgradeDataView);
            _upgradeViewModel.SelectStats(upgradeDataView);
            ClearStats();

            int currentLevel = upgradeDataView.UpgradeState.CurrentLevel;
            int parametersCount = upgradeDataView.UpgradeData.UpgradeParameters.Count;
            string nameParametr = upgradeDataView.UpgradeData.TypeParameter.ToString();

            string valueCurrentLvl;
            string valueNextLvl = string.Empty;
            int priceLevel;

            if (currentLevel > 0)
            {
                valueCurrentLvl = "+" + upgradeDataView.UpgradeData.UpgradeParameters[currentLevel - 1].Value.ToString();

                if (currentLevel < parametersCount)
                {
                    valueNextLvl = "+" + upgradeDataView.UpgradeData.UpgradeParameters[currentLevel].Value.ToString();
                    priceLevel = currentLevel;
                }
                else
                {
                    priceLevel = currentLevel - 1;
                }
            }
            else
            {
                valueCurrentLvl = "+" + upgradeDataView.UpgradeData.UpgradeParameters[0].Value.ToString();
                priceLevel = 0;
            }

            _currentPrice.text = upgradeDataView.UpgradeData.UpgradeParameters[priceLevel].Cost.ToString();
            ClassAbilityStatsView statsView = Instantiate(_statsViewPrafab, _statsConteiner);
            statsView.Initialize(nameParametr, valueCurrentLvl, valueNextLvl);
            _classAbilityStatsViews.Add(statsView);
        }

        private void OnStatsUpgraded(UpgradeState upgradeState)
        {
            _countUpgradePoints.text = _upgradeViewModel.GetUpgradePoints().ToString();

            foreach (var view in _upgradeDataViews)
            {
                if (view.UpgradeState.Id == upgradeState.Id)
                    OnStatsSelected(view);
            }
        }

        private IEnumerator SetUpgradeViewsAnimation()
        {
            WaitForSeconds delay = new WaitForSeconds(_delay);

            foreach (var view in _upgradeDataViews)
            {
                view.transform.localScale = Vector3.zero;
            }

            foreach (var view in _upgradeDataViews)
            {
                _audioPlayerService?.PlayOneShotPopupSound();

                view.transform.DOScale(_duration, _duration)
                    .SetEase(Ease.OutBounce)
                    .SetLink(view.gameObject);

                yield return new WaitForSeconds(_delay);
            }
        }

        private void OnExitButtonClicked()
        {
            _audioPlayerService?.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            Clear();
            _upgradeViewModel.Hide();
        }

        private void Show()
        {
            gameObject.SetActive(true);
            Clear();
            Fill();

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(SetUpgradeViewsAnimation());
        }
    }
}