using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Tips", menuName = "Create Tips", order = 51)]
    public class TipData : ScriptableObject
    {
        [SerializeField] private string _tipTranslationName;
        [SerializeField] private Sprite _sprite;

        public string TipTrnaslationName => _tipTranslationName;
        public Sprite Sprite => _sprite;
    }
}