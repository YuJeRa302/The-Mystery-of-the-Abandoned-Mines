using Assets.Source.Game.Scripts.Card;
using System;
using UniRx;

namespace Assets.Source.Game.Scripts.Characters
{
    public class PlayerWallet : IDisposable
    {
        private int _currentCoins;
        private CompositeDisposable _disposables = new();

        public PlayerWallet()
        {
            AddListeners();
        }

        public int CurrentCoins => _currentCoins;

        public void Dispose()
        {
            if (_disposables != null)
                _disposables.Dispose();
        }

        private void AddListeners()
        {
            MessageBroker.Default
                .Receive<M_CoinsAdd>()
                .Subscribe(m => AddCoins(m.Value))
                .AddTo(_disposables);
        }

        private void AddCoins(int reward)
        {
            if (reward <= 0)
                return;

            _currentCoins += reward;
        }
    }
}