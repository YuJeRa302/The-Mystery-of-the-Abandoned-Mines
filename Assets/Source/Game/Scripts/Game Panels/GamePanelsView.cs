using Assets.Source.Game.Scripts.ViewModels;
using System;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts.GamePanels
{
    public abstract class GamePanelsView : MonoBehaviour
    {
        protected GamePanelsViewModel GamePanelsViewModel;

        public event Action PanelOpened;
        public event Action PanelClosed;
        public event Action RewardAdOpened;
        public event Action RewardAdClosed;
        public event Action FullscreenAdOpened;
        public event Action FullscreenAdClosed;
        public event Action GameClosed;

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
            YG2.InterstitialAdvShow();
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