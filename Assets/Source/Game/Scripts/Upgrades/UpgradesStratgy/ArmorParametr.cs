using Assets.Source.Game.Scripts.Services;

public class ArmorParametr : IUpgradeStats, IRevertStats
{
    private float _currentArmorValue;

    public ArmorParametr(float baseArmor)
    {
        _currentArmorValue = baseArmor;
    }

    public float CurrentArmorValue => _currentArmorValue;

    public void Apply(float value)
    {
        _currentArmorValue += value;
    }

    public void Revent(float value)
    {
        _currentArmorValue -= value;
    }
}