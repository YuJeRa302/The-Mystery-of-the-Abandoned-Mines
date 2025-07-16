using Assets.Source.Game.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/Card", order = 51)]
    public class CardKnowledgeData : KnowledgeData
    {
        [SerializeField] private List<CardData> _cardDatas;

        public override void GetView(Transform conteiner, out List<KnowledgeView> knowladgeViews)
        {
            knowladgeViews = new();
            CardKnowledgebaseView cardKnowladgeView;

            foreach (CardData cardData in _cardDatas)
            {
                cardKnowladgeView = Instantiate(KnowledgeView as CardKnowledgebaseView, conteiner);
                cardKnowladgeView.Initialize(cardData);
                knowladgeViews.Add(cardKnowladgeView);
            }
        }
    }
}