using Assets.Source.Game.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThunderPresenter : IDisposable
{
    private readonly float _delayAttack = 0.3f;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IGameLoopService _gameLoopService;

    private Ability _ability;
    private AbilityView _abilityView;
    private LegendaryAbilitySpell _spellPrefab;
    private LegendaryAbilitySpell _spell;
    private Player _player;
    private Coroutine _blastThrowingCoroutine;
    private Coroutine _damageDealCoroutine;
    private ParticleSystem _particleSystem;

    public ThunderPresenter(
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
                new Vector3(_player.transform.position.x, _spellPrefab.transform.position.y, _player.transform.position.z),
                Quaternion.identity);

        (_spell as LegendadatyTargetAbilitySpell).Initialize(_particleSystem, _ability.CurrentDuration);
    }

    private IEnumerator DealDamage()
    {
        while (_ability.IsAbilityEnded == false)
        {
            yield return new WaitForSeconds(_delayAttack);

            if (_spell != null)
            {
                if ((_spell as LegendadatyTargetAbilitySpell).TryFindEnemys(out List<Enemy> enemies))
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.TakeDamage(_ability.CurrentAbilityValue);
                    }

                    GameObject.Destroy(_spell);
                }
            }
        }
    }
}