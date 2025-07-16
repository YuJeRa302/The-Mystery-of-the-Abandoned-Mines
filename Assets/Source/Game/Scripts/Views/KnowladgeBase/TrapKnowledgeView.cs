using Assets.Source.Game.Scripts.ScriptableObjects;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class TrapKnowledgeView : KnowledgeView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private LeanLocalizedText _name;
        [SerializeField] private LeanLocalizedText _description;

        public void Initialize(TrapData trapData)
        {
            _icon.sprite = trapData.Icon;
            _name.TranslationName = trapData.Name;
            _description.TranslationName = trapData.Description;
        }
    }
}