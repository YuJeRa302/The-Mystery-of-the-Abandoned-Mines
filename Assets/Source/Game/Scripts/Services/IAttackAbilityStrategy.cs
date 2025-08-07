using Assets.Source.Game.Scripts.AbilityScripts;

namespace Assets.Source.Game.Scripts.Services
{
    public interface IAttackAbilityStrategy
    {
        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder);
        public void Create();
    }
}