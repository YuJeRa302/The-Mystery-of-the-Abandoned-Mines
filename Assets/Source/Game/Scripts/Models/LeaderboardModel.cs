public class LeaderboardModel
{
    private readonly TemporaryData _temporaryData;

    public LeaderboardModel(TemporaryData temporaryData)
    {
        _temporaryData = temporaryData;
    }

    public int GetScore() => _temporaryData.PlayerScore;
}