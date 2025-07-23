using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class ShieldUpAbilityPresenter : AbilityPresenter
    {
        private Coroutine _coroutine;
        private Transform _effectContainer;
        private Pool _pool;
        private PoolParticle _poolParticle;
        private List<PoolObject> _spawnedEffects = new();
        private bool _isAbilityUse;

        public ShieldUpAbilityPresenter(
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
            ChangeAbilityEffect(_isAbilityUse);
            Player.PlayerAnimation.UsedAbilityEnd();
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
            ActivateShield();
            ChangeAbilityEffect(_isAbilityUse);
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

        protected override void OnCooldownValueReset(float value)
        {
            base.OnCooldownValueReset(value);
            (AbilityView as ClassSkillButtonView).SetInteractableButton(true);
        }

        private void ActivateShield()
        {
            Player.PlayerAnimation.UseCoverAbility();
        }
    }
}