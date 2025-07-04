using Assets.Source.Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderPresenter : AbilityPresenter
{
    private readonly float _delayAttack = 0.3f;

    private LegendaryAbilitySpell _spellPrefab;
    private LegendaryAbilitySpell _spell;
    private Coroutine _blastThrowingCoroutine;
    private Coroutine _damageDealCoroutine;
    private ParticleSystem _particleSystem;

    public ThunderPresenter(
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

        if (_blastThrowingCoroutine != null)
            CoroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            CoroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    protected override void OnGameResumed(bool state)
    {
        base.OnGameResumed(state);

        if (_damageDealCoroutine != null)
            _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        ThrowBlast();

        if (_damageDealCoroutine != null)
            CoroutineRunner.StopCoroutine(_damageDealCoroutine);

        _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        if (_blastThrowingCoroutine != null)
            CoroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            CoroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    private void ThrowBlast()
    {
        _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(Player.transform.position.x, _spellPrefab.transform.position.y, Player.transform.position.z),
                Quaternion.identity);

        (_spell as LegendadatyTunderAbilitySpell).Initialize(_particleSystem, Ability.CurrentDuration);
    }

    private IEnumerator DealDamage()
    {
        while (Ability.IsAbilityEnded == false)
        {
            yield return new WaitForSeconds(_delayAttack);

            if (_spell != null)
            {
                if ((_spell as LegendadatyTunderAbilitySpell).TryFindEnemys(out List<Enemy> enemies))
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.TakeDamage(Ability.DamageSource);
                    }

                    GameObject.Destroy(_spell);
                }
            }
        }
    }
}