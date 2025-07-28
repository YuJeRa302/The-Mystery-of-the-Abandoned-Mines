using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class DarkPactAbilityPresenter : IAbilityStrategy, IClassAbilityStrategy
    {
        private Transform _effectContainer;
        private Pool _pool;
        private PoolParticle _poolParticle;
        private List<PoolObject> _spawnedEffects = new ();
        private bool _isAbilityUse;
        private Ability _ability;
        private AbilityView _abilityView;
        private Player _player;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            DarkPactAbilityData darkPactAbilityData = abilityEntitiesHolder.AttributeData as DarkPactAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _abilityView = abilityEntitiesHolder.AbilityView;
            _player = abilityEntitiesHolder.Player;
            _poolParticle = darkPactAbilityData.PoolParticle;
            _pool = _player.Pool;
            _effectContainer = _player.PlayerAbilityContainer;
        }

        public void UsedAbility(Ability ability)
        {
            _isAbilityUse = true;
            ChangeAbilityEffect(_isAbilityUse);
        }

        public void EndedAbility(Ability ability)
        {
            _isAbilityUse = false;
            ChangeAbilityEffect(_isAbilityUse);
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
                    if (particle.isActiveAndEnabled)
                        particle.ReturnObjectPool();
                }
            }
        }
    }
}