using Assets.Source.Game.Scripts;
using System.Collections;
using UnityEngine;

public class FirestormPresenter : AbilityPresenter
{
    private readonly float _delayAttack = 0.3f;
    private readonly float _blastSpeed = 12f;

    private LegendaryAbilitySpell _spellPrefab;
    private LegendaryAbilitySpell _spell;
    private Vector3 _direction;
    private Coroutine _blastThrowingCoroutine;
    private Coroutine _damageDealCoroutine;
    private Transform _throwPoint;
    private ParticleSystem _particleSystem;

    public FirestormPresenter(Ability ability,
        AbilityView abilityView,
        Player player,
        IGameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner,
        ParticleSystem particleSystem,
        LegendaryAbilitySpell spellPrefab) : base(ability, abilityView, player, gameLoopService, coroutineRunner)
    {
        _particleSystem = particleSystem;
        _throwPoint = _player.ThrowAbilityPoint;
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

        if (_blastThrowingCoroutine != null)
            _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());

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
                    enemy.TakeDamage(_ability.DamageSource);
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
                _spell.transform.Translate(_direction * _blastSpeed * Time.deltaTime);
            else
                yield return null;

            yield return null;
        }
    }
}