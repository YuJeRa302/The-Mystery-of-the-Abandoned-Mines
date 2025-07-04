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
        private float _currentDelayAttack = 0.3f;

        public AttackAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            GamePauseService gamePauseService,
            GameLoopService gameLoopService,
            ICoroutineRunner coroutineRunner,
            Spell spellPrefab,
            ParticleSystem particleSystem) : base(ability, abilityView, player, gamePauseService, gameLoopService, coroutineRunner)
        {
            _throwPoint = Player.ThrowAbilityPoint;
            _particleSystem = particleSystem;
            _spellPrefab = spellPrefab;
            AddListener();

            if (Ability.TypeAttackAbility == TypeAttackAbility.AoEAbility)
                _currentDelayAttack = _delayAOE;

            if (Ability.TypeAttackAbility == TypeAttackAbility.ProjectileAbility)
                _currentDelayAttack = _delayAttackBlast;

            if (Ability.TypeAttackAbility == TypeAttackAbility.TargetSpell)
                _currentDelayAttack = _delayTargetSpell;

            if (Ability.TypeAttackAbility == TypeAttackAbility.RotationAbility)
                _currentDelayAttack = _delayAttackBlast;
        }

        protected override void OnGamePaused(bool state)
        {
            Ability.StopCoroutine();

            if (_blastThrowingCoroutine != null)
                CoroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealCoroutine);

            if (_blastRotateCoroutine != null)
                CoroutineRunner.StopCoroutine(_blastRotateCoroutine);
        }

        protected override void OnGameResumed(bool state)
        {
            Ability.Use();

            if (_blastThrowingCoroutine != null)
            {
                CoroutineRunner.StopCoroutine(_blastThrowingCoroutine);
                _blastThrowingCoroutine = CoroutineRunner.StartCoroutine(ThrowingBlast());
            }

            if (_damageDealCoroutine != null)
            {
                CoroutineRunner.StopCoroutine(_damageDealCoroutine);
                _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
            }

            if (_blastRotateCoroutine != null)
            {
                CoroutineRunner.StopCoroutine(_blastRotateCoroutine);
                _blastRotateCoroutine = CoroutineRunner.StartCoroutine(RotateSpell());
            }
        }

        protected override void OnAbilityUsed(Ability ability)
        {
            if (Ability.TypeAttackAbility == TypeAttackAbility.AoEAbility)
                CreateAoESpell();

            if (Ability.TypeAttackAbility == TypeAttackAbility.ProjectileAbility)
                ThrowBlast();

            if (Ability.TypeAttackAbility == TypeAttackAbility.TargetSpell)
                CreateTargetSpell();

            if (Ability.TypeAttackAbility == TypeAttackAbility.RotationAbility)
                CreateRotateSpell();

            if (_damageDealCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
        }

        protected override void OnAbilityEnded(Ability ability)
        {
            if (_blastThrowingCoroutine != null)
                CoroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealCoroutine);

            if (_blastRotateCoroutine != null)
                CoroutineRunner.StopCoroutine(_blastRotateCoroutine);
        }

        private void CreateRotateSpell()
        {
            float transformStartAmplifierY = 0.57f;

            _spell = GameObject.Instantiate(
               _spellPrefab,
               new Vector3(
                   Player.ShotPoint.position.x,
                   Player.ShotPoint.position.y + transformStartAmplifierY,
                   Player.ShotPoint.position.z),
               Quaternion.identity);

            _spell.Initialize(_particleSystem, Ability.CurrentDuration, Ability.SpellRadius);
            _spell.transform.position = Player.transform.position + new Vector3(SpellOffsetX, SpellOffsetY, SpellOffsetZ);

            _blastRotateCoroutine = CoroutineRunner.StartCoroutine(RotateSpell());
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
                _direction = (curretnTarget.position - Player.transform.position).normalized;
            }
            else
            {
                _direction = _throwPoint.forward;
            }

            _spell.Initialize(_particleSystem, Ability.CurrentDuration, Ability.SpellRadius);
            _blastThrowingCoroutine = CoroutineRunner.StartCoroutine(ThrowingBlast());
        }

        private void CreateAoESpell()
        {
            _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z),
                Quaternion.identity);

            _spell.Initialize(_particleSystem, Ability.CurrentDuration, Ability.SpellRadius);
        }

        private void CreateTargetSpell()
        {
            Vector3 targetPosition;

            if (TryFindEnemy(out Enemy enemy))
                targetPosition = enemy.transform.position;
            else
                targetPosition = Player.transform.position;

            _spell = GameObject.Instantiate(
                _spellPrefab,
                targetPosition,
                Quaternion.identity);

            _spell.Initialize(_particleSystem, Ability.CurrentDuration, Ability.SpellRadius);
        }

        public bool TryFindEnemy(out Enemy enemy)
        {
            Collider[] coliderEnemy = Physics.OverlapSphere(Player.transform.position, _searcheRadius);

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
            while (Ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                {
                    if (Ability.TypeAttackAbility == TypeAttackAbility.AoEAbility)
                    {
                        if (_spell.TryFindEnemys(out List<Enemy> enemies))
                        {
                            foreach (var enemy in enemies)
                            {
                                enemy.TakeDamage(Ability.DamageSource);
                            }
                        }
                    }
                    else
                    {
                        if (_spell.TryFindEnemy(out Enemy enemy))
                        {
                            enemy.TakeDamage(Ability.DamageSource);
                        }
                    }
                }

                yield return new WaitForSeconds(_currentDelayAttack);
            }
        }

        private IEnumerator ThrowingBlast()
        {
            while (Ability.IsAbilityEnded == false)
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

            while (Ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                {
                    _spell.transform.RotateAround(Player.transform.position + Vector3.up * verticalOffset, Vector3.up, _rotationSpeed * Time.deltaTime);
                    Vector3 direction = (_spell.transform.position - Player.transform.position).normalized;
                    _spell.transform.position = Player.transform.position + direction * distance + Vector3.up * verticalOffset;
                }

                yield return null;
            }
        }
    }
}