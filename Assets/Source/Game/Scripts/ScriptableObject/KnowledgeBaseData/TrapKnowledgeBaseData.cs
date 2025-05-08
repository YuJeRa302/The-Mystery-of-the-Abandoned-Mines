using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/Trap", order = 51)]
public class TrapKnowledgeBaseData : KnowledgeData
{
    [SerializeField] private List<TrapData> _trapDatas;

    public override void GetView(Transform conteiner, out List<KnowladgeView> knowladgeViews)
    {
        knowladgeViews = new();
        TrapKnowladgeView trapKnowladgeView;

        foreach (TrapData data in _trapDatas)
        {
            trapKnowladgeView = Instantiate(_knowladgeView as TrapKnowladgeView, conteiner);
            trapKnowladgeView.Initialize(data);
            knowladgeViews.Add(trapKnowladgeView);
        }
    }
}