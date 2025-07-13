using Assets.Source.Game.Scripts.States;
using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts.Services
{
    [Serializable]
    public class ClassAbilityService
    {
        public List<ClassAbilityState> ClassAbilityStates = new();

        public void SetClassAbilityStates(List<ClassAbilityState> classAbilityStates)
        {
            ClassAbilityStates = classAbilityStates;
        }

        public void SetClassAbilityState(ClassAbilityState newClassAbilityState)
        {
            if (ClassAbilityStates != null)
            {
                for (int index = 0; index < ClassAbilityStates.Count; index++)
                {
                    if (ClassAbilityStates[index].Id == newClassAbilityState.Id)
                        ClassAbilityStates[index].CurrentLevel = newClassAbilityState.CurrentLevel;
                    else
                        ClassAbilityStates.Add(new(newClassAbilityState.Id, newClassAbilityState.CurrentLevel));
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
            if (ClassAbilityStates != null)
            {
                foreach (ClassAbilityState classAbilityState in ClassAbilityStates)
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
            ClassAbilityStates.Add(classAbilityState);
            return classAbilityState;
        }
    }
}