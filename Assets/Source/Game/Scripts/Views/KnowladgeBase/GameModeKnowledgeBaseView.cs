using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class GameModeKnowledgeBaseView : KnowladgeView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private LeanLocalizedText _name;
        [SerializeField] private LeanLocalizedText _description;
        [SerializeField] private LeanLocalizedText _access;

        public void Initialize(GameModData gameMod)
        {
            _icon.sprite = gameMod.Icon;
            _name.TranslationName = gameMod.Name;
            _description.TranslationName = gameMod.Description;
            _access.TranslationName = gameMod.Access;
        }
    }
}