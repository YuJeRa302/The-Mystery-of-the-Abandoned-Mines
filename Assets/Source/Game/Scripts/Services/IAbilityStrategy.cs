using Assets.Source.Game.Scripts.AbilityScripts;

namespace Assets.Source.Game.Scripts.Services
{
    public interface IAbilityStrategy
    {
        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder);
        public void UsedAbility(Ability ability);
        public void EndedAbility(Ability ability);
    }
}