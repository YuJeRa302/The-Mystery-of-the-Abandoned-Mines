using Assets.Source.Game.Scripts.Characters;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Traps
{
    public abstract class Trap : MonoBehaviour
    {
        [SerializeField] protected int _damage;

        public int Damage => _damage;

        protected abstract void ApplyDamage(Player player);

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out Player player))
                ApplyDamage(player);
        }
    }
}