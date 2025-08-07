using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Reflex.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class RotationAbility : IAttackAbilityStrategy, ILoopCoroutineStrategy
    {
        private readonly float _rotationSpeed = 100f;
        private readonly float _spellOffsetX = 3f;
        private readonly float _spellOffsetY = 2f;
        private readonly float _spellOffsetZ = 0.57f;
        private readonly float _transformStartAmplifierY = 0.57f;
        private readonly float _verticalOffset = 0f;
        private readonly float _distance = 3f;

        private ICoroutineRunner _coroutineRunner;
        private Spell _spellPrefab;
        private Spell _spell;
        private Coroutine _blastRotateCoroutine;
        private Coroutine _damageDealCoroutine;
        private ParticleSystem _particleSystem;
        private Ability _ability;
        private Player _player;
        private float _damageDelay;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            AttackAbilityData attackAbilityData = abilityEntitiesHolder.AttributeData as AttackAbilityData;
            _damageDelay = attackAbilityData.DamageSource.DamageDelay;
            _ability = abilityEntitiesHolder.Ability;
            _player = abilityEntitiesHolder.Player;
            _particleSystem = abilityEntitiesHolder.ParticleSystem;
            _spellPrefab = attackAbilityData.Spell;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public void Create()
        {
            _spell = Object.Instantiate(
               _spellPrefab,
               new Vector3(
                   _player.ShotPoint.position.x,
                   _player.ShotPoint.position.y + _transformStartAmplifierY,
                   _player.ShotPoint.position.z),
               Quaternion.identity);

            _spell.Initialize(_particleSystem, _ability.CurrentDuration, _ability.SpellRadius);

            _spell.transform.position = _player.transform.position + new Vector3(
                _spellOffsetX,
                _spellOffsetY,
                _spellOffsetZ);

            _blastRotateCoroutine = _coroutineRunner.StartCoroutine(RotateSpell());
            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public void StartCoroutine()
        {
            if (_blastRotateCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastRotateCoroutine);

            _blastRotateCoroutine = _coroutineRunner.StartCoroutine(RotateSpell());

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public void StopCoroutine()
        {
            if (_blastRotateCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastRotateCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        private IEnumerator RotateSpell()
        {
            while (_ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                {
                    _spell.transform.RotateAround(
                        _player.transform.position + Vector3.up * _verticalOffset,
                        Vector3.up,
                        _rotationSpeed * Time.deltaTime);

                    Vector3 direction = (_spell.transform.position - _player.transform.position).normalized;

                    _spell.transform.position = _player.transform.position + direction * _distance +
                        Vector3.up * _verticalOffset;
                }

                yield return null;
            }
        }

        private IEnumerator DealDamage()
        {
            while (_ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                {
                    if (_spell.TryFindEnemy(out Enemy enemy))
                        enemy.TakeDamage(_ability.DamageSource);
                }

                yield return new WaitForSeconds(_damageDelay);
            }
        }
    }
}