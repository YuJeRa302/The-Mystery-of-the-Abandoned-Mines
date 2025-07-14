using Assets.Source.Game.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/PlayerClassnow", order = 51)]
    public class PlayerClassKnowledgeData : KnowledgeData
    {
        [SerializeField] private List<PlayerClassData> _playerClassDatas;

        public override void GetView(Transform conteiner, out List<KnowledgeView> knowladgeViews)
        {
            knowladgeViews = new();
            PlayerClassKnowledgeView playerClassKnowladgeView;

            foreach (PlayerClassData classData in _playerClassDatas)
            {
                playerClassKnowladgeView = Instantiate(_knowladgeView as PlayerClassKnowledgeView, conteiner);
                playerClassKnowladgeView.Initialize(classData);
                knowladgeViews.Add(playerClassKnowladgeView);
            }
        }
    }
}