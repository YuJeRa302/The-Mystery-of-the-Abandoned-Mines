using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.ViewModels;
using UniRx;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts.GamePanels
{
    public abstract class GamePanelsView : MonoBehaviour
    {
        protected GamePanelsViewModel GamePanelsViewModel => _gamePanelsViewModel;

        private GamePanelsViewModel _gamePanelsViewModel;
        private CompositeDisposable _disposables = new ();

        public virtual void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            _gamePanelsViewModel = gamePanelsViewModel;
            gameObject.SetActive(false);
            MessageBroker.Default.Receive<M_ClosePanels>().Subscribe(m => OnPanelsClosed()).AddTo(_disposables);
        }

        protected virtual void Open()
        {
            gameObject.SetActive(true);
            MessageBroker.Default.Publish(new M_OpenPanel());
        }

        protected virtual void Close()
        {
            gameObject.SetActive(false);
            MessageBroker.Default.Publish(new M_ClosePanel());
        }

        protected virtual void CloseGame()
        {
#if UNITY_EDITOR
            MessageBroker.Default.Publish(new M_CloseGame());
#else
            YG2.InterstitialAdvShow();
#endif
        }

        protected virtual void OpenRewardAds()
        {
            MessageBroker.Default.Publish(new M_OpenAdReward());
        }

        protected virtual void CloseRewardAds()
        {
            MessageBroker.Default.Publish(new M_CloseAdReward());
        }

        protected virtual void OpenFullscreenAds()
        {
            MessageBroker.Default.Publish(new M_OpenFullscreenAd());
        }

        protected virtual void CloseFullscreenAds()
        {
            MessageBroker.Default.Publish(new M_CloseFullscreenAd());
        }

        private void OnPanelsClosed() 
        {
            gameObject.SetActive(false);
        }
    }
}