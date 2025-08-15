using Assets.Source.Game.Scripts.States;

namespace Assets.Source.Game.Scripts.Models
{
    public struct M_AbilityUpgraded
    {
        private ClassAbilityState _classAbilityState;

        public M_AbilityUpgraded(ClassAbilityState classAbilityState)
        {
            _classAbilityState = classAbilityState;
        }

        public ClassAbilityState ClassAbilityState => _classAbilityState;
    }
}