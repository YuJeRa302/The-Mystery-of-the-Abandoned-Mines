using Assets.Source.Game.Scripts.AbilityScripts;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_AbilityEnd
    {
        private Ability _ability;

        public M_AbilityEnd(Ability ability)
        {
            _ability = ability;
        }
        public Ability Ability => _ability;
    }
}