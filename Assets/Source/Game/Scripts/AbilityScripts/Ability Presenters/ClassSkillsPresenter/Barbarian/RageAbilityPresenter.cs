using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class RageAbilityPresenter : ClassAbilityPresenter
    {
        private List<Transform> _effectContainer = new ();
        private List<PoolObject> _spawnedEffects = new ();
        private Pool _pool;
        private PoolParticle _particleEffectPrefab;
        private Player _player;

        public override void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            base.Construct(abilityEntitiesHolder);
            RageClassAbilityData rageClassAbilityData = abilityEntitiesHolder.AttributeData as RageClassAbilityData;
            _player = abilityEntitiesHolder.Player;
            _pool = _player.Pool;
            _particleEffectPrefab = rageClassAbilityData.RageEffect;
            _effectContainer.Add(_player.WeaponPoint);
            _effectContainer.Add(_player.AdditionalWeaponPoint);
        }

        public override void UsedAbility(Ability ability)
        {
            base.UsedAbility(ability);
            BoostPlayer(IsAbilityUse);
        }

        public override void EndedAbility(Ability ability)
        {
            base.EndedAbility(ability);
            BoostPlayer(IsAbilityUse);
        }

        private void BoostPlayer(bool isAbilityEnded)
        {
            foreach (var container in _effectContainer)
            {
                ChangeEffectEnable(isAbilityEnded, container);
            }
        }

        private void ChangeEffectEnable(bool isAbilityEnded, Transform container)
        {
            if (isAbilityEnded)
            {
                PoolParticle particle;

                if (_pool.TryPoolObject(_particleEffectPrefab.gameObject, out PoolObject poolParticle))
                {
                    particle = poolParticle as PoolParticle;
                    particle.gameObject.SetActive(true);
                }
                else
                {
                    particle = Object.Instantiate(_particleEffectPrefab, container);
                    _pool.InstantiatePoolObject(particle, _particleEffectPrefab.name);
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