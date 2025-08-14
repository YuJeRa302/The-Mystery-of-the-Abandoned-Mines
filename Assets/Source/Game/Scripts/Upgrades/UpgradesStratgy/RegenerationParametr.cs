using Assets.Source.Game.Scripts.Services;

public class RegenerationParametr : IUpgradeStats, IRevertStats
{
    private float _currentRegenerationValue;

    public RegenerationParametr(float baseValue)
    {
        _currentRegenerationValue = baseValue;
    }

    public float CurrentRegenerationValue => _currentRegenerationValue;

    public void Apply(float value)
    {
        _currentRegenerationValue += value;
    }

    public void Revent(float value)
    {
        _currentRegenerationValue -= value;
    }
}