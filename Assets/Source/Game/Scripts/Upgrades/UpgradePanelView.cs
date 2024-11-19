//using DG.Tweening;
using Lean.Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class UpgradePanelView : MonoBehaviour
    {
        private readonly string _upgradeNameText = "UpgradeName";
        private readonly string _buttonResetText = "ButtonReset";
        private readonly string _nameText = "Name";
        private readonly string _descriptionText = "Description";
        private readonly float _delay = 0.25f;
        private readonly float _duration = 1f;

        [SerializeField] private UpgradeMenuLoad _upgradeMenuLoad;
        [SerializeField] private LeanLocalizedText _description;
        [SerializeField] private LeanLocalizedText _nameStats;
        [SerializeField] private LeanLocalizedText _namePanel;
        [SerializeField] private LeanLocalizedText _resetButtonText;
        [SerializeField] private Text _countUpgradePoints;
        [SerializeField] private Image _statsImage;
        [SerializeField] private Sprite _defaultSprite;

        private void Awake()
        {
            _upgradeMenuLoad.StatsSelected += OnStatsSelected;
            _upgradeMenuLoad.StatsUpgraded += OnStatsUpgraded;
            _upgradeMenuLoad.StatsReseted += OnStatsReseted;
            _upgradeMenuLoad.TabOpened += Initialize;
            _upgradeMenuLoad.TabClosed += OnClosePanel;
        }

        private void OnDestroy()
        {
            _upgradeMenuLoad.StatsSelected -= OnStatsSelected;
            _upgradeMenuLoad.StatsUpgraded -= OnStatsUpgraded;
            _upgradeMenuLoad.StatsReseted -= OnStatsReseted;
            _upgradeMenuLoad.TabOpened -= Initialize;
            _upgradeMenuLoad.TabClosed -= OnClosePanel;
        }

        private void Initialize()
        {
            _statsImage.sprite = _defaultSprite;
            _nameStats.TranslationName = _nameText;
            _description.TranslationName = _descriptionText;
            _countUpgradePoints.text = _upgradeMenuLoad.CurrentUpgradePoints.ToString();
            _namePanel.TranslationName = _upgradeNameText;
            _resetButtonText.TranslationName = _buttonResetText;
            StartCoroutine(SetUpgradeViewsAnimation());
        }

        private void OnStatsReseted()
        {
            _countUpgradePoints.text = _upgradeMenuLoad.CurrentUpgradePoints.ToString();
        }

        private void OnStatsSelected(UpgradeData upgradeData)
        {
            _statsImage.sprite = upgradeData.Icon;
            _nameStats.TranslationName = upgradeData.Name;
            _description.TranslationName = upgradeData.Description;
        }

        private void OnStatsUpgraded(UpgradeState upgradeState)
        {
            _countUpgradePoints.text = _upgradeMenuLoad.CurrentUpgradePoints.ToString();
        }

        private void OnClosePanel()
        {
            StopCoroutine(SetUpgradeViewsAnimation());
        }

        private IEnumerator SetUpgradeViewsAnimation()
        {
            foreach (var view in _upgradeMenuLoad.UpgradeDataViews)
            {
                view.transform.localScale = Vector3.zero;
            }

            foreach (var view in _upgradeMenuLoad.UpgradeDataViews)
            {
                //_upgradeMenuLoad.MenuSoundPlayer.InterfaceAudioSource.PlayOneShot(_upgradeMenuLoad.MenuSoundPlayer.AudioPopup);

                //view.transform.DOScale(_duration, _duration).
                //    SetEase(Ease.OutBounce).
                //    SetLink(view.gameObject);

                yield return new WaitForSeconds(_delay);
            }
        }
    }
}