using Assets.Source.Game.Scripts.PoolSystem;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_DisableParticle
    {
        private PoolParticle _poolParticle;

        public M_DisableParticle(PoolParticle poolParticle)
        {
            _poolParticle = poolParticle;
        }

        public PoolParticle PoolParticle => _poolParticle;
    }
}