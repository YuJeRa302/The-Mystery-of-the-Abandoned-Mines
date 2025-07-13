namespace Assets.Source.Game.Scripts.States
{
    [System.Serializable]
    public class ClassAbilityState
    {
        public int Id;
        public int CurrentLevel;

        public ClassAbilityState(int id, int currentLvl)
        {
            Id = id;
            CurrentLevel = currentLvl;
        }
    }
}