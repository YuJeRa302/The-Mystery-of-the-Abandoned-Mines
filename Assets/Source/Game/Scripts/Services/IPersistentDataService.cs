namespace Assets.Source.Game.Scripts.Services
{
    public interface IPersistentDataService
    {
        void LoadDataFromCloud();
        void LoadDataFromPrefs();
        void SaveDataToPrefs();
        bool TrySpendCoins(int value);
        bool TrySpendUpgradePoints(int value);
    }
}