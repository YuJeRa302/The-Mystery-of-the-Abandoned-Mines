using Assets.Source.Game.Scripts.Views;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_PassiveAbilityViewCreat
    {
        private PassiveAbilityView _passiveAbilityView;

        public M_PassiveAbilityViewCreat(PassiveAbilityView passiveAbilityView)
        {
            _passiveAbilityView = passiveAbilityView;
        }

        public PassiveAbilityView PassiveAbilityView => _passiveAbilityView;
    }
}