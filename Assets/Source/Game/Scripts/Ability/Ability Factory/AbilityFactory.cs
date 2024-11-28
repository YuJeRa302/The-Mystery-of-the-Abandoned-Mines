namespace Assets.Source.Game.Scripts
{
    public class AbilityFactory
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public AbilityFactory(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public Ability Create(AbilityAttributeData abilityAttributeData, int currentLevel, float abilityCooldownReduction, float abilityDuration, int abilityValue)
        {
            Ability ability = new(abilityAttributeData, currentLevel, abilityCooldownReduction, abilityDuration, abilityValue, _coroutineRunner);
            return ability;
        }
    }
}