using Assets.Source.Game.Scripts.AbilityScripts;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_AbilityUse
    {
        private Ability _ability;

        public M_AbilityUse(Ability ability)
        {
            _ability = ability;
        }
        public Ability Ability => _ability;
    }
}