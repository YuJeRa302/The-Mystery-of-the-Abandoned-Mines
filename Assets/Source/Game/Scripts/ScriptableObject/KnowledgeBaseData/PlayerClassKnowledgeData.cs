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
            PlayerClassKnowledgeView playerClassKnowledgeView;

            foreach (PlayerClassData classData in _playerClassDatas)
            {
                playerClassKnowledgeView = Instantiate(KnowledgeView as PlayerClassKnowledgeView, conteiner);
                playerClassKnowledgeView.Initialize(classData);
                knowladgeViews.Add(playerClassKnowledgeView);
            }
        }
    }
}