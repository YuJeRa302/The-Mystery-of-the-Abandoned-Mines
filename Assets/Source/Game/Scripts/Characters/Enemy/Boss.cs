using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Boss : Enemy
    {
        [SerializeField] private float _additionalAttackDelay = 7f;

        public float AdditionalAttackDelay => _additionalAttackDelay;
    }
}