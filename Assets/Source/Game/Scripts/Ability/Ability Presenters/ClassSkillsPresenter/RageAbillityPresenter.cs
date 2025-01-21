using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RageAbillityPresenter : IDisposable
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IGameLoopService _gameLoopService;

    private Ability _ability;
    private AbilityView _abilityView;
    private Player _player;
    private List<Transform> _effectConteiner = new List<Transform>();
    private List<PoolObject> _spawnedEffects = new ();
    private Pool _pool;
    private PoolParticle _particleEffectPrefab;
    private int _bonusDamage;
    private float _bonusMoveSpeed;
    private int _bonusArmor;
    private bool _isAbilityUse;

    public RageAbillityPresenter(Ability ability, AbilityView abilityView, Player player, int boostDamage, float boostMoveSpeed, int boosArmor,
        IGameLoopService gameLoopService, ICoroutineRunner coroutineRunner, PoolParticle abilityEffect)
    {
        _ability = ability;
        _abilityView = abilityView;
        _player = player;
        _pool = _player.Pool;
        _particleEffectPrefab = abilityEffect;
        _bonusDamage = boostDamage;
        _bonusMoveSpeed = boostMoveSpeed;
        _bonusArmor = boosArmor;
        _coroutineRunner = coroutineRunner;
        _gameLoopService = gameLoopService;
        _effectConteiner.Add(_player.WeaponPoint);
        _effectConteiner.Add(_player.AdditionalWeaponPoint);

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
        _isAbilityUse = false;
        BoostPlayer(true);
    }

    private void OnButtonSkillClick()
    {
        if (_isAbilityUse)
            return;

        _isAbilityUse = true;
        _ability.Use();
    }

    private void OnAbilityUsed(Ability ability)
    {
        BoostPlayer(false);
    }

    private void BoostPlayer(bool isAbilityEnded)
    {
        if (isAbilityEnded)
            _player.ChengeStats(-_bonusDamage, -_bonusMoveSpeed, -_bonusArmor);
        else
            _player.ChengeStats(_bonusDamage, _bonusMoveSpeed, _bonusArmor);

        foreach (var conteiner in _effectConteiner)
        {
            ChangeEffectEneble(isAbilityEnded, conteiner);
        }
    }

    private void ChangeEffectEneble(bool isAbilityEnded, Transform conteiner)
    {
        if (isAbilityEnded == false)
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
                    patricle.ReturObjectPool();
            }
        }
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