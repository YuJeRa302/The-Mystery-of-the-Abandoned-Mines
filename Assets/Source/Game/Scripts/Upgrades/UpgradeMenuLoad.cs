using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class UpgradeMenuLoad : MenuTab
    {
        private readonly int _breakValue = -1;
        private readonly int _shiftValue = 1;
        private readonly int _minValue = 0;
        private readonly int _maxStatsLevel = 3;

        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private UpgradeDataView _upgradeDataView;
        [SerializeField] private Transform _upgradesContainer;
        [SerializeField] private List<UpgradeData> _defaultUpgradeData;

        private List<UpgradeDataView> _upgradeDataViews = new ();
        private UpgradeState _currentStats;
        private UpgradeData _currentUpgradeData;
        private int _currentUpgradePoints;

        public event Action<UpgradeState> StatsUpgraded;
        public event Action<UpgradeData> StatsSelected;
        public event Action StatsReseted;

        public List<UpgradeDataView> UpgradeDataViews => _upgradeDataViews;
        public int CurrentUpgradePoints => _currentUpgradePoints;

        protected override void Awake()
        {
            base.Awake();
            _upgradeButton.onClick.AddListener(UpgradeStats);
            _resetButton.onClick.AddListener(ResetStats);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _upgradeButton.onClick.RemoveListener(UpgradeStats);
            _resetButton.onClick.RemoveListener(ResetStats);
        }

        protected override void OpenTab()
        {
            Initialize();
            base.OpenTab();
        }

        protected override void CloseTab()
        {
            base.CloseTab();
            Clear();
            //TemporaryData.SetUpgradePoints(_currentUpgradePoints);
        }

        private void Initialize()
        {
            //_currentUpgradePoints = TemporaryData.UpgradePoints;
            Fill();
        }

        private void Fill()
        {
            foreach (UpgradeData upgradeData in _defaultUpgradeData)
            {
                //UpgradeDataView view = Instantiate(_upgradeDataView, _upgradesContainer);
                //_upgradeDataViews.Add(view);
                //UpgradeState upgradeState = TemporaryData.GetUpgradeState(upgradeData.Id);

                //if (upgradeState == null)
                //    upgradeState = InitState(upgradeData);

                //view.Initialize(
                //    upgradeData, 
                //    upgradeState, 
                //    this, 
                //    MenuPanel.MenuSoundPlayer.InterfaceAudioSource,
                //    MenuPanel.MenuSoundPlayer.AudioButtonClick,
                //    MenuPanel.MenuSoundPlayer.AudioButtonHover);

                //view.StatsSelected += OnStatsSelected;
            }
        }

        private UpgradeState InitState(UpgradeData upgradeData) 
        {
            UpgradeState upgradeState = new (upgradeData.Id, _minValue);
            return upgradeState;
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

        private void OnStatsSelected(UpgradeDataView upgradeDataView) 
        {
            _currentStats = upgradeDataView.UpgradeState;
            _currentUpgradeData = upgradeDataView.UpgradeData;
            StatsSelected?.Invoke(_currentUpgradeData);
        }

        private void ResetStats() 
        {
            foreach (UpgradeDataView view in _upgradeDataViews)
            {
                for (int index = view.UpgradeState.CurrentLevel - _shiftValue; index > _breakValue; index--) 
                {
                    _currentUpgradePoints += view.UpgradeData.UpgradeParameters[index].Cost;
                }
            }

            StatsReseted.Invoke();
            Clear();
            Fill();
        }

        private void UpgradeStats() 
        {
            if (_currentStats == null)
                return;

            if (_currentStats.CurrentLevel >= _currentUpgradeData.UpgradeParameters.Count)
                return;

            if (_currentUpgradePoints >= _currentUpgradeData.UpgradeParameters[_currentStats.CurrentLevel].Cost)
            {
                _currentUpgradePoints -= _currentUpgradeData.UpgradeParameters[_currentStats.CurrentLevel].Cost;

                if (_currentStats.CurrentLevel < _maxStatsLevel)
                    _currentStats.CurrentLevel++;

                StatsUpgraded?.Invoke(_currentStats);
            }
            else 
            {
                return;
            }
        }
    }
}