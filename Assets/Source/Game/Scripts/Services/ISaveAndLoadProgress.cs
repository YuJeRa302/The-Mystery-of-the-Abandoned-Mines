namespace Assets.Source.Game.Scripts.Services
{
    public interface ISaveAndLoadProgress
    {
        void SaveDataToPrefs();
        void SaveGameProgerss(int score, int coins, int upgradePoints, int levelId, bool isComplete, bool isGameInterrupted);
    }
}