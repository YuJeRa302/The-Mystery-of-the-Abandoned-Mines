namespace Assets.Source.Game.Scripts.Rooms
{
    [System.Serializable]
    public class RoomState
    {
        private int _id;
        private int _currentLevel;
        private bool _isComplete;

        public int Id => _id;
        public int CurrentLevel => _currentLevel;
        public bool IsComplete => _isComplete;
    }
}