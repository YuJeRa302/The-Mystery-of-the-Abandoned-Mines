using Assets.Source.Game.Scripts.Services;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public abstract class ClassAbilityPresenter : IAbilityStrategy, IClassAbilityStrategy
    {
        private ClassSkillButtonView _classSkillButtonView;
        private Ability _ability;

        public bool IsAbilityUse { get; private set; }

        public virtual void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            _ability = abilityEntitiesHolder.Ability;
            _classSkillButtonView = abilityEntitiesHolder.AbilityView as ClassSkillButtonView;
        }

        public virtual void UsedAbility(Ability ability)
        {
            if (IsAbilityUse == false)
                return;
        }

        public virtual void EndedAbility(Ability ability)
        {
            IsAbilityUse = false;
        }

        public void AddListener()
        {
            _classSkillButtonView.AbilityUsed += OnAbilityButtonClick;
        }

        public void RemoveListener()
        {
            _classSkillButtonView.AbilityUsed -= OnAbilityButtonClick;
        }

        public void SetInteractableButton()
        {
            _classSkillButtonView.SetInteractableButton(true);
        }

        private void OnAbilityButtonClick()
        {
            if (IsAbilityUse)
                return;

            IsAbilityUse = true;
            _ability.Use();
            _classSkillButtonView.SetInteractableButton(false);
        }
    }
}