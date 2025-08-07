using Assets.Source.Game.Scripts.Utility;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class Boss : Enemy
    {
        [SerializeField] private float _additionalAttackDelay = 7f;
        [SerializeField] private float _specialAttackDelay = 10f;
        [SerializeField] private int _additionalAttackDamage;
        [SerializeField] private int _specialAttackDamage;

        public float AdditionalAttackDelay => _additionalAttackDelay;
        public float SpecialAttackDelay => _specialAttackDelay;
        public int SpecialAttackDamage => _specialAttackDamage;

        public override void ResetEnemy(int lvlRoom)
        {
            base.ResetEnemy(lvlRoom);

            _additionalAttackDamage = _additionalAttackDamage * (1 + lvlRoom / GameConstants.EnemyBoostDivider);
            _specialAttackDamage = _specialAttackDamage * (1 + lvlRoom / GameConstants.EnemyBoostDivider);
        }
    }
}