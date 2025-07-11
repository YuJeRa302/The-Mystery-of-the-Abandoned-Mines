using Assets.Source.Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerkFrontAbillityPresenter : AbilityPresenter
{
    private Coroutine _coroutine;
    private Rigidbody _rigidbodyPlayer;
    private Transform _effectConteiner;
    private Pool _pool;
    private PoolParticle _poolParticle;
    private List<PoolObject> _spawnedEffects = new ();
    private bool _isAbilityUse;

    public JerkFrontAbillityPresenter(
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
        _rigidbodyPlayer = Player.GetComponent<Rigidbody>();
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

    private void OnButtonSkillClick()
    {
        if (_isAbilityUse)
            return;

        _isAbilityUse = true;
        Ability.Use();
        (AbilityView as ClassSkillButtonView).SetInerectableButton(false);
    }

    private void Jerk()
    {
        if (_coroutine != null)
            CoroutineRunner.StopCoroutine(_coroutine);

        _coroutine = CoroutineRunner.StartCoroutine(JerkForward());

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
                (particle as DamageParticle).Initialaze(Ability.DamageSource);
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

    private IEnumerator JerkForward()
    {
        float currentTime = 0;

        while (currentTime <= Ability.CurrentDuration)
        {
            _rigidbodyPlayer.AddForce(Player.transform.forward * Ability.CurrentAbilityValue, ForceMode.Impulse);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        ChandeAbilityEffect(_isAbilityUse);
        Jerk();
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        if (_coroutine != null)
            CoroutineRunner.StopCoroutine(_coroutine);

        _isAbilityUse = false;
        ChandeAbilityEffect(_isAbilityUse);
    }

    protected override void OnCooldownValueReseted(float value)
    {
        base.OnCooldownValueReseted(value);
        (AbilityView as ClassSkillButtonView).SetInerectableButton(true);
    }
}