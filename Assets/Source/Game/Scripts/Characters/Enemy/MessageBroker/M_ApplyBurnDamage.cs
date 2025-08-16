namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_ApplyBurnDamage
    {
        private float _damage;
        private EnemyDamageHandler _enemyDamageHandler;

        public M_ApplyBurnDamage(float damage, EnemyDamageHandler enemyDamageHandler)
        {
            _damage = damage;
            _enemyDamageHandler = enemyDamageHandler;
        }

        public EnemyDamageHandler EnemyDamageHandler => _enemyDamageHandler;
        public float Damage => _damage;
    }
}