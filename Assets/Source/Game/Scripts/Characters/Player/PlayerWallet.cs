using System;

namespace Assets.Source.Game.Scripts.Characters
{
    public class PlayerWallet : IDisposable
    {
        private int _currentCoins;

        public int CurrentCoins => _currentCoins;

        public void AddCoins(int reward)
        {
            if (reward <= 0)
                return;

            _currentCoins += reward;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}