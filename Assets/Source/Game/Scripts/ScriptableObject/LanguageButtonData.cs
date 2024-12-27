using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New LangButton", menuName = "Create LangButton", order = 51)]
    public class LanguageButtonData : ScriptableObject
    {
        [SerializeField] private string _nameLanguage;
        [SerializeField] private Sprite _iconLanguage;

        public string NameLanguage => _nameLanguage;
        public Sprite IconLanguage => _iconLanguage;
    }
}