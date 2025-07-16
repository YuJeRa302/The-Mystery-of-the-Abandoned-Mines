using UnityEngine;

namespace Assets.Source.Game.Scripts.Levels
{
    [System.Serializable]
    public class LevelState
    {
        [SerializeField] private int _id;
        [SerializeField] private bool _isComplete;

        public LevelState(int id, bool isComplete)
        {
            _id = id;
            _isComplete = isComplete;
        }

        public int Id => _id;
        public bool IsComplete => _isComplete;

        public void SetCompleteLevelStatus(bool status)
        {
            _isComplete = status;
        }
    }
}