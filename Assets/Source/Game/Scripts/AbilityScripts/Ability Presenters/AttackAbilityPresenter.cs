using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class AttackAbilityPresenter : IAbilityStrategy, IAbilityPauseStrategy
    {
        private ILoopCoroutineStrategy _loopCoroutineStrategy;
        private IAttackAbilityStrategy _attackAbilityStrategy;
        private Ability _ability;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            AttackAbilityData attackAbilityData = abilityEntitiesHolder.AttributeData as AttackAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _attackAbilityStrategy = attackAbilityData.IAttackAbilityStrategy;
            _loopCoroutineStrategy = _attackAbilityStrategy as ILoopCoroutineStrategy;
            _attackAbilityStrategy.Construct(abilityEntitiesHolder);
        }

        public void UsedAbility(Ability ability)
        {
            _attackAbilityStrategy.Create();
        }

        public void EndedAbility(Ability ability)
        {
            if (_loopCoroutineStrategy != null)
                _loopCoroutineStrategy.StopCoroutine();
        }

        public void PausedGame(bool state)
        {
            if (_loopCoroutineStrategy != null)
                _loopCoroutineStrategy.StopCoroutine();
        }

        public void ResumedGame(bool state)
        {
            _ability.Use();

            if (_loopCoroutineStrategy != null)
                _loopCoroutineStrategy.StartCoroutine();
        }
    }
}