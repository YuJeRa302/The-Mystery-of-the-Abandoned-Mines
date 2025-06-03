using System;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts
{
    public abstract class GamePanelsView : MonoBehaviour
    {
        protected GamePanelsViewModel GamePanelsViewModel;

        public Action PanelOpened;
        public Action PanelClosed;
        public Action RewardAdOpened;
        public Action RewardAdClosed;
        public Action FullscreenAdOpened;
        public Action FullscreenAdClosed;
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
#if UNITY_EDITOR
            GameClosed?.Invoke();
#else
            YandexGame.FullscreenShow();
#endif
        }

        protected virtual void OpenRewardAds()
        {
            RewardAdOpened?.Invoke();
        }

        protected virtual void CloseRewardAds()
        {
            RewardAdClosed?.Invoke();
        }

        protected virtual void OpenFullscreenAds()
        {
            FullscreenAdOpened?.Invoke();
        }

        protected virtual void CloseFullscreenAds()
        {
            FullscreenAdClosed?.Invoke();
        }
    }
}