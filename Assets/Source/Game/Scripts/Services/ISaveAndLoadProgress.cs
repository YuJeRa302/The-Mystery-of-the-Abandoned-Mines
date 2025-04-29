using Cysharp.Threading.Tasks;

public interface ISaveAndLoadProgress
{
    UniTask<(bool Success, GameInfo Data)> TryGetGameData();
    UniTask SaveData();
}