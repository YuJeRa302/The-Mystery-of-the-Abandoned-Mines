using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class DarkPactAbilityPresenter : ClassAbilityPresenter
    {
        private Transform _effectContainer;
        private Pool _pool;
        private PoolParticle _poolParticle;
        private List<PoolObject> _spawnedEffects = new ();
        private Player _player;

        public override void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            base.Construct(abilityEntitiesHolder);
            DarkPactAbilityData darkPactAbilityData = abilityEntitiesHolder.AttributeData as DarkPactAbilityData;
            _player = abilityEntitiesHolder.Player;
            _poolParticle = darkPactAbilityData.PoolParticle;
            _pool = _player.Pool;
            _effectContainer = _player.PlayerAbilityContainer;
        }

        public override void UsedAbility(Ability ability)
        {
            base.UsedAbility(ability);
            ChangeAbilityEffect(IsAbilityUse);
        }

        public override void EndedAbility(Ability ability)
        {
            base.EndedAbility(ability);
            ChangeAbilityEffect(IsAbilityUse);
        }

        private void ChangeAbilityEffect(bool isAbilityEnded)
        {
            if (isAbilityEnded)
            {
                PoolParticle particle;

                if (_pool.TryPoolObject(_poolParticle.gameObject, out PoolObject poolParticle))
                {
                    particle = poolParticle as PoolParticle;
                    particle.gameObject.SetActive(true);
                }
                else
                {
                    particle = Object.Instantiate(_poolParticle, _effectContainer);
                    _pool.InstantiatePoolObject(particle, _poolParticle.name);
                    _spawnedEffects.Add(particle);
                }
            }
            else
            {
                foreach (var particle in _spawnedEffects)
                {
                    if (particle != null)
                    {
                        if (particle.isActiveAndEnabled)
                            particle.ReturnObjectPool();
                    }
                }
            }
        }
    }
}