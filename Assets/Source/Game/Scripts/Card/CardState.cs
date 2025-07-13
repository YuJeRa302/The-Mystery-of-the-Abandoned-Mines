namespace Assets.Source.Game.Scripts.Card
{
    [System.Serializable]
    public class CardState
    {
        public int Id;
        public bool IsTaked;
        public bool IsLocked;
        public int Weight = 1;
        public int CurrentLevel;
        public bool IsCardUpgraded = false;
    }
}