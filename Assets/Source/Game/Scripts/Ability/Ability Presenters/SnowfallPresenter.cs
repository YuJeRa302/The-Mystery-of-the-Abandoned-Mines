using Assets.Source.Game.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowfallPresenter : AbilityPresenter
{
    private readonly float _delayAttack = 0.3f;
    private readonly float _blastSpeed = 12f;
    private readonly int _countSpell = 3;

    private LegendaryAbilitySpell _spellPrefab;
    private LegendaryAbilitySpell _spell;
    private List<LegendaryAbilitySpell> _spawnedSpell = new List<LegendaryAbilitySpell>();
    private Vector3 _direction;
    private Coroutine _blastThrowingCoroutine;
    private Coroutine _damageDealCoroutine;
    private Coroutine _throwSpellCorountine;
    private Transform _throwPoint;
    private ParticleSystem _particleSystem;

    public SnowfallPresenter(Ability ability,
        AbilityView abilityView,
        Player player,
        IGameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner, ParticleSystem particleSystem,
        LegendaryAbilitySpell spellPrefab) : base(ability, abilityView, player, gameLoopService, coroutineRunner)
    {
        _throwPoint = player.ThrowAbilityPoint;
        _spellPrefab = spellPrefab;
        _particleSystem = particleSystem;
        AddListener();
    }

    protected override void OnGamePaused(bool state)
    {
        base.OnGamePaused(state);

        if (_blastThrowingCoroutine != null)
            _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);

        if (_throwSpellCorountine != null)
            _coroutineRunner.StopCoroutine(_throwSpellCorountine);
    }

    protected override void OnGameResumed(bool state)
    {
        base.OnGameResumed(state);

        if (_blastThrowingCoroutine != null)
            _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());

        if (_damageDealCoroutine != null)
            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());

        if (_throwSpellCorountine != null)
            _throwSpellCorountine = _coroutineRunner.StartCoroutine(ThrowBlast());
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        if (_throwSpellCorountine != null)
            _coroutineRunner.StopCoroutine(_throwSpellCorountine);

        _throwSpellCorountine = _coroutineRunner.StartCoroutine(ThrowBlast());

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);

        _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        if (_throwSpellCorountine != null)
            _coroutineRunner.StopCoroutine(_throwSpellCorountine);

        if (_blastThrowingCoroutine != null)
            _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);

        _spawnedSpell.Clear();
    }

    private IEnumerator ThrowBlast()
    {
        for (int i = 0; i < _countSpell; i++)
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
            _spawnedSpell.Add(_spell);
            _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());

            yield return new WaitForSeconds(0.5f);
        }
        
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

            for (int i = 0; i < _spawnedSpell.Count; i++)
            {
                if (_spawnedSpell[i] != null)
                {
                    if (_spawnedSpell[i].TryFindEnemy(out Enemy enemy))
                    {
                        enemy.TakeDamage(_ability.DamageSource);
                    }
                }
            }
        }
    }

    private IEnumerator ThrowingBlast()
    {
        while (_ability.IsAbilityEnded == false)
        {
            for (int i = 0; i < _spawnedSpell.Count; i++)
            {
                if (_spawnedSpell[i] != null)
                    _spawnedSpell[i].transform.Translate(_direction * _blastSpeed * Time.deltaTime);
                else
                    yield return null;
            }

            yield return null;
        }
    }
}