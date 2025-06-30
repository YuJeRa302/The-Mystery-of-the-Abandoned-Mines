public interface ISaveAndLoadProgress
{
    bool TryGetGameData();
    void SaveData();
    void SaveDataToPrefs();
    void LoadDataFromPrefs();
    void SaveGameProgerss(int score, int coins, int upgradePoints, int levelId, bool isComplete, bool isGameInterrupted);
}