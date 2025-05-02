using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public abstract class GamePanelsView : MonoBehaviour
    {
        protected GamePanelsViewModel GamePanelsViewModel;

        public Action PanelOpened;
        public Action PanelClosed;
        public Action AdOpened;
        public Action AdClosed;
        public Action GameClosed;

        public virtual void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            GamePanelsViewModel = gamePanelsViewModel;
            gameObject.SetActive(false);
        }

        protected virtual void Open()
        {
            gameObject.SetActive(true);
            PanelOpened?.Invoke();
        }

        protected virtual void Close()
        {
            gameObject.SetActive(false);
            PanelClosed?.Invoke();
        }

        protected virtual void CloseGame() 
        {
            gameObject.SetActive(false);
            GameClosed?.Invoke();
        }

        protected virtual void OpenAds()
        {
            AdOpened?.Invoke();
        }

        protected virtual void CloseAds()
        {
            gameObject.SetActive(false);
            AdClosed?.Invoke();
        }
    }
}