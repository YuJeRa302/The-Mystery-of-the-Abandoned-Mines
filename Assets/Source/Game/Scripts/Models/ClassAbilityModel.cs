using Assets.Source.Game.Scripts;
using System;

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

    public event Action<ClassAbilityState> InvokedAbilityUpgrade;
    public event Action<PlayerClassData> InvokedAbilityReset;

    public int Coins => _persistentDataService.PlayerProgress.Coins;

    public void ResetAbilities(int value)
    {
        _persistentDataService.PlayerProgress.Coins += value;
        InvokedAbilityReset?.Invoke(_currentPlayerClassData);
    }

    public ClassAbilityState GetClassAbilityState(ClassAbilityData classAbilityData)
    {
        return _persistentDataService.PlayerProgress.ClassAbilityService.GetClassAbilityStateById(classAbilityData.Id);
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

        if (_currentClassAbilityState.CurrentLevel >= _currentClassAbilityData.AbilityClassParameters.Count)
            return;

        if (Coins >= _currentClassAbilityData.AbilityClassParameters[_currentClassAbilityState.CurrentLevel].Cost)
        {
            _persistentDataService.TrySpendCoins(_currentClassAbilityData.AbilityClassParameters[_currentClassAbilityState.CurrentLevel].Cost);

            if (_currentClassAbilityState.CurrentLevel < _maxAbilityLevel)
                _currentClassAbilityState.CurrentLevel++;

            InvokedAbilityUpgrade?.Invoke(_currentClassAbilityState);
            _persistentDataService.PlayerProgress.ClassAbilityService.SetClassAbilityState(_currentClassAbilityState);
        }
        else
        {
            return;
        }
    }
}