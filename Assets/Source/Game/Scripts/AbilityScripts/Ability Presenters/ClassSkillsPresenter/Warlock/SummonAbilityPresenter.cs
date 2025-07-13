using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class SummonAbilityPresenter : AbilityPresenter
    {
        private Summon _summonPrefab;
        private Transform _spawnPoint;
        private Pool _pool;
        private bool _isAbilityUse;

        public SummonAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            GamePauseService gamePauseService,
            GameLoopService gameLoopService,
            ICoroutineRunner coroutineRunner,
            Summon summonPrefab) : base(ability, abilityView, player,
                gamePauseService, gameLoopService, coroutineRunner)
        {
            _pool = Player.Pool;
            _summonPrefab = summonPrefab;
            _spawnPoint = Player.ShotPoint;
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
            for (int i = 0; i < Ability.Quantity; i++)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            Summon summon = null;

            if (TryFindSummon(_summonPrefab.gameObject, out Summon poolSummon))
            {
                summon = poolSummon;
                summon.transform.position = _spawnPoint.position;
                summon.gameObject.SetActive(true);
                summon.ResetStats();
            }
            else
            {
                summon = Object.Instantiate(_summonPrefab, _spawnPoint.position, _spawnPoint.rotation);

                _pool.InstantiatePoolObject(summon, _summonPrefab.name);
                summon.Initialize(Player, Player.DamageSource, Ability.CurrentDuration);
            }
        }

        private bool TryFindSummon(GameObject summonPrefab, out Summon poolSummon)
        {
            poolSummon = null;

            if (_pool.TryPoolObject(summonPrefab, out PoolObject enemyPool))
            {
                poolSummon = enemyPool as Summon;
            }

            return poolSummon != null;
        }

        protected override void OnCooldownValueReset(float value)
        {
            base.OnCooldownValueReset(value);
            (AbilityView as ClassSkillButtonView).SetInteractableButton(true);
        }
    }
}