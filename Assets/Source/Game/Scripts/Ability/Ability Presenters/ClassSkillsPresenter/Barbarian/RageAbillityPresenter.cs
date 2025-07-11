using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class RageAbillityPresenter : AbilityPresenter
{
    private List<Transform> _effectConteiner = new List<Transform> ();
    private List<PoolObject> _spawnedEffects = new ();
    private Pool _pool;
    private PoolParticle _particleEffectPrefab;
    private bool _isAbilityUse;

    public RageAbillityPresenter(
        Ability ability,
        AbilityView abilityView,
        Player player,
        GamePauseService gamePauseService,
        GameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner,
        PoolParticle abilityEffect) : base(ability, abilityView, player, gamePauseService, gameLoopService, coroutineRunner)
    {
        _pool = Player.Pool;
        _particleEffectPrefab = abilityEffect;
        _effectConteiner.Add(Player.WeaponPoint);
        _effectConteiner.Add(Player.AdditionalWeaponPoint);

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
        _isAbilityUse = false;
        BoostPlayer(_isAbilityUse);
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
        BoostPlayer(_isAbilityUse);
    }

    private void BoostPlayer(bool isAbilityEnded)
    {
        foreach (var conteiner in _effectConteiner)
        {
            ChangeEffectEneble(isAbilityEnded, conteiner);
        }
    }

    private void ChangeEffectEneble(bool isAbilityEnded, Transform conteiner)
    {
        if (isAbilityEnded)
        {
            PoolParticle particle;

            if (_pool.TryPoolObject(_particleEffectPrefab.gameObject, out PoolObject pollParticle))
            {
                particle = pollParticle as PoolParticle;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = GameObject.Instantiate(_particleEffectPrefab, conteiner);
                _pool.InstantiatePoolObject(particle, _particleEffectPrefab.name);
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

    protected override void OnCooldownValueReseted(float value)
    {
        base.OnCooldownValueReseted(value);
        (AbilityView as ClassSkillButtonView).SetInerectableButton(true);
    }

    protected override void OnGameResumed(bool state)
    {
        if (_isAbilityUse || Ability.IsAbilityUsed)
            base.OnGameResumed(state);
    }
}