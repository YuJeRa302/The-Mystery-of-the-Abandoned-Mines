using Assets.Source.Game.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "Create Level Data", order = 51)]
    public class LevelData : ScriptableObject, IIdData
    {
        [SerializeField] private int _id;
        [SerializeField] private int _tier;
        [SerializeField] private Color[] _tierColor;
        [SerializeField] private int _countStages;
        [SerializeField] private int _countRooms;
        [SerializeField] private int _cost;
        [SerializeField] private bool _isContractLevel;
        [SerializeField] private string _translationName;
        [SerializeField] private string _translationDescription;
        [SerializeField] private Sprite _icon;

        public Color TierColor => _tierColor[_tier];
        public int Cost => _cost;
        public int CountRooms => _countRooms;
        public int CountStages => _countStages;
        public bool IsContractLevel => _isContractLevel;
        public int Tier => _tier;
        public int Id => _id;
        public string TranslationName => _translationName;
        public string TranslationDescription => _translationDescription;
        public Sprite Icon => _icon;
    }
}