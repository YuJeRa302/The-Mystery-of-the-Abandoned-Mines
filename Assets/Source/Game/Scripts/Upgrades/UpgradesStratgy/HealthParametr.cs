using Assets.Source.Game.Scripts.Services;
using UniRx;

public class HealthParametr : IUpgradeStats
{
    public HealthParametr()
    {
    }

    public void Apply(float value)
    {
        if (value < 0)
            MessageBroker.Default.Publish(new M_HealthReduced(value));
        else
            MessageBroker.Default.Publish(new M_MaxHealthChanged(value));
    }
}