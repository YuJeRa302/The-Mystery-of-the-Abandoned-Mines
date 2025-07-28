using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class RageAbilityPresenter : IAbilityStrategy, IClassAbilityStrategy
    {
        private List<Transform> _effectContainer = new ();
        private List<PoolObject> _spawnedEffects = new ();
        private Pool _pool;
        private PoolParticle _particleEffectPrefab;
        private bool _isAbilityUse = false;
        private Ability _ability;
        private AbilityView _abilityView;
        private Player _player;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            RageClassAbilityData rageClassAbilityData = abilityEntitiesHolder.AttributeData as RageClassAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _abilityView = abilityEntitiesHolder.AbilityView;
            _player = abilityEntitiesHolder.Player;
            _pool = _player.Pool;
            _particleEffectPrefab = rageClassAbilityData.RageEffect;
            _effectContainer.Add(_player.WeaponPoint);
            _effectContainer.Add(_player.AdditionalWeaponPoint);
        }

        public void UsedAbility(Ability ability)
        {
            BoostPlayer(_isAbilityUse);
        }

        public void EndedAbility(Ability ability)
        {
            _isAbilityUse = false;
            BoostPlayer(_isAbilityUse);
        }

        public void AddListener()
        {
            (_abilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
        }

        public void RemoveListener()
        {
            (_abilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
        }

        public void SetInteractableButton()
        {
            (_abilityView as ClassSkillButtonView).SetInteractableButton(true);
        }

        private void OnButtonSkillClick()
        {
            if (_isAbilityUse)
                return;

            _isAbilityUse = true;
            _ability.Use();
            (_abilityView as ClassSkillButtonView).SetInteractableButton(false);
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