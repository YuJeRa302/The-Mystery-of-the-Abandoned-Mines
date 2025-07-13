using Lean.Localization;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Views
{
    public class SpawnedEnemyView : MonoBehaviour
    {
        [SerializeField] private LeanLocalizedText _nameEnemy;

        public void Initialize(string name)
        {
            _nameEnemy.TranslationName = name;
        }
    }
}