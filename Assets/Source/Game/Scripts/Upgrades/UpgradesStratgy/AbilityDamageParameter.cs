using Assets.Source.Game.Scripts.Services;

public class AbilityDamageParameter : IUpgradeStats
{
    private float _currentDamage = 0;

    public float CurrentDamage => _currentDamage;

    public void Apply(float value)
    {
        _currentDamage = value;
    }
}