using Assets.Source.Game.Scripts;
using System;
using System.Collections;
using UnityEngine;

public class ElectricGuardPresenter : IDisposable
{
    private readonly float _delayAttack = 0.3f;
    private readonly float _rotationSpeed = 100f;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IGameLoopService _gameLoopService;

    private Ability _ability;
    private AbilityView _abilityView;
    private LegendaryAbilitySpell _spellPrefab;
    private LegendaryAbilitySpell _spell;
    private Player _player;
    private Vector3 _rotationVector = new Vector3(0, 1, 0);
    private Coroutine _blastThrowingCoroutine;
    private Coroutine _damageDealCoroutine;
    private Transform _throwPoint;
    private ParticleSystem _particleSystem;

    public ElectricGuardPresenter(
        Ability ability,
        AbilityView abilityView,
        Player player,
        ParticleSystem particleSystem,
        IGameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner,
        LegendaryAbilitySpell spellPrefab)
    {
        _ability = ability;
        _abilityView = abilityView;
        _particleSystem = particleSystem;
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

    private void AddListener()
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

    private void RemoveListener()
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
            _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(RotateSpell());

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
                new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z),
                Quaternion.identity);

        _spell.Initialize(_particleSystem, _ability.CurrentDuration);
    }

    private IEnumerator DealDamage()
    {
        while (_ability.IsAbilityEnded == false)
        {
            yield return new WaitForSeconds(_delayAttack);

            if (_spell != null)
            {
                if (_spell.TryFindEnemy(out Enemy enemy))
                    enemy.TakeDamage(_ability.CurrentAbilityValue);
            }
        }
    }

    private IEnumerator RotateSpell()
    {
        while (_ability.IsAbilityEnded == false)
        {
            if (_spell != null)
            {
                _spell.transform.Rotate(_rotationVector * _rotationSpeed * Time.deltaTime);
                _spell.transform.position = new Vector3(
                    _player.transform.localPosition.x,
                    _player.PlayerAbilityContainer.localPosition.y,
                    _player.transform.transform.localPosition.z);
            }
            else
            {
                yield return null;
            }

            yield return null;
        }
    }
}