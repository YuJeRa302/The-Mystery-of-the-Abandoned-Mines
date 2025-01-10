using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "Create Level Data", order = 51)]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private int _tier;
        [SerializeField] private int _countLevels;
        [SerializeField] private bool _isContractLevel;
        [SerializeField] private string _translationName;
        [SerializeField] private string _translationDescription;
        [SerializeField] private Sprite _icon;

        public int CountLevels => _countLevels;
        public bool IsContractLevel => _isContractLevel;
        public int Tier => _tier;
        public int Id => _id;
        public string TranslationName => _translationName;
        public string TranslationDescription => _translationDescription;
        public Sprite Icon => _icon;
    }
}