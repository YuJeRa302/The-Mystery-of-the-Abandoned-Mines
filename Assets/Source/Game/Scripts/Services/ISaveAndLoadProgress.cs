public interface ISaveAndLoadProgress
{
    public bool TryGetGameData(out GameInfo gameInfo);
    public void SaveData();
}