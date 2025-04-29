using System;
using UnityEngine;

[Serializable]
public class SubcategoriesView
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private KnowledgeData _knowledgeData;

    public string Name => _name;
    public Sprite Icon => _icon;
    public KnowledgeData KnowledgeData => _knowledgeData;
}