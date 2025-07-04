using Assets.Source.Game.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpiphanyAbilityPresenter : AbilityPresenter
{
    private Coroutine _coroutine;
    private Coroutine _damageDealCoroutine;
    private ParticleSystem _poolParticle;
    private Spell _spell;
    private Spell _spellPrefab;
    private bool _isAbilityUse;
    private float _spellRadius = 8f;
    private float _delayDamage = 1f;

    public EpiphanyAbilityPresenter(
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
        (AbilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
    }

    protected override void RemoveListener()
    {
        base.RemoveListener();
        (AbilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        if (_coroutine != null)
            CoroutineRunner.StopCoroutine(_coroutine);

        if (_damageDealCoroutine != null)
            CoroutineRunner.StopCoroutine(_damageDealCoroutine);

        _isAbilityUse = false;
    }

    private void OnButtonSkillClick()
    {
        if (_isAbilityUse)
            return;

        _isAbilityUse = true;
        Ability.Use();
        (AbilityView as ClassSkillButtonView).SetInerectableButton(false);
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        _isAbilityUse = true;
        CastEpiphany();

        if (_damageDealCoroutine != null)
            CoroutineRunner.StopCoroutine(_damageDealCoroutine);

        _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
    }

    protected override void OnGameResumed(bool state)
    {
        base.OnGameResumed(state);
    }

    private void CastEpiphany()
    {
        CreateParticle();
    }

    private void CreateParticle()
    {
        _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z),
                Quaternion.identity);

        _spell.Initialize(_poolParticle, Ability.CurrentDuration, _spellRadius);
    }

    protected override void OnCooldownValueReseted(float value)
    {
        base.OnCooldownValueReseted(value);
        (AbilityView as ClassSkillButtonView).SetInerectableButton(true);
    }

    private IEnumerator DealDamage()
    {
        while (Ability.IsAbilityEnded == false)
        {
            if (_spell != null)
            {
                if (_spell.TryFindEnemys(out List<Enemy> enemies))
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.TakeDamage(Ability.DamageSource);
                    }
                }
            }

            yield return new WaitForSeconds(_delayDamage);
        }
    }
}