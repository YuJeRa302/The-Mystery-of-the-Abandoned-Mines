using Assets.Source.Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonTracePresenter : AbilityPresenter
{
    private readonly float _delayAttack = 0.3f;
    private readonly float _delaySpawnSpell = 0.5f;

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
            CoroutineRunner.StopCoroutine(_spawnedSpellCorountine);

        if (_blastThrowingCoroutine != null)
            CoroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            CoroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    protected override void OnGameResumed(bool state)
    {
        base.OnGameResumed(state);

        if (_spawnedSpellCorountine != null)
            _spawnedSpellCorountine = CoroutineRunner.StartCoroutine(SpawnSpell());

        if (_damageDealCoroutine != null)
            _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        if (_spawnedSpellCorountine != null)
            CoroutineRunner.StopCoroutine(_spawnedSpellCorountine);

        _spawnedSpellCorountine = CoroutineRunner.StartCoroutine(SpawnSpell());

        if (_damageDealCoroutine != null)
            CoroutineRunner.StopCoroutine(_damageDealCoroutine);

        _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        if (_spawnedSpellCorountine != null)
            CoroutineRunner.StopCoroutine(_spawnedSpellCorountine);

        if (_blastThrowingCoroutine != null)
            CoroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            CoroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    private IEnumerator SpawnSpell()
    {
        float lastTime = 0;

        while (lastTime < Ability.CurrentDuration)
        {
            _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z),
                Quaternion.identity);

            _spell.Initialize(_particleSystem, Ability.CurrentDuration);
            _spells.Add(_spell);

            yield return new WaitForSeconds(_delaySpawnSpell);
            lastTime++;
        }
    }

    private IEnumerator DealDamage()
    {
        while (Ability.IsAbilityEnded == false)
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
                                enemy.TakeDamage(Ability.DamageSource);
                            }
                        }
                    }
                }
            }
        }
    }
}
