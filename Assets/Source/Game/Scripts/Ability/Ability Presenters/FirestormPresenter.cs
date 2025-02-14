using Assets.Source.Game.Scripts;
using System;
using System.Collections;
using UnityEngine;

public class FirestormPresenter : IDisposable
{
    private readonly float _delayAttack = 0.3f;
    private readonly float _blastSpeed = 0.2f;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IGameLoopService _gameLoopService;

    private Ability _ability;
    private AbilityView _abilityView;
    private LegendaryAbilitySpell _spellPrefab;
    private LegendaryAbilitySpell _spell;
    private Player _player;
    private Vector3 _direction;
    private Coroutine _blastThrowingCoroutine;
    private Coroutine _damageDealCoroutine;
    private Transform _throwPoint;
    private ParticleSystem _particleSystem;

    public FirestormPresenter(
        Ability ability,
        AbilityView abilityView,
        Player player,
        Transform throwPoint,
        ParticleSystem particleSystem,
        IGameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner,
        LegendaryAbilitySpell spellPrefab)
    {
        _ability = ability;
        _abilityView = abilityView;
        _particleSystem = particleSystem;
        _throwPoint = throwPoint;
        _player = player;
        _spellPrefab = spellPrefab;
        _gameLoopService = gameLoopService;
        _coroutineRunner = coroutineRunner;
        AddListener();
    }

    public void Dispose()
    {
        if (_abilityView != null)
            _abilityView.ViewDestroy();

        RemoveListener();
        GC.SuppressFinalize(this);
    }

    protected virtual void AddListener()
    {
        _ability.AbilityUsed += OnAbilityUsed;
        _ability.AbilityEnded += OnAbilityEnded;
        _ability.AbilityUpgraded += OnAbilityUpgraded;
        _ability.CooldownValueChanged += OnCooldownValueChanged;
        _ability.CooldownValueReseted += OnCooldownValueReseted;
        _ability.AbilityRemoved += Dispose;
        _gameLoopService.GamePaused += OnGamePaused;
        _gameLoopService.GameResumed += OnGameResumed;
        _gameLoopService.GameClosed += OnGameClosed;
    }

    protected virtual void RemoveListener()
    {
        _ability.AbilityUsed -= OnAbilityUsed;
        _ability.AbilityEnded -= OnAbilityEnded;
        _ability.AbilityUpgraded -= OnAbilityUpgraded;
        _ability.CooldownValueChanged -= OnCooldownValueChanged;
        _ability.CooldownValueReseted -= OnCooldownValueReseted;
        _ability.AbilityRemoved -= Dispose;
        _gameLoopService.GamePaused -= OnGamePaused;
        _gameLoopService.GameResumed -= OnGameResumed;
        _gameLoopService.GameClosed -= OnGameClosed;
    }

    private void OnGameClosed()
    {
        Dispose();
    }

    private void OnGamePaused()
    {
        _ability.StopCoroutine();

        if (_blastThrowingCoroutine != null)
            _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    private void OnGameResumed()
    {
        _ability.Use();

        if (_blastThrowingCoroutine != null)
            _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());

        if (_damageDealCoroutine != null)
            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    private void OnAbilityUsed(Ability ability)
    {
        ThrowBlast();

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);

        _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    private void OnAbilityEnded(Ability ability)
    {
        if (_blastThrowingCoroutine != null)
            _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    private void OnAbilityUpgraded(float delay)
    {
        _abilityView.Upgrade(delay);
    }

    private void OnCooldownValueChanged(float value)
    {
        _abilityView.ChangeCooldownValue(value);
    }

    private void OnCooldownValueReseted(float value)
    {
        _abilityView.ResetCooldownValue(value);
    }

    private void ThrowBlast()
    {
        _spell = GameObject.Instantiate(
            _spellPrefab,
            new Vector3(_throwPoint.transform.position.x, _throwPoint.transform.position.y, _throwPoint.transform.position.z),
            Quaternion.identity);

        if (TryFindEnemy(out Enemy enemy))
        {
            Transform curretnTarget = enemy.transform;
            _direction = (curretnTarget.position - _player.transform.position).normalized;
        }
        else
        {
            _direction = _throwPoint.forward;
        }

        _spell.Initialize(_particleSystem, _ability.CurrentDuration);
        _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());
    }

    public bool TryFindEnemy(out Enemy enemy)
    {
        Collider[] coliderEnemy = Physics.OverlapSphere(_player.transform.position, 20);

        foreach (Collider collider in coliderEnemy)
        {
            if (collider.TryGetComponent(out enemy))
                return true;
        }

        enemy = null;
        return false;
    }

    private IEnumerator DealDamage()
    {
        while (_ability.IsAbilityEnded == false)
        {
            yield return new WaitForSeconds(_delayAttack);

            if (_spell != null)
            {
                if (_spell.TryFindEnemy(out Enemy enemy))
                {
                    enemy.TakeDamage(_ability.CurrentAbilityValue);
                    //enemy.TakeDamageTest(_ability.DamageParametr);
                }
            }
        }
    }

    private IEnumerator ThrowingBlast()
    {
        while (_ability.IsAbilityEnded == false)
        {
            if (_spell != null)
                _spell.transform.Translate(_direction * _blastSpeed);
            else
                yield return null;

            yield return null;
        }
    }
}