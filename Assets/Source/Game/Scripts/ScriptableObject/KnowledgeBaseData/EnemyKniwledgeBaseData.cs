using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/Enemy", order = 51)]
public class EnemyKniwledgeBaseData : KnowledgeData
{
    [SerializeField] private List<EnemyData> _enemyDatas;

    public override void GetView(Transform conteiner, out List<KnowladgeView> knowladgeViews)
    {
        knowladgeViews = new();
        EnemyKnowladgeView enemyKnowladgeView;

        foreach (EnemyData EnemyData in _enemyDatas)
        {
            enemyKnowladgeView = Instantiate(_knowladgeView as EnemyKnowladgeView, conteiner);
            enemyKnowladgeView.Initialize(EnemyData);
            knowladgeViews.Add(enemyKnowladgeView);
        }
    }
}