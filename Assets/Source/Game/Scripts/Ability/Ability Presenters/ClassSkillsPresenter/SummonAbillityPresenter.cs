using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class SummonAbillityPresenter : IDisposable
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IGameLoopService _gameLoopService;

    private Ability _ability;
    private AbilityView _abilityView;
    private Summon _summonPrefab;
    private Spell _spell;
    private Player _player;
    private Vector3 _direction;
    private Coroutine _blastThrowingCoroutine;
    private Coroutine _damageDealCoroutine;
    private Transform _spawnPoint;
    private ParticleSystem _particleSystem;
    private Pool _pool;
    private bool _isAbilityUse;

    public SummonAbillityPresenter(Ability ability, AbilityView abilityView, Transform spawnPoint, Player player, 
        IGameLoopService gameLoopService, ICoroutineRunner coroutineRunner, Summon summonPrefab, Pool pool)
    {
        _ability = ability;
        _abilityView = abilityView;
        _spawnPoint = spawnPoint;
        _player = player;
        _coroutineRunner = coroutineRunner;
        _gameLoopService = gameLoopService;
        _summonPrefab = summonPrefab;
        _pool = pool;
        AddListener();
    }

    private void AddListener()
    {
        (_abilityView as ClassSkillView).AbilityUsed += OnButtonSkillClick;
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
        (_abilityView as ClassSkillView).AbilityUsed -= OnButtonSkillClick;
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
    }

    public void Dispose()
    {
        RemoveListener();
        GC.SuppressFinalize(this);
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
        Spawn();
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

    private void Spawn()
    {
        Summon summon= null;

        if (TryFindSummon(_summonPrefab.gameObject, out Summon poolSummon))
        {
            summon = poolSummon;
            summon.transform.position = _spawnPoint.position;
            summon.gameObject.SetActive(true);
            summon.ResetStats();
        }
        else
        {
            summon = GameObject.Instantiate(_summonPrefab, _spawnPoint.position, _spawnPoint.rotation);

            _pool.InstantiatePoolObject(summon, _summonPrefab.name);
            summon.Initialize(_player);

            //if (_deadParticles.ContainsKey(enemyData.PrefabEnemy.name) == false)
            //{
            //    _deadParticles.Add(enemyData.PrefabEnemy.name, enemyData.EnemyDieParticleSystem);
            //}
        }
    }

    private bool TryFindSummon(GameObject enemyType, out Summon poolSummon)
    {
        poolSummon = null;
        Debug.Log(enemyType.name);
        if (_pool.TryPoolObject(enemyType, out PoolObject enemyPool))
        {
            poolSummon = enemyPool as Summon;
        }

        return poolSummon != null;
    }

    private void OnCooldownValueChanged(float value)
    {
        _abilityView.ChangeCooldownValue(value);
    }

    private void OnCooldownValueReseted(float value)
    {
        _abilityView.ResetCooldownValue(value);
    }
}