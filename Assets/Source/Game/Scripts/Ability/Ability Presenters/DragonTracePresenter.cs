using Assets.Source.Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonTracePresenter : AbilityPresenter
{
    private readonly float _delayAttack = 0.3f;

    private LegendaryAbilitySpell _spellPrefab;
    private LegendaryAbilitySpell _spell;
    private Coroutine _blastThrowingCoroutine;
    private Coroutine _damageDealCoroutine;
    private Coroutine _spawnedSpellCorountine;
    private ParticleSystem _particleSystem;
    private List<LegendaryAbilitySpell> _spells = new ();

    public DragonTracePresenter(
        Ability ability,
        AbilityView abilityView,
        Player player,
        GamePauseService gamePauseService,
        GameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner,
        ParticleSystem particleSystem,
        LegendaryAbilitySpell spellPrefab) : base(ability, abilityView, player, gamePauseService, gameLoopService, coroutineRunner)
    {
        _particleSystem = particleSystem;
        _spellPrefab = spellPrefab;
        AddListener();
    }

    protected override void OnGamePaused(bool state)
    {
        base.OnGamePaused(state);

        if (_spawnedSpellCorountine != null)
            _coroutineRunner.StopCoroutine(_spawnedSpellCorountine);

        if (_blastThrowingCoroutine != null)
            _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    protected override void OnGameResumed(bool state)
    {
        base.OnGameResumed(state);

        if (_spawnedSpellCorountine != null)
            _spawnedSpellCorountine = _coroutineRunner.StartCoroutine(SpawnSpell());

        if (_damageDealCoroutine != null)
            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        if (_spawnedSpellCorountine != null)
            _coroutineRunner.StopCoroutine(_spawnedSpellCorountine);

        _spawnedSpellCorountine = _coroutineRunner.StartCoroutine(SpawnSpell());

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);

        _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        if (_spawnedSpellCorountine != null)
            _coroutineRunner.StopCoroutine(_spawnedSpellCorountine);

        if (_blastThrowingCoroutine != null)
            _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    private IEnumerator SpawnSpell()
    {
        float lastTime = 0;

        while (lastTime < _ability.CurrentDuration)
        {
            _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z),
                Quaternion.identity);

            _spell.Initialize(_particleSystem, _ability.CurrentDuration);
            _spells.Add(_spell);

            yield return new WaitForSeconds(0.5f);
            lastTime++;
        }
    }

    private IEnumerator DealDamage()
    {
        while (_ability.IsAbilityEnded == false)
        {
            yield return new WaitForSeconds(_delayAttack);

            if (_spells.Count > 0)
            {
                foreach (var spell in _spells)
                {
                    if (spell != null)
                    {
                        if (_spell.TryFindEnemys(out List<Enemy> enemies))
                        {
                            foreach (var enemy in enemies)
                            {
                                enemy.TakeDamage(_ability.DamageSource);
                            }
                        }
                    }
                }
            }
        }
    }
}
