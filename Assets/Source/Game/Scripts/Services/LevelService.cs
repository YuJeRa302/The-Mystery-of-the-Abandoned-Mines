using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;

[Serializable]
public class LevelService
{
    public int CurrentLevelId;
    public List<LevelState> LevelStates = new ();

    public void SetLevelStates(LevelState[] levelStates) 
    {
        for (int index = 0; index < levelStates.Length; index++)
        {
            LevelStates.Add(levelStates[index]);
        }
    }

    public void AddLevelState(int id, bool isComplete) 
    {
        LevelState levelState = GetLevelStateById(id);

        if (levelState.IsComplete == true)
            return;
        else
            levelState.IsComplete = isComplete;
    }

    public LevelState GetLevelStateByLevelData(LevelData levelData)
    {
        LevelState levelState = GetLevelStateById(levelData.Id);

        if (levelState == null)
            levelState = InitLevelState(levelData);

        return levelState;
    }

    public LevelState GetLevelStateById(int id)
    {
        if (LevelStates != null)
        {
            foreach (LevelState levelState in LevelStates)
            {
                if (levelState.Id == id)
                    return levelState;
            }
        }

        return null;
    }

    private LevelState InitLevelState(LevelData levelData)
    {
        LevelState levelState = new ()
        {
            Id = levelData.Id,
            IsComplete = false,
            CurrentCompleteStages = 0,
            Tier = levelData.Tier
        };

        LevelStates.Add(levelState);
        return levelState;
    }
}
