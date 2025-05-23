using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New KnowledgeBaseData", menuName = "Create KnowledgeBase/GameMode", order = 51)]
public class GameModeKnowledgeData : KnowledgeData
{
    [SerializeField] private List<GameModData> _gameModeData;

    public override void GetView(Transform conteiner, out List<KnowladgeView> knowladgeViews)
    {
        knowladgeViews = new();
        GameModeKnowledgeBaseView gameModeKnowladgeView;

        foreach (GameModData gameMode in _gameModeData)
        {
            gameModeKnowladgeView = Instantiate(_knowladgeView as GameModeKnowledgeBaseView, conteiner);
            gameModeKnowladgeView.Initialize(gameMode);
            knowladgeViews.Add(gameModeKnowladgeView);
        }
    }
}