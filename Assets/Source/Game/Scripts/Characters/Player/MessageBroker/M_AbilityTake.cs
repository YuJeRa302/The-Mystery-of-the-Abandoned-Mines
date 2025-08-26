using Assets.Source.Game.Scripts.ScriptableObjects;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_AbilityTake
    {
        private ActiveAbilityData _activeAbilityData;
        private int _currentLvl;
        
        public M_AbilityTake(ActiveAbilityData activeAbilityData, int currentLvl)
        {
            _activeAbilityData = activeAbilityData;
            _currentLvl = currentLvl;
        }

        public ActiveAbilityData ActiveAbilityData => _activeAbilityData;
        public int CurrentLvl => _currentLvl;
    }
}