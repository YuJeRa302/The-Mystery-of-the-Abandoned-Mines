public class LeaderboardModel
{
    private readonly PersistentDataService _persistentDataService;

    public LeaderboardModel(PersistentDataService persistentDataService)
    {
        _persistentDataService = persistentDataService;
    }

    public int GetScore() => _persistentDataService.PlayerProgress.Score;
}