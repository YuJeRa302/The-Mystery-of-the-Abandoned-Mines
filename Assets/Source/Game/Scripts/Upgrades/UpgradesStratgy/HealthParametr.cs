using Assets.Source.Game.Scripts.Services;
using UniRx;

namespace Assets.Source.Game.Scripts.Upgrades
{
    public class HealthParametr : IUpgradeStats
    {
        public void Apply(float value)
        {
            if (value < 0)
                MessageBroker.Default.Publish(new M_HealthReduced(value));
            else
                MessageBroker.Default.Publish(new M_MaxHealthChanged(value));
        }
    }
}