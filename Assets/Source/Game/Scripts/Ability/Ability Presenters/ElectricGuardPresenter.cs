using Assets.Source.Game.Scripts;
using System.Collections;
using UnityEngine;

public class ElectricGuardPresenter : AbilityPresenter
{
    private readonly float _delayAttack = 0.3f;
    private readonly float _rotationSpeed = 100f;

    private LegendaryAbilitySpell _spellPrefab;
    private LegendaryAbilitySpell _spell;
    private Vector3 _rotationVector = new Vector3(0, 1, 0);
    private Coroutine _blastThrowingCoroutine;
    private Coroutine _damageDealCoroutine;
    private ParticleSystem _particleSystem;

    public ElectricGuardPresenter(
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

        if (_blastThrowingCoroutine != null)
            _blastThrowingCoroutine = CoroutineRunner.StartCoroutine(RotateSpell());

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
                new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z),
                Quaternion.identity);

        _spell.Initialize(_particleSystem, Ability.CurrentDuration);
    }

    private IEnumerator DealDamage()
    {
        while (Ability.IsAbilityEnded == false)
        {
            yield return new WaitForSeconds(_delayAttack);

            if (_spell != null)
                if (_spell.TryFindEnemy(out Enemy enemy))
                    enemy.TakeDamage(Ability.DamageSource);
        }
    }

    private IEnumerator RotateSpell()
    {
        while (Ability.IsAbilityEnded == false)
        {
            if (_spell != null)
            {
                _spell.transform.Rotate(_rotationVector * _rotationSpeed * Time.deltaTime);
                _spell.transform.position = new Vector3(
                    Player.transform.localPosition.x,
                    Player.PlayerAbilityContainer.localPosition.y,
                    Player.transform.transform.localPosition.z);
            }
            else
            {
                yield return null;
            }

            yield return null;
        }
    }
}