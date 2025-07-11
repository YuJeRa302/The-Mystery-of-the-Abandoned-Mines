using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/Room", order = 51)]
public class RoomKnowledgeBaseData : KnowledgeData
{
    [SerializeField] private List<RoomData> _roomDatas;

    public override void GetView(Transform conteiner, out List<KnowladgeView> knowladgeViews)
    {
        knowladgeViews = new();
        RoomKnowladgeView roomKnowladgeView;

        foreach (RoomData roomData in _roomDatas)
        {
            roomKnowladgeView = Instantiate(_knowladgeView as RoomKnowladgeView, conteiner);
            roomKnowladgeView.Initialize(roomData);
            knowladgeViews.Add(roomKnowladgeView);
        }
    }
}