using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Services;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class FirestormPresenter : AbilityPresenter
    {
        private readonly float _delayAttack = 0.3f;
        private readonly float _blastSpeed = 12f;
        private readonly float _searchRadius = 20f;

        private LegendarySpell _spellPrefab;
        private LegendarySpell _spell;
        private Vector3 _direction;
        private Coroutine _blastThrowingCoroutine;
        private Coroutine _damageDealCoroutine;
        private Transform _throwPoint;
        private ParticleSystem _particleSystem;

        public FirestormPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            GamePauseService gamePauseService,
            GameLoopService gameLoopService,
            ICoroutineRunner coroutineRunner,
            ParticleSystem particleSystem,
            LegendarySpell spellPrefab) : base(ability, abilityView, player,
                gamePauseService, gameLoopService, coroutineRunner)
        {
            _particleSystem = particleSystem;
            _throwPoint = Player.ThrowAbilityPoint;
            _spellPrefab = spellPrefab;
            AddListener();
        }

        protected override void OnGamePaused(bool state)
        {
            base.OnGamePaused(state);

            if (_blastThrowingCoroutine != null)
                CoroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        protected override void OnGameResumed(bool state)
        {
            base.OnGameResumed(state);

            if (_blastThrowingCoroutine != null)
                _blastThrowingCoroutine = CoroutineRunner.StartCoroutine(ThrowingBlast());

            if (_damageDealCoroutine != null)
                _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
        }

        protected override void OnAbilityUsed(Ability ability)
        {
            ThrowBlast();

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
        }

        private void ThrowBlast()
        {
            _spell = Object.Instantiate(
                _spellPrefab,
                new Vector3(_throwPoint.transform.position.x, _throwPoint.transform.position.y,
                _throwPoint.transform.position.z),
                Quaternion.identity);

            if (TryFindEnemy(out Enemy enemy))
            {
                Transform currentTarget = enemy.transform;
                _direction = (currentTarget.position - Player.transform.position).normalized;
            }
            else
            {
                _direction = _throwPoint.forward;
            }

            _spell.Initialize(_particleSystem, Ability.CurrentDuration);
            _blastThrowingCoroutine = CoroutineRunner.StartCoroutine(ThrowingBlast());
        }

        public bool TryFindEnemy(out Enemy enemy)
        {
            Collider[] colliderEnemy = Physics.OverlapSphere(Player.transform.position,
                _searchRadius);

            foreach (Collider collider in colliderEnemy)
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
                yield return new WaitForSeconds(_delayAttack);

                if (_spell != null)
                    if (_spell.TryFindEnemy(out Enemy enemy))
                        enemy.TakeDamage(Ability.DamageSource);
            }
        }

        private IEnumerator ThrowingBlast()
        {
            while (Ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                    _spell.transform.Translate(_direction * _blastSpeed * Time.deltaTime);
                else
                    yield return null;

                yield return null;
            }
        }
    }
}