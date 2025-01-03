using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Boss : Enemy
    {
        [SerializeField] private float _additionalAttackDelay = 7f;
        [SerializeField] private float _additionalAttackRange = 7f;
        [SerializeField] private float _specilaAttackDelay = 10f;

        public float AdditionalAttackDelay => _additionalAttackDelay;
        public float AdditionalAttackRange => _additionalAttackRange;
        public float SpecilaAttackDelay => _specilaAttackDelay;
    }
}