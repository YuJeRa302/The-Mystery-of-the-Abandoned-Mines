namespace Assets.Source.Game.Scripts
{
    [System.Serializable]
    public class CardState
    {
        public int Id;
        public bool IsTaked;
        public bool IsLocked;
        public int Weight = 1;
        public int CurrentLevel;
    }
}