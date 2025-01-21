namespace Assets.Source.Game.Scripts
{
    public class AbilityFactory
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public AbilityFactory(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public Ability Create(AbilityAttributeData abilityAttributeData, int currentLevel, float abilityCooldownReduction, float abilityDuration, int abilityValue, bool isAutoCast)
        {
            Ability ability = new(abilityAttributeData, currentLevel, abilityCooldownReduction, abilityDuration, abilityValue, isAutoCast, _coroutineRunner);
            return ability;
        }

        public Ability CreateClassSkill(ClassAbilityData classAbilityData, bool isAutoCast)
        {
            Ability ability = new Ability(classAbilityData, isAutoCast, _coroutineRunner);
            return ability;
        }
    }
}