using Assets.Source.Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulExplosionAbilityPresenter : AbilityPresenter
{
    private Coroutine _coroutine;
    private Transform _effectConteiner;
    private Pool _pool;
    private ParticleSystem _poolParticle;
    private Spell _spell;
    private Spell _spellPrefab;
    private bool _isAbilityUse;
    private Coroutine _damageDealCoroutine;

    public SoulExplosionAbilityPresenter(Ability ability,
        AbilityView abilityView,
        Player player,
        IGameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner, ParticleSystem abilityEffect, Spell spell) : base(ability, abilityView, player, gameLoopService, coroutineRunner)
    {
        _pool = _player.Pool;
        _poolParticle = abilityEffect;
        _effectConteiner = _player.PlayerAbilityContainer;
        _spellPrefab = spell;
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
        if (_coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);

        _isAbilityUse = false;
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
        CreateParticle();

        if (_damageDealCoroutine != null)
            _coroutineRunner.StopCoroutine(_damageDealCoroutine);

        _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    private void CreateParticle()
    {
        //PoolParticle particle;

        //if (_pool.TryPoolObject(_poolParticle.gameObject, out PoolObject pollParticle))
        //{
        //    particle = pollParticle as PoolParticle;
        //    particle.transform.position = _effectConteiner.position;
        //    particle.gameObject.SetActive(true);
        //}
        //else
        //{
        //    particle = GameObject.Instantiate(_poolParticle, _effectConteiner);
        //    (particle as DamageParticle).Initialaze(_ability.DamageParametr);
        //    _pool.InstantiatePoolObject(particle, _poolParticle.name);
        //}

        _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z),
                Quaternion.identity);

        _spell.Initialize(_poolParticle, _ability.CurrentDuration, 5f);
    }

    protected override void OnGameResumed()
    {
        //if (_isAbilityUse)
            base.OnGameResumed();

        if (_damageDealCoroutine != null)
            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnGamePaused()
    {
        base.OnGamePaused();
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
            if (_spell != null)
            {
                if (_spell.TryFindEnemys(out List<Enemy> enemies))
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.TakeDamageTest(_ability.DamageParametr);
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}