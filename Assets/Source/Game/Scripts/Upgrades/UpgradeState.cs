using UnityEngine;

namespace Assets.Source.Game.Scripts.Upgrades
{
    [System.Serializable]
    public class UpgradeState
    {
        [SerializeField] private int _id;
        [SerializeField] private int _currentLevel;

        public UpgradeState(int id, int currentLevel)
        {
            _id = id;
            _currentLevel = currentLevel;
        }

        public int Id => _id;
        public int CurrentLevel => _currentLevel;

        public void ChangeCurrentLevel(int level)
        {
            _currentLevel = level;
        }
    }
}