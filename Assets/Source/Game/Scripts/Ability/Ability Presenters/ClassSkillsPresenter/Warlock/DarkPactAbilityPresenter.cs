using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class DarkPactAbilityPresenter : AbilityPresenter
{
    private Coroutine _coroutine;
    private Transform _effectConteiner;
    private Pool _pool;
    private PoolParticle _poolParticle;
    private List<PoolObject> _spawnedEffects = new ();
    private bool _isAbilityUse;

    public DarkPactAbilityPresenter(
        Ability ability,
        AbilityView abilityView,
        Player player,
        GamePauseService gamePauseService,
        GameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner,
        PoolParticle abilityEffect) : base(ability, abilityView, player, gamePauseService, gameLoopService, coroutineRunner)
    {
        _pool = Player.Pool;
        _poolParticle = abilityEffect;
        _effectConteiner = Player.PlayerAbilityContainer;
        AddListener();
    }

    protected override void AddListener()
    {
        base.AddListener();
        (AbilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
    }

    protected override void RemoveListener()
    {
        base.RemoveListener();
        (AbilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        if (_coroutine != null)
            CoroutineRunner.StopCoroutine(_coroutine);

        _isAbilityUse = false;
        ChandeAbilityEffect(_isAbilityUse);
    }

    private void OnButtonSkillClick()
    {
        if (_isAbilityUse)
            return;

        _isAbilityUse = true;
        Ability.Use();
        (AbilityView as ClassSkillButtonView).SetInerectableButton(false);
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        _isAbilityUse = true;
        ChandeAbilityEffect(_isAbilityUse);
    }

    private void ChandeAbilityEffect(bool isAbilityEnded)
    {
        if (isAbilityEnded)
        {
            PoolParticle particle;

            if (_pool.TryPoolObject(_poolParticle.gameObject, out PoolObject pollParticle))
            {
                particle = pollParticle as PoolParticle;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = GameObject.Instantiate(_poolParticle, _effectConteiner);
                _pool.InstantiatePoolObject(particle, _poolParticle.name);
                _spawnedEffects.Add(particle);
            }
        }
        else
        {
            foreach (var patricle in _spawnedEffects)
            {
                if (patricle.isActiveAndEnabled)
                    patricle.ReturnObjectPool();
            }
        }
    }

    protected override void OnGameResumed(bool state)
    {
        if (_isAbilityUse || Ability.IsAbilityUsed)
            base.OnGameResumed(state);
    }

    protected override void OnCooldownValueReseted(float value)
    {
        base.OnCooldownValueReseted(value);
        (AbilityView as ClassSkillButtonView).SetInerectableButton(true);
    }
}