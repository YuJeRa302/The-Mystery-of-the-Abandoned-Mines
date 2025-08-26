using Assets.Source.Game.Scripts.ScriptableObjects;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_LegendaryAbilityTake
    {
        private ActiveAbilityData _abilityData;

        public M_LegendaryAbilityTake(ActiveAbilityData abilityData)
        {
            _abilityData = abilityData;
        }

        public ActiveAbilityData ActiveAbilityData => _abilityData;
    }
}