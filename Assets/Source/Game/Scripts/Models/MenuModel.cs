using System;
using UniRx;
using YG;

namespace Assets.Source.Game.Scripts.Models
{
    public class MenuModel : IDisposable
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private CompositeDisposable _disposables = new ();

        public MenuModel()
        {
            AddListeners();
        }

        public event Action InvokedMainMenuShowed;

        public void Dispose()
        {
            RemoveListeners();
        }

        private void OnShowMenu()
        {
            InvokedMainMenuShowed?.Invoke();
        }

        private void AddListeners()
        {
            YG2.onFocusWindowGame += OnVisibilityWindowGame;

            MessageBroker.Default
                .Receive<M_Hide>()
                .Subscribe(m => OnShowMenu())
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            YG2.onFocusWindowGame -= OnVisibilityWindowGame;
            _disposables?.Dispose();
        }

        private void OnVisibilityWindowGame(bool state)
        {
            if (state == true)
                Message.Publish(new M_GameResumed(state));
            else
                Message.Publish(new M_GamePaused(state));
        }
    }
}