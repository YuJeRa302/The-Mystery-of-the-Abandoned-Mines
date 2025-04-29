using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/PlayerClassnow", order = 51)]
public class PlayerClassKnowledgeData : KnowledgeData
{
    [SerializeField] private List<PlayerClassData> _playerClassDatas;

    public override void GetView(Transform conteiner, out List<KnowladgeView> knowladgeViews)
    {
        knowladgeViews = new();
        PlayerClassKnowladgeView playerClassKnowladgeView;

        foreach (PlayerClassData classData in _playerClassDatas)
        {
            playerClassKnowladgeView = Instantiate(_knowladgeView as PlayerClassKnowladgeView, conteiner);
            playerClassKnowladgeView.Initialize(classData);
            knowladgeViews.Add(playerClassKnowladgeView);
        }
    }
}