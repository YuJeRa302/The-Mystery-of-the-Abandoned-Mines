using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;

namespace Assets.Source.Game.Scripts.Factories
{
    public class AbilityFactory
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public AbilityFactory(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public Ability CreateAbility(ActiveAbilityData abilityAttributeData,
            int currentLevel, float abilityCooldownReduction,
            float abilityDuration, int abilityValue,
            bool isAutoCast)
        {
            Ability ability = new(abilityAttributeData,
                currentLevel, abilityCooldownReduction,
                abilityDuration, abilityValue,
                isAutoCast, _coroutineRunner);
            return ability;
        }

        public Ability CreateClassSkill(ClassAbilityData classAbilityData, bool isAutoCast, int currentLvl)
        {
            Ability ability = new Ability(classAbilityData, isAutoCast, currentLvl, _coroutineRunner);
            return ability;
        }

        public Ability CreateLegendaryAbility(
            LegendaryAbilityData legendaryAbilityData,
            ActiveAbilityData abilityAttributeData,
            float abilityCooldownReduction,
            float abilityDuration,
            int abilityValue,
            bool isAutoCast)
        {
            Ability ability = new(legendaryAbilityData,
                abilityAttributeData,
                abilityCooldownReduction,
                abilityDuration, abilityValue,
                isAutoCast, _coroutineRunner);
            return ability;
        }
    }
}