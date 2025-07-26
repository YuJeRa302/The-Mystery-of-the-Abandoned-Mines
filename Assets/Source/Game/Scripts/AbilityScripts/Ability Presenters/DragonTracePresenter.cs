using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class DragonTracePresenter : AbilityPresenter
    {
        private readonly float _delayAttack = 0.3f;
        private readonly float _delaySpawnSpell = 0.5f;

        private LegendarySpell _spellPrefab;
        private LegendarySpell _spell;
        private Coroutine _blastThrowingCoroutine;
        private Coroutine _damageDealCoroutine;
        private Coroutine _spawnedSpellCoroutine;
        private ParticleSystem _particleSystem;
        private List<LegendarySpell> _spells = new();

        public DragonTracePresenter(
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
            _spellPrefab = spellPrefab;
            AddListener();
        }

        protected override void OnGamePaused(bool state)
        {
            base.OnGamePaused(state);

            if (_spawnedSpellCoroutine != null)
                CoroutineRunner.StopCoroutine(_spawnedSpellCoroutine);

            if (_blastThrowingCoroutine != null)
                CoroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        protected override void OnGameResumed(bool state)
        {
            base.OnGameResumed(state);

            if (_spawnedSpellCoroutine != null)
                _spawnedSpellCoroutine = CoroutineRunner.StartCoroutine(SpawnSpell());

            if (_damageDealCoroutine != null)
                _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
        }

        protected override void OnAbilityUsed(Ability ability)
        {
            if (_spawnedSpellCoroutine != null)
                CoroutineRunner.StopCoroutine(_spawnedSpellCoroutine);

            _spawnedSpellCoroutine = CoroutineRunner.StartCoroutine(SpawnSpell());

            if (_damageDealCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
        }

        protected override void OnAbilityEnded(Ability ability)
        {
            if (_spawnedSpellCoroutine != null)
                CoroutineRunner.StopCoroutine(_spawnedSpellCoroutine);

            if (_blastThrowingCoroutine != null)
                CoroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        private IEnumerator SpawnSpell()
        {
            float lastTime = 0;

            while (lastTime < Ability.CurrentDuration)
            {
                _spell = Object.Instantiate(
                    _spellPrefab,
                    new Vector3(Player.transform.position.x, Player.transform.position.y,
                    Player.transform.position.z),
                    Quaternion.identity);

                _spell.Initialize(_particleSystem, Ability.CurrentDuration);
                _spells.Add(_spell);

                yield return new WaitForSeconds(_delaySpawnSpell);
                lastTime++;
            }
        }

        private IEnumerator DealDamage()
        {
            while (Ability.IsAbilityEnded == false)
            {
                yield return new WaitForSeconds(_delayAttack);

                if (_spells.Count > 0)
                {
                    foreach (var spell in _spells)
                    {
                        if (spell != null)
                        {
                            if (spell.TryFindEnemies(out List<Enemy> enemies))
                            {
                                foreach (var enemy in enemies)
                                {
                                    enemy.TakeDamage(Ability.DamageSource);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}