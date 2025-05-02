using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
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