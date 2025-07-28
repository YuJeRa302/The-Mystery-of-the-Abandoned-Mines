using Assets.Source.Game.Scripts.Services;
using Reflex.Extensions;
using System;
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
        private readonly IAbilityStrategy _iAbilityStrategy;
        private readonly IClassAbilityStrategy _iClassAbilityStrategy;
        private readonly IAbilityPauseStrategy _iAbilityPauseStrategy;

        public AbilityPresenter(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            _abilityEntitiesHolder = abilityEntitiesHolder;
            _ability = _abilityEntitiesHolder.Ability;
            _abilityView = _abilityEntitiesHolder.AbilityView;
            _iAbilityStrategy = _abilityEntitiesHolder.IAbilityStrategy;

            _iClassAbilityStrategy = _abilityEntitiesHolder.IAbilityStrategy is IClassAbilityStrategy ?
            _abilityEntitiesHolder.IAbilityStrategy as IClassAbilityStrategy : null;

            _iAbilityPauseStrategy = _abilityEntitiesHolder.IAbilityStrategy is IAbilityPauseStrategy ?
            _abilityEntitiesHolder.IAbilityStrategy as IAbilityPauseStrategy : null;

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
            GC.SuppressFinalize(this);
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

            if (_iClassAbilityStrategy != null)
                _iClassAbilityStrategy.AddListener();
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

            if (_iClassAbilityStrategy != null)
                _iClassAbilityStrategy.RemoveListener();
        }

        private void OnAbilityUsed(Ability ability)
        {
            _iAbilityStrategy.UsedAbility(_ability);
        }

        private void OnAbilityEnded(Ability ability)
        {
            _iAbilityStrategy.EndedAbility(_ability);
        }

        private void OnCooldownValueChanged(float value)
        {
            _abilityView.ChangeCooldownValue(value);
        }

        private void OnCooldownValueReset(float value)
        {
            _abilityView.ResetCooldownValue(value);

            if (_iClassAbilityStrategy != null)
                _iClassAbilityStrategy.SetInteractableButton();
        }

        private void OnGamePaused(bool state)
        {
            _ability.StopCoroutine();

            if (_iAbilityPauseStrategy != null)
                _iAbilityPauseStrategy.PausedGame(state);
        }

        private void OnGameResumed(bool state)
        {
            _ability.ResumeCoroutine();

            if (_iAbilityPauseStrategy != null)
                _iAbilityPauseStrategy.ResumedGame(state);
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