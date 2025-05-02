using Lean.Localization;
using UnityEngine;

public class SpawnedEnemyView : MonoBehaviour
{
    [SerializeField] private LeanLocalizedText _nameEnemy;

    public void Initialize(string name)
    {
        _nameEnemy.TranslationName = name;
    }
}