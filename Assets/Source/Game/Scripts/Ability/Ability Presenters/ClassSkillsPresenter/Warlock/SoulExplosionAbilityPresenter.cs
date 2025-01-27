using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class SoulExplosionAbilityPresenter : MonoBehaviour
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

    public SoulExplosionAbilityPresenter(Ability ability, AbilityView abilityView, Player player,
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
        CreateParticle();
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

    private void CreateParticle()
    {
        PoolParticle particle;

        if (_pool.TryPoolObject(_poolParticle.gameObject, out PoolObject pollParticle))
        {
            particle = pollParticle as PoolParticle;
            particle.transform.position = _effectConteiner.position;
            particle.gameObject.SetActive(true);
        }
        else
        {
            particle = GameObject.Instantiate(_poolParticle, _effectConteiner);
            (particle as DamageParticle).Initialaze(_ability.AbilityDamage);
            _pool.InstantiatePoolObject(particle, _poolParticle.name);
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