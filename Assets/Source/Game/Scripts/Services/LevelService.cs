using Assets.Source.Game.Scripts.Levels;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Services
{
    [Serializable]
    public class LevelService
    {
        [SerializeField] private int _currentLevelId;
        [SerializeField] private List<LevelState> _levelStates = new();

        public int CurrentLevelId => _currentLevelId;
        public List<LevelState> LevelStates => _levelStates;

        public void SetLevelId(int id) => _currentLevelId = id;

        public void SetLevelStates(LevelState[] levelStates)
        {
            for (int index = 0; index < levelStates.Length; index++)
            {
                _levelStates.Add(levelStates[index]);
            }
        }

        public void AddLevelState(int id, bool isComplete)
        {
            LevelState levelState = GetLevelStateById(id);

            if (levelState.IsComplete == true)
                return;
            else
                levelState.SetCompleteLevelStatus(isComplete);
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
            if (_levelStates != null)
            {
                foreach (LevelState levelState in _levelStates)
                {
                    if (levelState.Id == id)
                        return levelState;
                }
            }

            return null;
        }

        private LevelState InitLevelState(LevelData levelData)
        {
            LevelState levelState = new(levelData.Id, false);

            _levelStates.Add(levelState);
            return levelState;
        }
    }
}