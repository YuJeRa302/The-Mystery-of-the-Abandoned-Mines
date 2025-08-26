using Assets.Source.Game.Scripts.ScriptableObjects;

namespace Assets.Source.Game.Scripts.GamePanels
{
    public struct M_PassiveAbilityTake
    {
        private PassiveAttributeData _passiveAttributeData;

        public M_PassiveAbilityTake(PassiveAttributeData passiveAttributeData)
        {
            _passiveAttributeData = passiveAttributeData;
        }

        public PassiveAttributeData PassiveAttributeData => _passiveAttributeData;
    }
}