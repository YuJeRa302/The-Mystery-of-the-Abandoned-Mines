using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class SummonAbilityPresenter : IAbilityStrategy, IClassAbilityStrategy
    {
        private Summon _summonPrefab;
        private Transform _spawnPoint;
        private Pool _pool;
        private bool _isAbilityUse;
        private Ability _ability;
        private AbilityView _abilityView;
        private Player _player;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            SummonAbilityData summonAbilityData = abilityEntitiesHolder.AttributeData as SummonAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _abilityView = abilityEntitiesHolder.AbilityView;
            _player = abilityEntitiesHolder.Player;
            _pool = abilityEntitiesHolder.Player.Pool;
            _spawnPoint = abilityEntitiesHolder.Player.ShotPoint;
            _summonPrefab = summonAbilityData.Summon.Summon;
        }

        public void UsedAbility(Ability ability)
        {
            for (int i = 0; i < ability.Quantity; i++)
            {
                Spawn();
            }
        }

        public void EndedAbility(Ability ability)
        {
            _isAbilityUse = false;
        }

        public void SetInteractableButton()
        {
            (_abilityView as ClassSkillButtonView).SetInteractableButton(true);
        }

        public void AddListener()
        {
            (_abilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
        }

        public void RemoveListener()
        {
            (_abilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
        }

        private void OnButtonSkillClick()
        {
            if (_isAbilityUse)
                return;

            _isAbilityUse = true;
            _ability.Use();
            (_abilityView as ClassSkillButtonView).SetInteractableButton(false);
        }

        private void Spawn()
        {
            Summon summon;

            if (TryFindSummon(_summonPrefab.gameObject, out Summon poolSummon))
            {
                summon = poolSummon;
                summon.transform.position = _spawnPoint.position;
                summon.gameObject.SetActive(true);
                summon.ResetStats();
            }
            else
            {
                summon = GameObject.Instantiate(_summonPrefab, _spawnPoint.position, _spawnPoint.rotation);

                _pool.InstantiatePoolObject(summon, _summonPrefab.name);
                summon.Initialize(_player, _player.DamageSource, _ability.CurrentDuration);
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
    }
}