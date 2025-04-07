using System;

public class PlayerWallet : IDisposable
{
    private int _currentCoins;

    public PlayerWallet()
    {
    }

    public int CurrentCoins => _currentCoins;

    public void AddCoins(int revard)
    {
        if (revard <= 0)
            return;

        _currentCoins += revard;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}