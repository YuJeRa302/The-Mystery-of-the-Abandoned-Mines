using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class AttackAbilityPresenter : IDisposable
    {
        private readonly float _delayAttack = 0.3f;
        private readonly float _blastSpeed = 0.2f;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IGameLoopService _gameLoopService;

        private Ability _ability;
        private AbilityView _abilityView;
        private Spell _spellPrefab;
        private Spell _spell;
        private Player _player;
        private Vector3 _direction;
        private Coroutine _blastThrowingCoroutine;
        private Coroutine _damageDealCoroutine;
        private Transform _throwPoint;
        private ParticleSystem _particleSystem;

        public AttackAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            Transform throwPoint,
            ParticleSystem particleSystem,
            IGameLoopService gameLoopService,
            ICoroutineRunner coroutineRunner,
            Spell spellPrefab) 
        {
            _ability = ability;
            _abilityView = abilityView;
            _throwPoint = throwPoint;
            _particleSystem = particleSystem;
            _player = player;
            _spellPrefab = spellPrefab;
            _gameLoopService = gameLoopService;
            _coroutineRunner = coroutineRunner;
            AddListener();
        }

        public void Dispose()
        {
            if (_abilityView != null)
                _abilityView.ViewDestroy();

            RemoveListener();
            GC.SuppressFinalize(this);
        }

        protected virtual void AddListener()
        {
            _ability.AbilityUsed += OnAbilityUsed;
            _ability.AbilityEnded += OnAbilityEnded;
            _ability.AbilityUpgraded += OnAbilityUpgraded;
            _ability.CooldownValueChanged += OnCooldownValueChanged;
            _ability.CooldownValueReseted += OnCooldownValueReseted;
            _ability.AbilityRemoved += Dispose;
            _gameLoopService.GamePaused += OnGamePaused;
            _gameLoopService.GameResumed += OnGameResumed;
            _gameLoopService.GameClosed += OnGameClosed;
        }

        protected virtual void RemoveListener()
        {
            _ability.AbilityUsed -= OnAbilityUsed;
            _ability.AbilityEnded -= OnAbilityEnded;
            _ability.AbilityUpgraded -= OnAbilityUpgraded;
            _ability.CooldownValueChanged -= OnCooldownValueChanged;
            _ability.CooldownValueReseted -= OnCooldownValueReseted;
            _ability.AbilityRemoved -= Dispose;
            _gameLoopService.GamePaused -= OnGamePaused;
            _gameLoopService.GameResumed -= OnGameResumed;
            _gameLoopService.GameClosed -= OnGameClosed;
        }

        private void OnGameClosed()
        {
            Dispose();
        }

        private void OnGamePaused()
        {
            _ability.StopCoroutine();

            if (_blastThrowingCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        private void OnGameResumed()
        {
            _ability.Use();

            if (_blastThrowingCoroutine != null)
                _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());

            if (_damageDealCoroutine != null)
                _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        private void OnAbilityUsed(Ability ability) 
        {
            if (_ability.TypeAttackAbility == TypeAttackAbility.AoEAbility)
                CreateAoESpell();

            if (_ability.TypeAttackAbility == TypeAttackAbility.ProjectileAbility)
                ThrowBlast();

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        private void OnAbilityEnded(Ability ability) 
        {
            if (_blastThrowingCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        private void OnAbilityUpgraded(float delay)
        {
            _abilityView.Upgrade(delay);
        }

        private void OnCooldownValueChanged(float value) 
        {
            _abilityView.ChangeCooldownValue(value);
        }

        private void OnCooldownValueReseted(float value)
        {
            _abilityView.ResetCooldownValue(value);
        }

        private void ThrowBlast()
        {
            _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(_throwPoint.transform.position.x, _throwPoint.transform.position.y, _throwPoint.transform.position.z),
                Quaternion.identity);

            _direction = _throwPoint.transform.forward;
            _spell.Initialize(_particleSystem, _ability.CurrentDuration);
            _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());
        }

        private void CreateAoESpell()
        {
            _spell = GameObject.Instantiate(
                _spellPrefab,
                new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z),
                Quaternion.identity);

            _spell.Initialize(_particleSystem, _ability.CurrentDuration);
        }

        private IEnumerator DealDamage()
        {
            while (_ability.IsAbilityEnded == false)
            {
                yield return new WaitForSeconds(_delayAttack);

                if (_spell != null)
                {
                    if (_spell.TryFindEnemy(out Enemy enemy))
                        enemy.TakeDamage(_ability.CurrentAbilityValue);
                }
            }
        }

        private IEnumerator ThrowingBlast()
        {
            while (_ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                    _spell.transform.Translate(_direction * _blastSpeed);
                else
                    yield return null;

                yield return null;
            }
        }
    }
}