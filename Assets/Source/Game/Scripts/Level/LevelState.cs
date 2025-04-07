namespace Assets.Source.Game.Scripts
{
    [System.Serializable]
    public class LevelState
    {
        private int _id;
        private bool _isComplete;
        public int CurrentComplitStages;

        public LevelState(int id, bool isComplete, int CurrentStages)
        {
            _id = id;
            _isComplete = isComplete;
            CurrentComplitStages = CurrentStages;
        }

        public int Id => _id;
        public bool IsComplete => _isComplete;
    }
}