using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class ThrowAxeAbilityPresenter : IDisposable
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IGameLoopService _gameLoopService;

    private Ability _ability;
    private AbilityView _abilityView;
    private Player _player;
    private Transform _throwPoint;
    private Pool _pool;
    private AxemMssile _axemMssile;
    private bool _isAbilityUse;

    public ThrowAxeAbilityPresenter(Ability ability, AbilityView abilityView, Transform spawnPoint, Player player,
        IGameLoopService gameLoopService, ICoroutineRunner coroutineRunner, AxemMssile axemMssile, Pool pool)
    {
        _ability = ability;
        _abilityView = abilityView;
        _throwPoint = spawnPoint;
        _player = player;
        _coroutineRunner = coroutineRunner;
        _gameLoopService = gameLoopService;
        _axemMssile = axemMssile;
        _pool = pool;
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
        AxemMssile axemMssile = null;

        if (TryFindSummon(_axemMssile.gameObject, out AxemMssile poolAxe))
        {
            axemMssile = poolAxe;
            axemMssile.transform.position = _throwPoint.position;
            axemMssile.gameObject.SetActive(true);
            axemMssile.ThrowNow();
        }
        else
        {
            axemMssile = GameObject.Instantiate(_axemMssile, _throwPoint.position, UnityEngine.Quaternion.identity);

            _pool.InstantiatePoolObject(axemMssile, _axemMssile.name);
            axemMssile.Initialaze(_player, _player.PlayerAttacker.Damage, _player.PlayerMovment.MoveSpeed);

            //if (_deadParticles.ContainsKey(enemyData.PrefabEnemy.name) == false)
            //{
            //    _deadParticles.Add(enemyData.PrefabEnemy.name, enemyData.EnemyDieParticleSystem);
            //}
        }

        axemMssile.GetComponent<Rigidbody>().AddForce(_throwPoint.forward * _ability.CurrentDuration, ForceMode.Impulse);
    }

    private bool TryFindSummon(GameObject type, out AxemMssile poolObj)
    {
        poolObj = null;

        if (_pool.TryPoolObject(type, out PoolObject axePool))
        {
            poolObj = axePool as AxemMssile;
        }

        return poolObj != null;
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