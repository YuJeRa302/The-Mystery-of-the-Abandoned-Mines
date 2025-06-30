using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class ShildUpAbilityPresenter : AbilityPresenter
{
    private Coroutine _coroutine;
    private Transform _effectConteiner;
    private Pool _pool;
    private PoolParticle _poolParticle;
    private List<PoolObject> _spawnedEffects = new();
    private bool _isAbilityUse;

    public ShildUpAbilityPresenter(Ability ability,
        AbilityView abilityView,
        Player player,
        IGameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner, PoolParticle abilityEffect) : base(ability, abilityView, player, gameLoopService, coroutineRunner)
    {
        _pool = _player.Pool;
        _poolParticle = abilityEffect;
        _effectConteiner = _player.PlayerAbilityContainer;
        AddListener();
    }

    protected override void AddListener()
    {
        base.AddListener();
        (_abilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
    }

    protected override void RemoveListener()
    {
        base.RemoveListener();
        (_abilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        if (_coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);

        _isAbilityUse = false;
        ChandeAbilityEffect(_isAbilityUse);
        _player.PlayerAnimation.UsedAbilityEnd();
    }

    private void OnButtonSkillClick()
    {
        if (_isAbilityUse)
            return;

        _isAbilityUse = true;
        _ability.Use();
        (_abilityView as ClassSkillButtonView).SetInerectableButton(false);
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        _isAbilityUse = true;
        UpShield();
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
        if (_isAbilityUse || _ability.IsAbilityUsed)
            base.OnGameResumed(state);
    }

    private void UpShield()
    {
        _player.PlayerAnimation.UseCoverAbility();
    }

    protected override void OnCooldownValueReseted(float value)
    {
        base.OnCooldownValueReseted(value);
        (_abilityView as ClassSkillButtonView).SetInerectableButton(true);
    }
}