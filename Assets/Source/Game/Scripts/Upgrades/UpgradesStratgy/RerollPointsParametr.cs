using Assets.Source.Game.Scripts.Services;
using System;

namespace Assets.Source.Game.Scripts.Upgrades
{
    public class RerollPointsParametr : IUpgradeStats, IRevertStats
    {
        private int _rerollPoints;

        public RerollPointsParametr(float stardValue)
        {
            _rerollPoints = Convert.ToInt32(stardValue);
        }

        public int RerollPoints => _rerollPoints;

        public void Apply(float value)
        {
            _rerollPoints += Convert.ToInt32(value);
        }

        public void Revent(float value)
        {
            _rerollPoints -= Convert.ToInt32(value);
        }
    }
}