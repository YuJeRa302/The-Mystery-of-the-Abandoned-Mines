using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/Enemy", order = 51)]
    public class EnemyKniwledgeBaseData : KnowledgeData
    {
        [SerializeField] private List<EnemyData> _enemyDatas;

        public override void GetView(Transform conteiner, out List<KnowledgeView> knowladgeViews)
        {
            knowladgeViews = new();
            EnemyKnowledgeView enemyKnowladgeView;

            foreach (EnemyData EnemyData in _enemyDatas)
            {
                enemyKnowladgeView = Instantiate(KnowledgeView as EnemyKnowledgeView, conteiner);
                enemyKnowladgeView.Initialize(EnemyData);
                knowladgeViews.Add(enemyKnowladgeView);
            }
        }
    }
}