using UnityEngine;

namespace Assets.Source.Game.Scripts.States
{
    [System.Serializable]
    public class ClassAbilityState
    {
        [SerializeField] private int _id;
        [SerializeField] private int _currentLevel;

        public ClassAbilityState(int id, int currentLvl)
        {
            _id = id;
            _currentLevel = currentLvl;
        }

        public int Id => _id;
        public int CurrentLevel => _currentLevel;

        public void ChangeCurrentLevel(int level)
        {
            _currentLevel = level;
        }
    }
}