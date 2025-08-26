namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_HealedVampirism
    {
        private float _heal;

        public M_HealedVampirism(float heal)
        {
            _heal = heal;
        }

        public float Heal => _heal;
    }
}