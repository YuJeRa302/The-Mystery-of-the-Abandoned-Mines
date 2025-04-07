using Lean.Localization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class StagePanel : GamePanelsView
    {
        [SerializeField] private Button _buttonNext;
        [SerializeField] private Button _buttonRestart;
        [SerializeField] private Button _buttonExit;
        [Space(20)]
        [SerializeField] private LeanLocalizedText _titleText;
        [SerializeField] private Text _numberStageText;
        [Space(20)]
        [SerializeField] private Text _playerHealth;
        [SerializeField] private Text _playerDamage;
        [SerializeField] private Text _coins;
        [SerializeField] private Text _rerollPoints;
        [Space(10)]
        [SerializeField] private Text _currentRoomLevel;
        [SerializeField] private Text _killCount;

        public event Action RestartButtonClicked;
        public event Action ExitButtonClicked;

        private void Awake()
        {
            _buttonNext.onClick.AddListener(ClickButtonNext);
            _buttonRestart.onClick.AddListener(ClickButtonRestart);
            _buttonExit.onClick.AddListener(ClickButtonExit);
        }

        private void OnDestroy()
        {
            _buttonNext.onClick.RemoveListener(ClickButtonNext);
            _buttonRestart.onClick.RemoveListener(ClickButtonRestart);
            _buttonExit.onClick.RemoveListener(ClickButtonExit);
            GamePanelsViewModel.StageCompleted -= Open;
        }

        public override void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            base.Initialize(gamePanelsViewModel);
            GamePanelsViewModel.StageCompleted += Open;
        }

        protected override void Open()
        {
            base.Open();
            FillGameParameters();
        }

        private void FillGameParameters()
        {
            _numberStageText.text = GamePanelsViewModel.GetCurrentRoomLevel().ToString() + " / " + GamePanelsViewModel.GetStagesCount().ToString();
            _playerHealth.text = GamePanelsViewModel.GetPlayer().PlayerHealth.CurrentHealth.ToString();
            _playerDamage.text = GamePanelsViewModel.GetPlayer().PlayerStats.Damage.ToString();
            _coins.text = GamePanelsViewModel.GetPlayer().PlayerWallet.CurrentCoins.ToString();
            _rerollPoints.text = GamePanelsViewModel.GetPlayer().PlayerStats.RerollPoints.ToString();
            _currentRoomLevel.text = GamePanelsViewModel.GetCurrentRoomLevel().ToString();
            _killCount.text = GamePanelsViewModel.GetPlayer().PlayerStats.CountKillEnemy.ToString();
        }

        private void ClickButtonNext() 
        {
            Close();
        }

        private void ClickButtonRestart()
        {
            RestartButtonClicked?.Invoke();
        }

        private void ClickButtonExit()
        {
            ExitButtonClicked?.Invoke();
        }
    }
}