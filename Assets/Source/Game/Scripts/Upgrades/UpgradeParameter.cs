using UnityEngine;

namespace Assets.Source.Game.Scripts.Upgrades
{
    [System.Serializable]
    public class UpgradeParameter
    {
        [SerializeField] private int _currentLevel;
        [SerializeField] private int _cost;
        [SerializeField] private int _value;

        public int CurrentLevel => _currentLevel;
        public int Cost => _cost;
        public int Value => _value;
    }
}