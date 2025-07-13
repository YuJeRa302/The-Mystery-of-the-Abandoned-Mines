using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class SoulExplosionAbilityPresenter : AbilityPresenter
    {
        private Coroutine _coroutine;
        private ParticleSystem _poolParticle;
        private Spell _spell;
        private Spell _spellPrefab;
        private bool _isAbilityUse;
        private Coroutine _damageDealingCoroutine;
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
            Spell spell) : base(ability, abilityView, player,
                gamePauseService, gameLoopService, coroutineRunner)
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

            if (_damageDealingCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealingCoroutine);

            _isAbilityUse = false;
        }

        private void OnButtonSkillClick()
        {
            if (_isAbilityUse)
                return;

            _isAbilityUse = true;
            Ability.Use();
            (AbilityView as ClassSkillButtonView).SetInteractableButton(false);
        }

        protected override void OnAbilityUsed(Ability ability)
        {
            _isAbilityUse = true;
            CreateParticle();

            if (_damageDealingCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealingCoroutine);

            _damageDealingCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
        }

        private void CreateParticle()
        {
            _spell = Object.Instantiate(
                    _spellPrefab,
                    new Vector3(Player.transform.position.x, Player.transform.position.y,
                    Player.transform.position.z),
                    Quaternion.identity);

            _spell.Initialize(_poolParticle, Ability.CurrentDuration, _spellRadius);
        }

        protected override void OnGameResumed(bool state)
        {
            base.OnGameResumed(state);

            if (_damageDealingCoroutine != null)
                _damageDealingCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
        }

        protected override void OnCooldownValueReset(float value)
        {
            base.OnCooldownValueReset(value);
            (AbilityView as ClassSkillButtonView).SetInteractableButton(true);
        }

        private IEnumerator DealDamage()
        {
            while (Ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                {
                    if (_spell.TryFindEnemies(out List<Enemy> enemies))
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
}