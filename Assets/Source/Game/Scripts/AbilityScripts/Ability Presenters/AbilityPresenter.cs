using Assets.Source.Game.Scripts.Services;
using Reflex.Extensions;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public sealed class AbilityPresenter
    {
        private readonly AbilityEntitiesHolder _abilityEntitiesHolder;
        private readonly GamePauseService _gamePauseService;
        private readonly GameLoopService _gameLoopService;
        private readonly Ability _ability;
        private readonly AbilityView _abilityView;
        private readonly IAbilityStrategy _abilityStrategy;
        private readonly IClassAbilityStrategy _classAbilityStrategy;
        private readonly IAbilityPauseStrategy _abilityPauseStrategy;

        public AbilityPresenter(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            _abilityEntitiesHolder = abilityEntitiesHolder;
            _ability = _abilityEntitiesHolder.Ability;
            _abilityView = _abilityEntitiesHolder.AbilityView;
            _abilityStrategy = _abilityEntitiesHolder.IAbilityStrategy;
            _classAbilityStrategy = _abilityEntitiesHolder.IAbilityStrategy as IClassAbilityStrategy;
            _abilityPauseStrategy = _abilityEntitiesHolder.IAbilityStrategy as IAbilityPauseStrategy;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _gamePauseService = container.Resolve<GamePauseService>();
            _gameLoopService = container.Resolve<GameLoopService>();
            AddListener();
        }

        public void Dispose()
        {
            if (_abilityView != null)
                _abilityView.ViewDestroy();

            RemoveListener();
        }

        private void AddListener()
        {
            _ability.AbilityUsed += OnAbilityUsed;
            _ability.AbilityEnded += OnAbilityEnded;
            _ability.AbilityUpgraded += OnAbilityUpgraded;
            _ability.CooldownValueChanged += OnCooldownValueChanged;
            _ability.CooldownValueReseted += OnCooldownValueReset;
            _ability.AbilityRemoved += Dispose;
            _gamePauseService.GamePaused += OnGamePaused;
            _gamePauseService.GameResumed += OnGameResumed;
            _gameLoopService.GameClosed += OnGameClosed;

            if (_classAbilityStrategy != null)
                _classAbilityStrategy.AddListener();
        }

        private void RemoveListener()
        {
            _ability.AbilityUsed -= OnAbilityUsed;
            _ability.AbilityEnded -= OnAbilityEnded;
            _ability.AbilityUpgraded -= OnAbilityUpgraded;
            _ability.CooldownValueChanged -= OnCooldownValueChanged;
            _ability.CooldownValueReseted -= OnCooldownValueReset;
            _ability.AbilityRemoved -= Dispose;
            _gamePauseService.GamePaused -= OnGamePaused;
            _gamePauseService.GameResumed -= OnGameResumed;
            _gameLoopService.GameClosed -= OnGameClosed;

            if (_classAbilityStrategy != null)
                _classAbilityStrategy.RemoveListener();
        }

        private void OnAbilityUsed(Ability ability)
        {
            _abilityStrategy.UsedAbility(_ability);
        }

        private void OnAbilityEnded(Ability ability)
        {
            _abilityStrategy.EndedAbility(_ability);
        }

        private void OnCooldownValueChanged(float value)
        {
            _abilityView.ChangeCooldownValue(value);
        }

        private void OnCooldownValueReset(float value)
        {
            _abilityView.ResetCooldownValue(value);

            if (_classAbilityStrategy != null)
                _classAbilityStrategy.SetInteractableButton();
        }

        private void OnGamePaused(bool state)
        {
            _ability.StopCoroutine();

            if (_abilityPauseStrategy != null)
                _abilityPauseStrategy.PausedGame(state);
        }

        private void OnGameResumed(bool state)
        {
            _ability.ResumeCoroutine();

            if (_abilityPauseStrategy != null)
                _abilityPauseStrategy.ResumedGame(state);
        }

        private void OnGameClosed()
        {
            Dispose();
        }

        private void OnAbilityUpgraded(float delay)
        {
            _abilityView.Upgrade(delay);
        }
    }
}