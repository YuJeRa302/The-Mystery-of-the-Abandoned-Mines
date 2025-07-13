using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class JerkFrontAbilityPresenter : AbilityPresenter
    {
        private Coroutine _coroutine;
        private Rigidbody _rigidbodyPlayer;
        private Transform _effectContainer;
        private Pool _pool;
        private PoolParticle _poolParticle;
        private List<PoolObject> _spawnedEffects = new();
        private bool _isAbilityUse;

        public JerkFrontAbilityPresenter(
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
            _rigidbodyPlayer = Player.GetComponent<Rigidbody>();
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

        protected override void OnAbilityUsed(Ability ability)
        {
            ChangeAbilityEffect(_isAbilityUse);
            Jerk();
        }

        protected override void OnAbilityEnded(Ability ability)
        {
            if (_coroutine != null)
                CoroutineRunner.StopCoroutine(_coroutine);

            _isAbilityUse = false;
            ChangeAbilityEffect(_isAbilityUse);
        }

        protected override void OnCooldownValueReset(float value)
        {
            base.OnCooldownValueReset(value);
            (AbilityView as ClassSkillButtonView).SetInteractableButton(true);
        }

        private void OnButtonSkillClick()
        {
            if (_isAbilityUse)
                return;

            _isAbilityUse = true;
            Ability.Use();
            (AbilityView as ClassSkillButtonView).SetInteractableButton(false);
        }

        private void Jerk()
        {
            if (_coroutine != null)
                CoroutineRunner.StopCoroutine(_coroutine);

            _coroutine = CoroutineRunner.StartCoroutine(JerkForward());

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
                    (particle as DamageParticle).Initialize(Ability.DamageSource);
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

        private IEnumerator JerkForward()
        {
            float currentTime = 0;

            while (currentTime <= Ability.CurrentDuration)
            {
                _rigidbodyPlayer.AddForce(Player.transform.forward * Ability.CurrentAbilityValue, ForceMode.Impulse);
                currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}