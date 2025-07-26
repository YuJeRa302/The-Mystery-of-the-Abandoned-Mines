using UnityEngine;

namespace Assets.Source.Game.Scripts.Card
{
    [System.Serializable]
    public class CardState
    {
        [SerializeField] private int _id;
        [SerializeField] private bool _isTaked;
        [SerializeField] private bool _isLocked = false;
        [SerializeField] private int _weight = 1;
        [SerializeField] private int _currentLevel;
        [SerializeField] private bool _isCardUpgraded = false;

        private int _minValueWeight = 1;
        private bool _isLegendariCard;

        public CardState(int id, bool isLocked, int currentLevel, bool isLegendari)
        {
            _id = id;
            _isLocked = isLocked;
            _currentLevel = currentLevel;
            _isLegendariCard = isLegendari;
        }

        public int Id => _id;
        public bool IsTaked => _isTaked;
        public bool IsLocked => _isLocked;
        public int Weight => _weight;
        public int CurrentLevel => _currentLevel;
        public bool IsCardUpgraded => _isCardUpgraded;
        public bool IsLegendariCard => _isLegendariCard;

        public void SetCardLocked(bool isLocked)
        {
            _isLocked = isLocked;
        }

        public void SetUpgradedStatus(bool status) => _isCardUpgraded = status;

        public void AddWeight() => _weight++;

        public void AddCurrentLevel() => _currentLevel++;

        public void ResetWeight() => _weight = _minValueWeight;
    }
}