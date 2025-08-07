using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class StunningBlowAbilityPresenter : ClassAbilityPresenter
    {
        private Transform _effectContainer;
        private Pool _pool;
        private PoolParticle _poolParticle;
        private float _searchRadius = 4f;
        private Ability _ability;
        private Player _player;
        private Collider[] _foundEnemyColliders = new Collider[50];

        public override void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            base.Construct(abilityEntitiesHolder);
            StunningBlowClassAbilityData stunningBlow = abilityEntitiesHolder.AttributeData as StunningBlowClassAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _player = abilityEntitiesHolder.Player;
            _poolParticle = stunningBlow.PoolParticle;
            _pool = abilityEntitiesHolder.Player.Pool;
            _effectContainer = abilityEntitiesHolder.Player.PlayerAbilityContainer;
        }

        public override void UsedAbility(Ability ability)
        {
            base.UsedAbility(ability);
            CastBlow();
        }

        private void CastBlow()
        {
            CreateParticle();
            ApplyDamage();
        }

        private void CreateParticle()
        {
            PoolParticle particle;

            if (_pool.TryPoolObject(_poolParticle.gameObject, out PoolObject poolParticle))
            {
                particle = poolParticle as PoolParticle;
                particle.transform.position = _effectContainer.position;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = Object.Instantiate(_poolParticle, _effectContainer);
                _pool.InstantiatePoolObject(particle, _poolParticle.name);
            }
        }

        private void ApplyDamage()
        {
            if (TryFindEnemy(out List<Enemy> foundEnemies))
            {
                foreach (var enemy in foundEnemies)
                {
                    enemy.TakeDamage(_ability.DamageSource);
                }
            }
        }

        private bool TryFindEnemy(out List<Enemy> foundEnemies)
        {
            foundEnemies = new List<Enemy>();

            int count = Physics.OverlapSphereNonAlloc(
                _player.transform.position,
                _searchRadius,
                _foundEnemyColliders
            );

            for (int i = 0; i < count; i++)
            {
                if (_foundEnemyColliders[i] != null &&
                    _foundEnemyColliders[i].TryGetComponent(out Enemy enemy))
                {
                    foundEnemies.Add(enemy);
                }
            }

            return foundEnemies.Count > 0;
        }
    }
}