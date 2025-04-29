using System.Collections.Generic;
using UnityEngine;

public abstract class KnowledgeData : ScriptableObject
{
    [SerializeField] protected KnowladgeView _knowladgeView;

    public abstract void GetView(Transform conteiner, out List<KnowladgeView> knowladgeViews);
}