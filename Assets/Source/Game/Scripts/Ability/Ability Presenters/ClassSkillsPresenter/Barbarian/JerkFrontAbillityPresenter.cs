using Assets.Source.Game.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerkFrontAbillityPresenter : IDisposable
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IGameLoopService _gameLoopService;

    private Ability _ability;
    private AbilityView _abilityView;
    private Player _player;
    private Coroutine _coroutine;
    private Rigidbody _rigidbodyPlayer;
    private Transform _effectConteiner;
    private Pool _pool;
    private PoolParticle _poolParticle;
    private List<PoolObject> _spawnedEffects = new();
    private bool _isAbilityUse;

    public JerkFrontAbillityPresenter(Ability ability, AbilityView abilityView, Player player,
        IGameLoopService gameLoopService, ICoroutineRunner coroutineRunner, PoolParticle abilityEffect)
    {
        _ability = ability;
        _abilityView = abilityView;
        _player = player;
        _coroutineRunner = coroutineRunner;
        _gameLoopService = gameLoopService;
        _pool = _player.Pool;
        _poolParticle = abilityEffect;
        _effectConteiner = _player.PlayerAbilityContainer;
        _rigidbodyPlayer = _player.GetComponent<Rigidbody>();
        AddListener();
    }

    private void AddListener()
    {
        (_abilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
        _ability.AbilityUsed += OnAbilityUsed;
        _ability.AbilityEnded += OnAbilityEnded;
        //_ability.AbilityUpgraded += OnAbilityUpgraded;
        _ability.CooldownValueChanged += OnCooldownValueChanged;
        _ability.CooldownValueReseted += OnCooldownValueReseted;
        _ability.AbilityRemoved += Dispose;
        _gameLoopService.GamePaused += OnGamePaused;
        _gameLoopService.GameResumed += OnGameResumed;
        _gameLoopService.GameClosed += OnGameClosed;
    }

    private void RemoveListener()
    {
        (_abilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
        _ability.AbilityUsed -= OnAbilityUsed;
        _ability.AbilityEnded -= OnAbilityEnded;
        //_ability.AbilityUpgraded -= OnAbilityUpgraded;
        _ability.CooldownValueChanged -= OnCooldownValueChanged;
        _ability.CooldownValueReseted -= OnCooldownValueReseted;
        _ability.AbilityRemoved -= Dispose;
        _gameLoopService.GamePaused -= OnGamePaused;
        _gameLoopService.GameResumed -= OnGameResumed;
        _gameLoopService.GameClosed -= OnGameClosed;
    }

    private void OnAbilityEnded(Ability ability)
    {
        if (_coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);

        _isAbilityUse = false;
        ChandeAbilityEffect(_isAbilityUse);
    }

    private void OnButtonSkillClick()
    {
        if (_isAbilityUse)
            return;

       // _isAbilityUse = true;
        _ability.Use();
    }

    private void OnAbilityUsed(Ability ability)
    {
        _isAbilityUse = true;
        ChandeAbilityEffect(_isAbilityUse);
        Jerk();
    }

    private void OnGameClosed()
    {
        Dispose();
    }

    private void OnGamePaused()
    {
        _ability.StopCoroutine();
    }

    private void OnGameResumed()
    {
        if (_isAbilityUse)
            _ability.ResumeCoroutine();
    }

    private void Jerk()
    {
        if (_coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);

        _coroutine = _coroutineRunner.StartCoroutine(JerkForward());

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
               // particle = GameObject.Instantiate(_poolParticle, _effectConteiner.position, Quaternion.identity);
                _pool.InstantiatePoolObject(particle, _poolParticle.name);
                _spawnedEffects.Add(particle);
                (particle as DamageParticle).Initialaze(_ability.DamageParametr);
            }
        }
        else
        {
            foreach (var patricle in _spawnedEffects)
            {
                if (patricle.isActiveAndEnabled)
                    patricle.ReturObjectPool();
            }
        }
    }

    private IEnumerator JerkForward()
    {
        float currentTime = 0;

        while (currentTime <= _ability.CurrentDuration)
        {
            _rigidbodyPlayer.AddForce(_player.transform.forward * _ability.CurrentAbilityValue, ForceMode.Impulse);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    private void OnCooldownValueChanged(float value)
    {
        _abilityView.ChangeCooldownValue(value);
    }

    private void OnCooldownValueReseted(float value)
    {
        _abilityView.ResetCooldownValue(value);
    }

    public void Dispose()
    {
        RemoveListener();
    }
}