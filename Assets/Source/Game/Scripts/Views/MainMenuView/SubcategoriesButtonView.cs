using Lean.Localization;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SubcategoriesButtonView : MonoBehaviour
{
    [SerializeField] private LeanLocalizedText _nameStats;
    [SerializeField] private Image _icon;

    private Button _button;
    private KnowledgeData _knowledgeData;

    public event Action<KnowledgeData> ChengetCategoru;

    public KnowledgeData KnowledgeData => _knowledgeData;

    public void Initialize(string nameCategori, Sprite icon, KnowledgeData knowledgeData)
    {
        _nameStats.TranslationName = nameCategori;
        _icon.sprite = icon;
        _knowledgeData = knowledgeData;
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => ChengetCategoru?.Invoke(_knowledgeData));
    }
}