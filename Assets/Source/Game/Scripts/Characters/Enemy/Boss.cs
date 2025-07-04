using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Boss : Enemy
    {
        [SerializeField] private float _additionalAttackDelay = 7f;
        [SerializeField] private float _additionalAttackRange = 7f;
        [SerializeField] private float _specilaAttackDelay = 10f;
        [SerializeField] private int _additionalAttackDamage;
        [SerializeField] private int _specilAttackDamage;

        public float AdditionalAttackDelay => _additionalAttackDelay;
        public float SpecilaAttackDelay => _specilaAttackDelay;
        public int SpecilAttackDamage => _specilAttackDamage;

        public override void ResetEnemy(int lvlRoom)
        {
            base.ResetEnemy(lvlRoom);

            _additionalAttackDamage = _additionalAttackDamage * (1 + lvlRoom / GameConstants.EnemyBoostDivider);
            _specilAttackDamage = _specilAttackDamage * (1 + lvlRoom / GameConstants.EnemyBoostDivider);
        }
    }
}