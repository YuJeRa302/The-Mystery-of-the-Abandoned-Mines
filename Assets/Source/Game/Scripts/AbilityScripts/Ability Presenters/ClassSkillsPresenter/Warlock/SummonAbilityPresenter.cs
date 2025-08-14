using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class SummonAbilityPresenter : ClassAbilityPresenter
    {
        private Summon _summonPrefab;
        private Transform _spawnPoint;
        private Pool _pool;
        private Ability _ability;
        private Player _player;

        public override void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            base.Construct(abilityEntitiesHolder);
            SummonAbilityData summonAbilityData = abilityEntitiesHolder.AttributeData as SummonAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _player = abilityEntitiesHolder.Player;
            _pool = abilityEntitiesHolder.Player.Pool;
            _spawnPoint = abilityEntitiesHolder.Player.ShotPoint;
            _summonPrefab = summonAbilityData.Summon.Summon;
        }

        public override void UsedAbility(Ability ability)
        {
            base.UsedAbility(ability);

            for (int i = 0; i < ability.AbilityParameters[TypeParameter.Quantity]; i++)
            {
                Spawn();
            }
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