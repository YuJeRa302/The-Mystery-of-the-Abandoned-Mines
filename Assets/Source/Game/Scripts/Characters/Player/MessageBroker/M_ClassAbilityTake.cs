using Assets.Source.Game.Scripts.ScriptableObjects;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_ClassAbilityTake
    {
        private ClassAbilityData _classAbilityData;
        private int _currentLvl;

        public M_ClassAbilityTake(ClassAbilityData classAbilityData, int currentLvl)
        {
            _classAbilityData = classAbilityData;
            _currentLvl = currentLvl;
        }

        public ClassAbilityData ClassAbilityData => _classAbilityData;
        public int CurrentLvl => _currentLvl;
    }
}