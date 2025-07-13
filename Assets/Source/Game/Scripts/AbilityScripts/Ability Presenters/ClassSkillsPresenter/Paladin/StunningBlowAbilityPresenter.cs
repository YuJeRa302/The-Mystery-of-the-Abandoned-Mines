using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class StunningBlowAbilityPresenter : AbilityPresenter
    {
        private Coroutine _coroutine;
        private Transform _effectContainer;
        private Pool _pool;
        private PoolParticle _poolParticle;
        private bool _isAbilityUse;
        private float _searchRadius = 4f;

        public StunningBlowAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            GamePauseService gamePauseService,
            GameLoopService gameLoopService,
            ICoroutineRunner coroutineRunner,
            PoolParticle abilityEffect) : base(ability, abilityView, player,
                gamePauseService, gameLoopService, coroutineRunner)
        {
            _pool = Player.Pool;
            _poolParticle = abilityEffect;
            _effectContainer = Player.PlayerAbilityContainer;
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
            if (_coroutine != null)
                CoroutineRunner.StopCoroutine(_coroutine);

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
            _isAbilityUse = true;
            CastBlow();
        }

        protected override void OnGameResumed(bool state)
        {
            base.OnGameResumed(state);
        }

        protected override void OnCooldownValueReset(float value)
        {
            base.OnCooldownValueReset(value);
            (AbilityView as ClassSkillButtonView).SetInteractableButton(true);
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
                    enemy.TakeDamage(Ability.DamageSource);
                }
            }
        }

        private bool TryFindEnemy(out List<Enemy> foundEnemies)
        {
            Collider[] colliderEnemy = Physics.OverlapSphere(Player.transform.position, _searchRadius);
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