using Assets.Source.Game.Scripts.PoolSystem;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_DisableParticle
    {
        private PoolParticle _poolParticle;
        private EnemyDamageHandler _enemyDamageHandler;

        public M_DisableParticle(PoolParticle poolParticle, EnemyDamageHandler enemyDamageHandler)
        {
            _poolParticle = poolParticle;
            _enemyDamageHandler = enemyDamageHandler;
        }

        public PoolParticle PoolParticle => _poolParticle;
        public EnemyDamageHandler EnemyDamageHandler => _enemyDamageHandler;
    }
}