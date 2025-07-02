using Assets.Source.Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulExplosionAbilityPresenter : AbilityPresenter
{
    private Coroutine _coroutine;
    private ParticleSystem _poolParticle;
    private Spell _spell;
    private Spell _spellPrefab;
    private bool _isAbilityUse;
    private Coroutine _damageDealCoroutine;
    private float _delayDamage = 1f;
    private float _spellRadius = 5f;

    public SoulExplosionAbilityPresenter(
        Ability ability,
        AbilityView abilityView,
        Player player,
        GamePauseService gamePauseService,
        GameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner,
        ParticleSystem abilityEffect,
        Spell spell) : base(ability, abilityView, player, gamePauseService, gameLoopService, coroutineRunner)
    {
        _poolParticle = abilityEffect;
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
        _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z),
                Quaternion.identity);

        _spell.Initialize(_poolParticle, _ability.CurrentDuration, _spellRadius);
    }

    protected override void OnGameResumed(bool state)
    {
        base.OnGameResumed(state);

        if (_damageDealCoroutine != null)
            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnGamePaused(bool state)
    {
        base.OnGamePaused(state);
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
                        enemy.TakeDamage(_ability.DamageSource);
                    }
                }
            }

            yield return new WaitForSeconds(_delayDamage);
        }
    }
}