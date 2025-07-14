using Assets.Source.Game.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/Trap", order = 51)]
    public class TrapKnowledgeBaseData : KnowledgeData
    {
        [SerializeField] private List<TrapData> _trapDatas;

        public override void GetView(Transform conteiner, out List<KnowledgeView> knowladgeViews)
        {
            knowladgeViews = new();
            TrapKnowledgeView trapKnowladgeView;

            foreach (TrapData data in _trapDatas)
            {
                trapKnowladgeView = Instantiate(_knowladgeView as TrapKnowledgeView, conteiner);
                trapKnowladgeView.Initialize(data);
                knowladgeViews.Add(trapKnowladgeView);
            }
        }
    }
}