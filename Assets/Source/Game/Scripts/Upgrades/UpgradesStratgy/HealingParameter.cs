using Assets.Source.Game.Scripts.Services;
using UniRx;

namespace Assets.Source.Game.Scripts.Upgrades
{
    public class HealingParameter : IUpgradeStats
    {
        public void Apply(float value)
        {
            MessageBroker.Default.Publish(new M_Healing(value));
        }
    }
}