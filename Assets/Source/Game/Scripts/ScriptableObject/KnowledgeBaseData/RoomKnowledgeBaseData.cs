using Assets.Source.Game.Scripts.Rooms;
using Assets.Source.Game.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/Room", order = 51)]
    public class RoomKnowledgeBaseData : KnowledgeData
    {
        [SerializeField] private List<RoomData> _roomDatas;

        public override void GetView(Transform conteiner, out List<KnowledgeView> knowladgeViews)
        {
            knowladgeViews = new();
            RoomKnowledgeView roomKnowladgeView;

            foreach (RoomData roomData in _roomDatas)
            {
                roomKnowladgeView = Instantiate(_knowladgeView as RoomKnowledgeView, conteiner);
                roomKnowladgeView.Initialize(roomData);
                knowladgeViews.Add(roomKnowladgeView);
            }
        }
    }
}