using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.Services;
using UniRx;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts.GamePanels
{
    public abstract class GamePanelsView : MonoBehaviour
    {
        protected GamePanelsModel GamePanelsModel => _gamePanelsModel;
        protected CompositeDisposable Disposable => _disposables;

        private GamePanelsModel _gamePanelsModel;
        private CompositeDisposable _disposables = new ();

        private void OnDestroy()
        {
            if (_disposables != null)
                _disposables.Dispose();
        }

        public virtual void Initialize(GamePanelsModel gamePanelsModel)
        {
            _gamePanelsModel = gamePanelsModel;
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