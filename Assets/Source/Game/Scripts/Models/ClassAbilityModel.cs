using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Game.Scripts.Views;
using UniRx;

namespace Assets.Source.Game.Scripts.Models
{
    public class ClassAbilityModel
    {
        private readonly PersistentDataService _persistentDataService;
        private readonly int _maxAbilityLevel = 3;

        private PlayerClassData _currentPlayerClassData;
        private ClassAbilityState _currentClassAbilityState;
        private ClassAbilityData _currentClassAbilityData;

        public ClassAbilityModel(PersistentDataService persistentDataService)
        {
            _persistentDataService = persistentDataService;
        }

        public int Coins => _persistentDataService.PlayerProgress.Coins;

        public void ResetAbilities(int value)
        {
            _persistentDataService.PlayerProgress.Coins += value;
            MessageBroker.Default.Publish(new M_AbilityReseted(_currentPlayerClassData));
        }

        public ClassAbilityState GetClassAbilityState(ClassAbilityData classAbilityData)
        {
            return _persistentDataService.PlayerProgress.
                ClassAbilityService.GetClassAbilityStateById(classAbilityData.Id);
        }

        public void SelectPlayerClass(PlayerClassData playerClassData)
        {
            _currentPlayerClassData = playerClassData;
        }

        public void SelectClassAbility(ClassAbilityDataView classAbilityDataView)
        {
            _currentClassAbilityState = classAbilityDataView.ClassAbilityState;
            _currentClassAbilityData = classAbilityDataView.ClassAbilityData;
        }

        public void UpgradeAbility()
        {
            if (_currentClassAbilityState == null)
                return;

            if (_currentClassAbilityState.CurrentLevel >= _maxAbilityLevel)
                return;

            if (!_persistentDataService.TrySpendCoins(
                    _currentClassAbilityData.AbilityClassParameters[_currentClassAbilityState.CurrentLevel].Cost))
                return;

            _currentClassAbilityState.ChangeCurrentLevel(_currentClassAbilityState.CurrentLevel + 1);

            _persistentDataService.PlayerProgress.
                ClassAbilityService.SetClassAbilityState(_currentClassAbilityState);

            MessageBroker.Default.Publish(new M_AbilityUpgraded(_currentClassAbilityState));
        }
    }
}