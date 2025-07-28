using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Reflex.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class SoulExplosionAbilityPresenter : IAbilityStrategy, IClassAbilityStrategy, IAbilityPauseStrategy
    {
        private ICoroutineRunner _coroutineRunner;
        private ParticleSystem _poolParticle;
        private Spell _spell;
        private Spell _spellPrefab;
        private bool _isAbilityUse;
        private Coroutine _damageDealingCoroutine;
        private float _delayDamage = 1f;
        private float _spellRadius = 5f;
        private Ability _ability;
        private AbilityView _abilityView;
        private Player _player;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            SoulExplosionAbilityData soulExplosionAbilityData = abilityEntitiesHolder.AttributeData as SoulExplosionAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _abilityView = abilityEntitiesHolder.AbilityView;
            _player = abilityEntitiesHolder.Player;
            _poolParticle = soulExplosionAbilityData.DamageParticle;
            _spellPrefab = soulExplosionAbilityData.Spell;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public void UsedAbility(Ability ability)
        {
            CreateParticle();

            if (_damageDealingCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealingCoroutine);

            _damageDealingCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public void EndedAbility(Ability ability)
        {
            if (_damageDealingCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealingCoroutine);

            _isAbilityUse = false;
        }

        public void AddListener()
        {
            (_abilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
        }

        public void RemoveListener()
        {
            (_abilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
        }

        public void SetInteractableButton()
        {
            (_abilityView as ClassSkillButtonView).SetInteractableButton(true);
        }

        public void PausedGame(bool state)
        {
            if (_damageDealingCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealingCoroutine);
        }

        public void ResumedGame(bool state)
        {
            if (_damageDealingCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealingCoroutine);

            _damageDealingCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        private void OnButtonSkillClick()
        {
            if (_isAbilityUse)
                return;

            _isAbilityUse = true;
            _ability.Use();
            (_abilityView as ClassSkillButtonView).SetInteractableButton(false);
        }

        private void CreateParticle()
        {
            _spell = Object.Instantiate(
                    _spellPrefab,
                    new Vector3(
                    _player.transform.position.x, 
                    _player.transform.position.y,
                    _player.transform.position.z),
                    Quaternion.identity);

            _spell.Initialize(_poolParticle, _ability.CurrentDuration, _spellRadius);
        }

        private IEnumerator DealDamage()
        {
            while (_ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                {
                    if (_spell.TryFindEnemies(out List<Enemy> enemies))
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
}