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

    public ThunderPresenter(Ability ability,
        AbilityView abilityView,
        Player player,
        IGameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner,
        ParticleSystem particleSystem,
        LegendaryAbilitySpell spellPrefab) : base(ability, abilityView, player, gameLoopService, coroutineRunner)
    {
        _particleSystem = particleSystem;
        _spellPrefab = spellPrefab;
        AddListener();
    }

    protected override void OnGamePaused(bool state)
    {
        base.OnGamePaused(state);

        if (_blastThrowingCoroutine != null)
            _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    protected override void OnGameResumed(bool state)
    {
        base.OnGameResumed(state);

        if (_damageDealCoroutine != null)
            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        ThrowBlast();

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);

        _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        if (_blastThrowingCoroutine != null)
            _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    private void ThrowBlast()
    {
        _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(_player.transform.position.x, _spellPrefab.transform.position.y, _player.transform.position.z),
                Quaternion.identity);

        (_spell as LegendadatyTunderAbilitySpell).Initialize(_particleSystem, _ability.CurrentDuration);
    }

    private IEnumerator DealDamage()
    {
        while (_ability.IsAbilityEnded == false)
        {
            yield return new WaitForSeconds(_delayAttack);

            if (_spell != null)
            {
                if ((_spell as LegendadatyTunderAbilitySpell).TryFindEnemys(out List<Enemy> enemies))
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.TakeDamage(_ability.DamageSource);
                    }

                    GameObject.Destroy(_spell);
                }
            }
        }
    }
}