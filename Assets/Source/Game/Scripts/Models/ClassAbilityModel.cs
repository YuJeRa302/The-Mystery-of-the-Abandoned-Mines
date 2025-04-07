using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;

public class ClassAbilityModel
{
    private readonly TemporaryData _temporaryData;
    private readonly int _minValue = 0;
    private readonly int _maxAbilityLevel = 3;

    private PlayerClassData _currentPlayerClassData;
    private ClassAbilityState _currentClassAbilityState;
    private ClassAbilityData _currentClassAbilityData;
    private List<ClassAbilityState> _classAbilityStates = new ();

    public ClassAbilityModel(TemporaryData temporaryData)
    {
        _temporaryData = temporaryData;
        

        if(_temporaryData.ClassAbilityStates != null)
            SetClassAbilityStates(_temporaryData.ClassAbilityStates);
    }

    public event Action<ClassAbilityState> InvokedAbilityUpgrade;
    public event Action<PlayerClassData> InvokedAbilityReset;

    public int Coins => _temporaryData.Coins;

    public void ResetAbilities(int value)
    {
        _temporaryData.AddCoins(value);
        InvokedAbilityReset?.Invoke(_currentPlayerClassData);
    }

    public ClassAbilityState GetClassAbilityState(ClassAbilityData classAbilityData)
    {
        ClassAbilityState classAbilityState = _temporaryData.GetClassAbilityState(classAbilityData.Id);

        if (classAbilityState == null)
            classAbilityState = InitState(classAbilityData);

        return classAbilityState;
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
            int cost = _currentClassAbilityData.AbilityClassParameters[_currentClassAbilityState.CurrentLevel].Cost;
            _temporaryData.TrySpendCoins(cost);

            if (_currentClassAbilityState.CurrentLevel < _maxAbilityLevel)
                _currentClassAbilityState.CurrentLevel++;

            InvokedAbilityUpgrade?.Invoke(_currentClassAbilityState);
        }
        else
        {
            return;
        }
    }

    public void UpdateTemporaryData()
    {
        _temporaryData.SetClassAbilityState(_classAbilityStates);
    }

    private ClassAbilityState InitState(ClassAbilityData classAbilityData)
    {
        ClassAbilityState classAbilityState = new (classAbilityData.Id, _minValue);
        _classAbilityStates.Add(classAbilityState);
        return classAbilityState;
    }

    private void SetClassAbilityStates(ClassAbilityState[] classAbilityStates) 
    {
        foreach (ClassAbilityState state in classAbilityStates)
        {
            _classAbilityStates.Add(state);
        }
    }
}