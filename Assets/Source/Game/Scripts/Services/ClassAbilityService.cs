using Assets.Source.Game.Scripts.States;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Services
{
    [Serializable]
    public class ClassAbilityService
    {
        [SerializeField] private List<ClassAbilityState> _classAbilityStates = new();

        public List<ClassAbilityState> ClassAbilityStates => _classAbilityStates;

        public void SetClassAbilityStates(List<ClassAbilityState> classAbilityStates)
        {
            _classAbilityStates = classAbilityStates;
        }

        public void SetClassAbilityState(ClassAbilityState newClassAbilityState)
        {
            if (_classAbilityStates != null)
            {
                for (int index = 0; index < _classAbilityStates.Count; index++)
                {
                    if (_classAbilityStates[index].Id == newClassAbilityState.Id)
                        _classAbilityStates[index].ChangeCurrentLevel(newClassAbilityState.CurrentLevel);
                    else
                        _classAbilityStates.Add(new(newClassAbilityState.Id, newClassAbilityState.CurrentLevel));
                }
            }
        }

        public ClassAbilityState GetClassAbilityStateById(int id)
        {
            ClassAbilityState classAbilityState = FindClassAbilityStateById(id);

            if (classAbilityState == null)
                classAbilityState = InitState(id);

            return classAbilityState;
        }

        private ClassAbilityState FindClassAbilityStateById(int id)
        {
            if (_classAbilityStates != null)
            {
                foreach (ClassAbilityState classAbilityState in _classAbilityStates)
                {
                    if (classAbilityState.Id == id)
                        return classAbilityState;
                }
            }

            return null;
        }

        private ClassAbilityState InitState(int id)
        {
            ClassAbilityState classAbilityState = new(id, 0);
            _classAbilityStates.Add(classAbilityState);
            return classAbilityState;
        }
    }
}