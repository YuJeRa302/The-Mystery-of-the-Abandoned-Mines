namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_ApplyBurnDamage
    {
        private float _damage;

        public M_ApplyBurnDamage(float damage)
        {
            _damage = damage;
        }

        public float Damage => _damage;
    }
}