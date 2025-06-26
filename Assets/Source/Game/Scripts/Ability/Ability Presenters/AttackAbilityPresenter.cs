using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class AttackAbilityPresenter : AbilityPresenter
    {
        private readonly float _rotationSpeed = 100f;
        private readonly float _delayAttackBlast = 0.3f;
        private readonly float _delayAOE = 1f;
        private readonly float _delayTargetSpell = 1f;
        private readonly float _blastSpeed = 10f;
        private readonly float _searcheRadius = 20f;
        private const float SpellOffsetX = 3f;
        private const float SpellOffsetY = 2f;
        private const float SpellOffsetZ = 0.57f;

        private Spell _spellPrefab;
        private Spell _spell;
        private Vector3 _direction;
        private Coroutine _blastThrowingCoroutine;
        private Coroutine _blastRotateCoroutine;
        private Coroutine _damageDealCoroutine;
        private Transform _throwPoint;
        private ParticleSystem _particleSystem;
        float _currentDelayAttack = 0.3f;

        public AttackAbilityPresenter(Ability ability, 
            AbilityView abilityView, 
            Player player, 
            IGameLoopService gameLoopService, 
            ICoroutineRunner coroutineRunner,
            Spell spellPrefab, ParticleSystem particleSystem) : base(ability, abilityView, player, gameLoopService, coroutineRunner)
        {
            _throwPoint = _player.ThrowAbilityPoint;
            _particleSystem = particleSystem;
            _spellPrefab = spellPrefab;
            AddListener();

            if (_ability.TypeAttackAbility == TypeAttackAbility.AoEAbility)
                _currentDelayAttack = _delayAOE;

            if (_ability.TypeAttackAbility == TypeAttackAbility.ProjectileAbility)
                _currentDelayAttack = _delayAttackBlast;

            if (_ability.TypeAttackAbility == TypeAttackAbility.TargetSpell)
                _currentDelayAttack = _delayTargetSpell;

            if (_ability.TypeAttackAbility == TypeAttackAbility.RotationAbility)
                _currentDelayAttack = _delayAttackBlast;
        }

        protected override void OnGamePaused(bool state)
        {
            _ability.StopCoroutine();

            if (_blastThrowingCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            if (_blastRotateCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastRotateCoroutine);
        }

        protected override void OnGameResumed(bool state)
        {
            _ability.Use();

            if (_blastThrowingCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);
                _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());
            }

            if (_damageDealCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
                _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
            }

            if (_blastRotateCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_blastRotateCoroutine);
                _blastRotateCoroutine = _coroutineRunner.StartCoroutine(RotateSpell());
            }
        }

        protected override void OnAbilityUsed(Ability ability) 
        {
            if (_ability.TypeAttackAbility == TypeAttackAbility.AoEAbility)
                CreateAoESpell();

            if (_ability.TypeAttackAbility == TypeAttackAbility.ProjectileAbility)
                ThrowBlast();

            if (_ability.TypeAttackAbility == TypeAttackAbility.TargetSpell)
                CreateTargetSpell();

            if (_ability.TypeAttackAbility == TypeAttackAbility.RotationAbility)
                CreateRotateSpell();

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

            if (_blastRotateCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastRotateCoroutine);
        }

        private void CreateRotateSpell()
        {
            float transformStartAmplifierY = 0.57f;

            _spell = GameObject.Instantiate(
               _spellPrefab,
               new Vector3(
                   _player.ShotPoint.position.x,
                   _player.ShotPoint.position.y + transformStartAmplifierY,
                   _player.ShotPoint.position.z),
               Quaternion.identity);

            _spell.Initialize(_particleSystem, _ability.CurrentDuration, _ability.SpellRadius);
            _spell.transform.position = _player.transform.position + new Vector3(SpellOffsetX, SpellOffsetY, SpellOffsetZ);

            _blastRotateCoroutine = _coroutineRunner.StartCoroutine(RotateSpell());
        }

        private void ThrowBlast()
        {
            _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(_throwPoint.transform.position.x, _spellPrefab.transform.position.y, _throwPoint.transform.position.z),
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

            _spell.Initialize(_particleSystem, _ability.CurrentDuration, _ability.SpellRadius);
            _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());
        }

        private void CreateAoESpell()
        {
            _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z),
                Quaternion.identity);

            _spell.Initialize(_particleSystem, _ability.CurrentDuration, _ability.SpellRadius);
        }

        private void CreateTargetSpell()
        {
            Vector3 targetPosition;

            if (TryFindEnemy(out Enemy enemy))
                targetPosition = enemy.transform.position;
            else
                targetPosition = _player.transform.position;

            _spell = GameObject.Instantiate(
                _spellPrefab,
                targetPosition,
                Quaternion.identity);

            _spell.Initialize(_particleSystem, _ability.CurrentDuration, _ability.SpellRadius);
        }

        public bool TryFindEnemy(out Enemy enemy)
        {
            Collider[] coliderEnemy = Physics.OverlapSphere(_player.transform.position, _searcheRadius);

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
                if (_spell != null)
                {
                    if (_ability.TypeAttackAbility == TypeAttackAbility.AoEAbility)
                    {
                        if (_spell.TryFindEnemys(out List<Enemy> enemies))
                        {
                            foreach (var enemy in enemies)
                            {
                                enemy.TakeDamage(_ability.DamageSource);
                            }
                        }
                    }
                    else
                    {
                        if (_spell.TryFindEnemy(out Enemy enemy))
                        {
                            enemy.TakeDamage(_ability.DamageSource);
                        }
                    }
                }

                yield return new WaitForSeconds(_currentDelayAttack);
            }
        }

        private IEnumerator ThrowingBlast()
        {
            while (_ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                    _spell.transform.Translate(_direction * _blastSpeed * Time.deltaTime);

                yield return null;
            }
        }

        private IEnumerator RotateSpell()
        {
            float verticalOffset = 0f;
            float distance = 3f;

            while (_ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                {
                    _spell.transform.RotateAround(_player.transform.position + Vector3.up * verticalOffset, Vector3.up, _rotationSpeed * Time.deltaTime);
                    Vector3 direction = (_spell.transform.position - _player.transform.position).normalized;
                    _spell.transform.position = _player.transform.position + direction * distance + Vector3.up * verticalOffset;
                }

                yield return null;
            }
        }
    }
}