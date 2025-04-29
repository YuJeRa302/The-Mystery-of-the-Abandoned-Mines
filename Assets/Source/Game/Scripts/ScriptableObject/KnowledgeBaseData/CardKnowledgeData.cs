using Assets.Source.Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/Card", order = 51)]
public class CardKnowledgeData : KnowledgeData
{
    [SerializeField] private List<CardData> _cardDatas;

    public override void GetView(Transform conteiner, out List<KnowladgeView> knowladgeViews)
    {
        knowladgeViews = new();
        CardKnowledgebaseView cardKnowladgeView;

        foreach (CardData cardData in _cardDatas)
        {
            cardKnowladgeView = Instantiate(_knowladgeView as CardKnowledgebaseView, conteiner);
            cardKnowladgeView.Initialize(cardData);
            knowladgeViews.Add(cardKnowladgeView);
        }
    }
}