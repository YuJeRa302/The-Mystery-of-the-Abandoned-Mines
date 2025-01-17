using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class ThrowAxeAbilityPresenter : AttackAbilityPresenter
{
    private readonly float _delayAttack = 0.3f;
    private readonly float _blastSpeed = 0.2f;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IGameLoopService _gameLoopService;

    private Ability _ability;
    private AbilityView _abilityView;
    private Spell _spellPrefab;
    private Spell _spell;
    private Player _player;
    private Vector3 _direction;
    private Coroutine _blastThrowingCoroutine;
    private Coroutine _damageDealCoroutine;
    private Transform _throwPoint;
    private ParticleSystem _particleSystem;
    private Pool _pool;
    private AxemMssile _axemMssile;
    private bool _isAbilityUse;

    public ThrowAxeAbilityPresenter(Ability ability, 
        AbilityView abilityView, 
        Player player, 
        Transform throwPoint, 
        ParticleSystem particleSystem, 
        IGameLoopService gameLoopService, 
        ICoroutineRunner coroutineRunner, 
        Spell spellPrefab, AxemMssile axemMssile) : base(ability, abilityView, player, throwPoint, particleSystem, gameLoopService, coroutineRunner, spellPrefab)
    {
        _ability = ability;
        _abilityView = abilityView;
        _throwPoint = throwPoint;
        _particleSystem = particleSystem;
        _player = player;
        _spellPrefab = spellPrefab;
        _gameLoopService = gameLoopService;
        _coroutineRunner = coroutineRunner;
        _pool = _player.Pool;
        _axemMssile = axemMssile;
        AddListener();
    }

    protected override void AddListener()
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

    protected override void RemoveListener()
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
            Debug.Log("Нашел в пуле");
            axemMssile = poolAxe;
            axemMssile.transform.position = _throwPoint.position;
            axemMssile.gameObject.SetActive(true);
            axemMssile.ThrowNow();
        }
        else
        {
            axemMssile = GameObject.Instantiate(_axemMssile, _throwPoint.position, _throwPoint.rotation);

            _pool.InstantiatePoolObject(axemMssile, _axemMssile.name);
            axemMssile.Initialaze(_player, 10, 3);

            //if (_deadParticles.ContainsKey(enemyData.PrefabEnemy.name) == false)
            //{
            //    _deadParticles.Add(enemyData.PrefabEnemy.name, enemyData.EnemyDieParticleSystem);
            //}
        }
    }

    private bool TryFindSummon(GameObject type, out AxemMssile poolObj)
    {
        poolObj = null;
        Debug.Log(type.name);
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
}