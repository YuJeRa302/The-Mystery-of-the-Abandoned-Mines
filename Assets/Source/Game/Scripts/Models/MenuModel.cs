using System;
using UniRx;
using YG;

namespace Assets.Source.Game.Scripts.Models
{
    public class MenuModel : IDisposable
    {
        private CompositeDisposable _disposables = new ();

        public MenuModel()
        {
            YG2.onFocusWindowGame += OnVisibilityWindowGame;

            MessageBroker.Default
                .Receive<M_Hide>()
                .Subscribe(m => OnShowMenu())
                .AddTo(_disposables);
        }

        public event Action InvokedMainMenuShowed;
        public event Action<bool> GamePaused;
        public event Action<bool> GameResumed;

        public void Dispose()
        {
            YG2.onFocusWindowGame -= OnVisibilityWindowGame;
        }

        private void OnShowMenu()
        {
            InvokedMainMenuShowed?.Invoke();
        }

        private void OnVisibilityWindowGame(bool state)
        {
            if (state == true)
                GameResumed?.Invoke(state);
            else
                GamePaused?.Invoke(state);
        }
    }
}