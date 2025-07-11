using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatsView : MonoBehaviour
{
    [SerializeField] private LeanLocalizedText _nameStats;
    [SerializeField] private Text _value;

    public void Initialize(string nameStats, string value)
    {
        _nameStats.TranslationName = nameStats;
        _value.text = value;
    }
}