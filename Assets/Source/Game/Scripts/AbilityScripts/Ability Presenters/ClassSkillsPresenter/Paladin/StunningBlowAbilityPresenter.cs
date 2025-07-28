using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class StunningBlowAbilityPresenter : IAbilityStrategy, IClassAbilityStrategy
    {
        private Transform _effectContainer;
        private Pool _pool;
        private PoolParticle _poolParticle;
        private bool _isAbilityUse;
        private float _searchRadius = 4f;
        private Ability _ability;
        private AbilityView _abilityView;
        private Player _player;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            StunningBlowClassAbilityData stunningBlow = abilityEntitiesHolder.AttributeData as StunningBlowClassAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _abilityView = abilityEntitiesHolder.AbilityView;
            _player = abilityEntitiesHolder.Player;
            _poolParticle = stunningBlow.PoolParticle;
            _pool = abilityEntitiesHolder.Player.Pool;
            _effectContainer = abilityEntitiesHolder.Player.PlayerAbilityContainer;
        }

        public void UsedAbility(Ability ability)
        {
            _isAbilityUse = true;
            CastBlow();
        }

        public void EndedAbility(Ability ability)
        {
            _isAbilityUse = false;
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
            Collider[] colliderEnemy = Physics.OverlapSphere(_player.transform.position, _searchRadius);
            List<Enemy> enemies = new();

            foreach (Collider collider in colliderEnemy)
            {
                if (collider.TryGetComponent(out Enemy enemy))
                    enemies.Add(enemy);
            }

            foundEnemies = enemies;
            return foundEnemies != null;
        }
    }
}