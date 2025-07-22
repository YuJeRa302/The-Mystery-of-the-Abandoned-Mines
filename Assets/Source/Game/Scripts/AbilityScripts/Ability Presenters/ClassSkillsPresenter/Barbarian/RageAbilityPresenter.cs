using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class RageAbilityPresenter : AbilityPresenter
    {
        private List<Transform> _effectContainer = new List<Transform>();
        private List<PoolObject> _spawnedEffects = new();
        private Pool _pool;
        private PoolParticle _particleEffectPrefab;
        private bool _isAbilityUse;

        public RageAbilityPresenter(
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
            _particleEffectPrefab = abilityEffect;
            _effectContainer.Add(Player.WeaponPoint);
            _effectContainer.Add(Player.AdditionalWeaponPoint);

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
            BoostPlayer(_isAbilityUse);
        }

        protected override void OnCooldownValueReset(float value)
        {
            base.OnCooldownValueReset(value);
            (AbilityView as ClassSkillButtonView).SetInteractableButton(true);
        }

        protected override void OnGameResumed(bool state)
        {
            if (_isAbilityUse || Ability.IsAbilityUsed)
                base.OnGameResumed(state);
        }

        protected override void OnAbilityUsed(Ability ability)
        {
            _isAbilityUse = true;
            BoostPlayer(_isAbilityUse);
        }

        private void OnButtonSkillClick()
        {
            if (_isAbilityUse)
                return;

            _isAbilityUse = true;
            Ability.Use();
            (AbilityView as ClassSkillButtonView).SetInteractableButton(false);
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
                    if (particle.isActiveAndEnabled)
                        particle.ReturnObjectPool();
                }
            }
        }
    }
}