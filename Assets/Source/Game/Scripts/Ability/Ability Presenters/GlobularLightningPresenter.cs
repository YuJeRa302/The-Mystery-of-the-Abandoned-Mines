using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class GlobularLightningPresenter : AbilityPresenter
    {
        private readonly float _delayAttack = 0.3f;
        private readonly float _rotationSpeed = 100f;

        private LegendaryAbilitySpell _spellPrefab;
        private LegendaryAbilitySpell _spell;
        private Vector3 _rotationVector = new Vector3(0, 1, 0);
        private Coroutine _blastThrowingCoroutine;
        private Coroutine _damageDealCoroutine;
        private ParticleSystem _particleSystem;

        public GlobularLightningPresenter(Ability ability,
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

        protected override void OnGamePaused()
        {
            base.OnGamePaused();

            if (_blastThrowingCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        protected override void OnGameResumed()
        {
            base.OnGameResumed();

            if (_blastThrowingCoroutine != null)
                _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(RotateSpell());

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
                new Vector3(
                    _player.PlayerAbilityContainer.transform.position.x,
                    _player.PlayerAbilityContainer.transform.position.y,
                    _player.PlayerAbilityContainer.transform.position.z),
                Quaternion.identity);

            _spell.Initialize(_particleSystem, _ability.CurrentDuration);
            _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(RotateSpell());
        }

        private IEnumerator DealDamage()
        {
            while (_ability.IsAbilityEnded == false)
            {
                yield return new WaitForSeconds(_delayAttack);

                if (_spell != null)
                {
                    if (_spell.TryFindEnemy(out Enemy enemy))
                        enemy.TakeDamageTest(_ability.DamageParametr);
                }
            }
        }

        private IEnumerator RotateSpell()
        {
            while (_ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                {
                    _spell.transform.Rotate(_rotationVector * _rotationSpeed * Time.deltaTime);
                    _spell.transform.position = new Vector3(
                        _player.transform.localPosition.x,
                        _player.PlayerAbilityContainer.localPosition.y,
                        _player.transform.transform.localPosition.z);
                }
                else 
                {
                    yield return null;
                }

                yield return null;
            }
        }
    }
}