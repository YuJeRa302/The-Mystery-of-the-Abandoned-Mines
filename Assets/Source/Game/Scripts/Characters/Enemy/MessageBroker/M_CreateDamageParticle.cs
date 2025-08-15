using Assets.Source.Game.Scripts.PoolSystem;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_CreateDamageParticle
    {
        private PoolParticle _poolParticle;

        public M_CreateDamageParticle(PoolParticle poolParticle)
        {
            _poolParticle = poolParticle;
        }

        public PoolParticle PoolParticle => _poolParticle;
    }
}