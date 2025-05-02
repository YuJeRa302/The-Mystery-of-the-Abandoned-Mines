using Assets.Source.Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAxeAbilityPresenter : AbilityPresenter
{
    private Transform _throwPoint;
    private Pool _pool;
    private AxemMssile _axemMssilePredab;
    private AxemMssile _axemMssile;
    private bool _isAbilityUse;
    private Coroutine _damageDealCoroutine;

    public ThrowAxeAbilityPresenter(Ability ability,
        AbilityView abilityView,
        Player player,
        IGameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner, AxemMssile axemMssile) : base(ability, abilityView, player, gameLoopService, coroutineRunner)
    {
        _throwPoint = _player.ThrowAbilityPoint;
        _pool = _player.Pool;
        _axemMssilePredab = axemMssile;
        AddListener();
    }

    protected override void AddListener()
    {
        base.AddListener();
        (_abilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
    }

    protected override void RemoveListener()
    {
        base.RemoveListener();
        (_abilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        _isAbilityUse = false;

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);
    }

    private void OnButtonSkillClick()
    {
        if (_isAbilityUse)
            return;

        _isAbilityUse = true;
        _ability.Use();
        (_abilityView as ClassSkillButtonView).SetInerectableButton(false);
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        _isAbilityUse = true;
        Spawn();

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);

        _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnGameResumed()
    {
        //if (_isAbilityUse)
        base.OnGameResumed();

        if (_damageDealCoroutine != null)
            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    private void Spawn()
    {
        if (TryFindSummon(_axemMssilePredab.gameObject, out AxemMssile poolAxe))
        {
            _axemMssile = poolAxe;
            _axemMssile.transform.position = _throwPoint.position;
            _axemMssile.gameObject.SetActive(true);
            _axemMssile.ThrowNow();
        }
        else
        {
            _axemMssile = GameObject.Instantiate(_axemMssilePredab, _throwPoint.position, UnityEngine.Quaternion.identity);

            _pool.InstantiatePoolObject(_axemMssile, _axemMssilePredab.name);
            _axemMssile.Initialaze(_player, _player.DamageSource, _player.MoveSpeed, _ability.CurrentDuration);
        }

        _axemMssile.GetComponent<Rigidbody>().AddForce(_throwPoint.forward * _ability.CurrentDuration, ForceMode.Impulse);
    }

    private bool TryFindSummon(GameObject type, out AxemMssile poolObj)
    {
        poolObj = null;

        if (_pool.TryPoolObject(type, out PoolObject axePool))
        {
            poolObj = axePool as AxemMssile;
        }

        return poolObj != null;
    }

    protected override void OnCooldownValueReseted(float value)
    {
        base.OnCooldownValueReseted(value);
        (_abilityView as ClassSkillButtonView).SetInerectableButton(true);
    }

    private IEnumerator DealDamage()
    {
        while (_ability.IsAbilityEnded == false)
        {
            if (_axemMssile != null)
            {
                if (_axemMssile.TryFindEnemys(out List<Enemy> enemies))
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.TakeDamage(_player.DamageSource);
                    }
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
}