namespace Assets.Source.Game.Scripts
{
    [System.Serializable]
    public class UpgradeState
    {
        public int Id;
        public int CurrentLevel;

        public UpgradeState(int id, int currentLevel)
        {
            Id = id;
            CurrentLevel = currentLevel;
        }
    }
}