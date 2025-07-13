using Assets.Source.Game.Scripts.ViewModels;
using Lean.Localization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.GamePanels
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

        private void Awake()
        {
            _buttonNext.onClick.AddListener(ClickButtonNext);
        }

        private void OnDestroy()
        {
            _buttonNext.onClick.RemoveListener(ClickButtonNext);
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
            _numberStageText.text =
                GamePanelsViewModel.GetCurrentRoomLevel().ToString() +
                " / " + GamePanelsViewModel.GetStagesCount().ToString();
            _playerHealth.text = GamePanelsViewModel.GetPlayer().CurrentHealth.ToString();
            _playerDamage.text = GamePanelsViewModel.GetPlayer().DamageSource.Damage.ToString();
            _coins.text = GamePanelsViewModel.GetPlayer().Coins.ToString();
            _rerollPoints.text = GamePanelsViewModel.GetPlayer().RerollPoints.ToString();
            _currentRoomLevel.text = GamePanelsViewModel.GetCurrentRoomLevel().ToString();
            _killCount.text = GamePanelsViewModel.GetPlayer().KillCount.ToString();
        }

        private void ClickButtonNext()
        {
            Close();
        }
    }
}