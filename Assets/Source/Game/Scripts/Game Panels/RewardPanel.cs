using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class RewardPanel : GamePanelsView
    {
        [SerializeField] private Button _exitButton;

        private LevelObserver _levelObserver;

        public event Action ExitButtonClicked;

        private void Awake()
        {
            _exitButton.onClick.AddListener(ClickButtonExit);
        }

        public void ListenEndGame(LevelObserver levelObserver)
        {
            _levelObserver = levelObserver;
            _levelObserver.GameEnded += Open;
        }

        private void ClickButtonExit()
        {
            ExitButtonClicked?.Invoke();
        }
    }
}