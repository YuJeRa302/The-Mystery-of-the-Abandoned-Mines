using Assets.Source.Game.Scripts;
using System;

public abstract class AbilityPresenter : IDisposable
{
    protected readonly ICoroutineRunner _coroutineRunner;
    protected readonly GamePauseService _gamePauseService;
    protected readonly GameLoopService _gameLoopService;

    protected Ability _ability;
    protected AbilityView _abilityView;
    protected Player _player;

    public AbilityPresenter(
        Ability ability,
        AbilityView abilityView,
        Player player,
        GamePauseService gamePauseService,
        GameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner)
    {
        _ability = ability;
        _abilityView = abilityView;
        _player = player;
        _coroutineRunner = coroutineRunner;
        _gamePauseService = gamePauseService;
        _gameLoopService = gameLoopService;
    }

    public void Dispose()
    {
        if (_abilityView != null)
            _abilityView.ViewDestroy();

        RemoveListener();
        GC.SuppressFinalize(this);
    }

    protected virtual void AddListener()
    {
        _ability.AbilityUsed += OnAbilityUsed;
        _ability.AbilityEnded += OnAbilityEnded;
        _ability.AbilityUpgraded += OnAbilityUpgraded;
        _ability.CooldownValueChanged += OnCooldownValueChanged;
        _ability.CooldownValueReseted += OnCooldownValueReseted;
        _ability.AbilityRemoved += Dispose;
        _gamePauseService.GamePaused += OnGamePaused;
        _gamePauseService.GameResumed += OnGameResumed;
        _gameLoopService.GameClosed += OnGameClosed;
    }

    protected virtual void RemoveListener()
    {
        _ability.AbilityUsed -= OnAbilityUsed;
        _ability.AbilityEnded -= OnAbilityEnded;
        _ability.AbilityUpgraded -= OnAbilityUpgraded;
        _ability.CooldownValueChanged -= OnCooldownValueChanged;
        _ability.CooldownValueReseted -= OnCooldownValueReseted;
        _ability.AbilityRemoved -= Dispose;
        _gamePauseService.GamePaused -= OnGamePaused;
        _gamePauseService.GameResumed -= OnGameResumed;
        _gameLoopService.GameClosed -= OnGameClosed;
    }

    protected abstract void OnAbilityUsed(Ability ability);

    protected abstract void OnAbilityEnded(Ability ability);

    protected virtual void OnCooldownValueChanged(float value)
    {
        _abilityView.ChangeCooldownValue(value);
    }

    protected virtual void OnCooldownValueReseted(float value)
    {
        _abilityView.ResetCooldownValue(value);
    }

    protected virtual void OnGamePaused(bool state)
    {
        _ability.StopCoroutine();
    }
    
    protected virtual void OnGameResumed(bool state)
    {
        _ability.ResumeCoroutine();
    }

    protected virtual void OnGameClosed()
    {
        Dispose();
    }

    protected virtual void OnAbilityUpgraded(float delay)
    {
        _abilityView.Upgrade(delay);
    }
}