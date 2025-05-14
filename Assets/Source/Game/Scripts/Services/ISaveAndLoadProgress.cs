using YG;

public interface ISaveAndLoadProgress
{
    bool TryGetGameData(out SavesYG gameInfo);
    void SaveData();
}