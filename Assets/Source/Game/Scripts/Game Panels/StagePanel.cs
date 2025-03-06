using Lean.Localization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class StagePanel : GamePanels
    {
        [SerializeField] private Button _buttonNext;
        [SerializeField] private Button _buttonRestart;
        [SerializeField] private Button _buttonExit;
        [Space(20)]
        [SerializeField] private LeanLocalizedText _titleText;
        [SerializeField] private Text _numberStageText;

        public event Action RestartButtonClicked;
        public event Action ExitButtonClicked;

        private void Awake()
        {
            gameObject.SetActive(false);
            _buttonNext.onClick.AddListener(ClickButtonNext);
            _buttonRestart.onClick.AddListener(ClickButtonRestart);
            _buttonExit.onClick.AddListener(ClickButtonExit);
        }

        private void OnDestroy()
        {
            _buttonNext.onClick.RemoveListener(ClickButtonNext);
            _buttonRestart.onClick.RemoveListener(ClickButtonRestart);
            _buttonExit.onClick.RemoveListener(ClickButtonExit);
            LevelObserver.StageCompleted -= Open;
        }

        public override void Initialize(Player player, LevelObserver levelObserver)
        {
            base.Initialize(player, levelObserver);
            LevelObserver.StageCompleted += Open;
        }

        protected override void Open()
        {
            base.Open();
            _numberStageText.text = LevelObserver.CurrentRoomLevel.ToString() + " / " + LevelObserver.CountStages.ToString();
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