using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Services;

namespace Assets.Source.Game.Scripts.Upgrades
{
    public class DamageParameterPlayer : IUpgradeStats, IRevertStats
    {
        private DamageSource _damageSource;

        public DamageParameterPlayer(DamageSource damageSource)
        {
            _damageSource = damageSource;
        }

        public void Apply(float value)
        {
            _damageSource.ChangeDamage(_damageSource.Damage + value);
        }

        public void Revent(float value)
        {
            _damageSource.ChangeDamage(_damageSource.Damage - value);
        }
    }
}