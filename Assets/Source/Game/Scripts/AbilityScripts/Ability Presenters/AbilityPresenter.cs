using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Services;
using System;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public abstract class AbilityPresenter : IDisposable
    {
        protected readonly ICoroutineRunner CoroutineRunner;
        protected readonly GamePauseService GamePauseService;
        protected readonly GameLoopService GameLoopService;

        protected Ability Ability;
        protected AbilityView AbilityView;
        protected Player Player;

        public AbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            GamePauseService gamePauseService,
            GameLoopService gameLoopService,
            ICoroutineRunner coroutineRunner)
        {
            Ability = ability;
            AbilityView = abilityView;
            Player = player;
            CoroutineRunner = coroutineRunner;
            GamePauseService = gamePauseService;
            GameLoopService = gameLoopService;
        }

        public void Dispose()
        {
            if (AbilityView != null)
                AbilityView.ViewDestroy();

            RemoveListener();
            GC.SuppressFinalize(this);
        }

        protected virtual void AddListener()
        {
            Ability.AbilityUsed += OnAbilityUsed;
            Ability.AbilityEnded += OnAbilityEnded;
            Ability.AbilityUpgraded += OnAbilityUpgraded;
            Ability.CooldownValueChanged += OnCooldownValueChanged;
            Ability.CooldownValueReseted += OnCooldownValueReset;
            Ability.AbilityRemoved += Dispose;
            GamePauseService.GamePaused += OnGamePaused;
            GamePauseService.GameResumed += OnGameResumed;
            GameLoopService.GameClosed += OnGameClosed;
        }

        protected virtual void RemoveListener()
        {
            Ability.AbilityUsed -= OnAbilityUsed;
            Ability.AbilityEnded -= OnAbilityEnded;
            Ability.AbilityUpgraded -= OnAbilityUpgraded;
            Ability.CooldownValueChanged -= OnCooldownValueChanged;
            Ability.CooldownValueReseted -= OnCooldownValueReset;
            Ability.AbilityRemoved -= Dispose;
            GamePauseService.GamePaused -= OnGamePaused;
            GamePauseService.GameResumed -= OnGameResumed;
            GameLoopService.GameClosed -= OnGameClosed;
        }

        protected abstract void OnAbilityUsed(Ability ability);

        protected abstract void OnAbilityEnded(Ability ability);

        protected virtual void OnCooldownValueChanged(float value)
        {
            AbilityView.ChangeCooldownValue(value);
        }

        protected virtual void OnCooldownValueReset(float value)
        {
            AbilityView.ResetCooldownValue(value);
        }

        protected virtual void OnGamePaused(bool state)
        {
            Ability.StopCoroutine();
        }

        protected virtual void OnGameResumed(bool state)
        {
            Ability.ResumeCoroutine();
        }

        protected virtual void OnGameClosed()
        {
            Dispose();
        }

        protected virtual void OnAbilityUpgraded(float delay)
        {
            AbilityView.Upgrade(delay);
        }
    }
}