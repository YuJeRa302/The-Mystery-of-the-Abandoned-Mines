using Assets.Source.Game.Scripts.Services;
using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class AmplifierAbilityPresenter : IDisposable
    {
        private readonly IGameLoopService _gameLoopService;

        private Ability _ability;
        private AbilityView _abilityView;
        private ParticleSystem _particleSystem;

        public AmplifierAbilityPresenter(Ability ability,
            AbilityView abilityView,
            ParticleSystem particleSystem,
            IGameLoopService gameLoopService)
        {
            _ability = ability;
            _abilityView = abilityView;
            _particleSystem = particleSystem;
            _gameLoopService = gameLoopService;
            AddListener();
        }

        public void Dispose()
        {
            if (_abilityView != null)
                _abilityView.ViewDestroy();

            RemoveListener();
            GC.SuppressFinalize(this);
        }

        private void AddListener()
        {
            _ability.AbilityUsed += OnAbilityUsed;
            _ability.AbilityEnded += OnAbilityEnded;
            _ability.AbilityUpgraded += OnAbilityUpgraded;
            _ability.CooldownValueChanged += OnCooldownValueChanged;
            _ability.CooldownValueReseted += CooldownValueReset;
            _ability.AbilityRemoved += Dispose;
            _gameLoopService.GamePaused += OnGamePaused;
            _gameLoopService.GameResumed += OnGameResumed;
            _gameLoopService.GameEnded += OnGameClosed;
        }

        private void RemoveListener()
        {
            _ability.AbilityUsed -= OnAbilityUsed;
            _ability.AbilityEnded -= OnAbilityEnded;
            _ability.AbilityUpgraded -= OnAbilityUpgraded;
            _ability.CooldownValueChanged -= OnCooldownValueChanged;
            _ability.CooldownValueReseted -= CooldownValueReset;
            _ability.AbilityRemoved -= Dispose;
            _gameLoopService.GamePaused -= OnGamePaused;
            _gameLoopService.GameResumed -= OnGameResumed;
            _gameLoopService.GameEnded -= OnGameClosed;
        }

        private void OnGameClosed(bool state)
        {
            Dispose();
        }

        private void OnGamePaused(bool state)
        {
            _ability.StopCoroutine();
        }

        private void OnGameResumed(bool state)
        {
            _ability.Use();
        }

        private void OnAbilityUpgraded(float delay)
        {
            _abilityView.Upgrade(delay);
        }

        private void OnAbilityUsed(Ability ability)
        {
            _particleSystem.Play();
        }

        private void OnAbilityEnded(Ability ability)
        {
            _particleSystem.Stop();
        }

        private void OnCooldownValueChanged(float value)
        {
            _abilityView.ChangeCooldownValue(value);
        }

        private void CooldownValueReset(float value)
        {
            _abilityView.ResetCooldownValue(value);
        }
    }
}