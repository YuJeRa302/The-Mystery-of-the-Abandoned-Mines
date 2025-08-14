using Assets.Source.Game.Scripts.Services;

public class AbilityCooldownReductionParameter : IUpgradeStats
{
    private float _currentReduction = 0;

    public float CurrentReduction => _currentReduction;

    public void Apply(float value)
    {
        _currentReduction = value;
    }
}