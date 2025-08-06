using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class DragonSpell : MonoBehaviour
    {
        [SerializeField] private float _findPlayer = 4f;
        [SerializeField] private ParticleSystem _effect;

        private Collider[] _foundColliders = new Collider[50];

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _findPlayer);
        }

        public bool TryFindPlayer(out Player player)
        {
            _effect.Play();
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _findPlayer,
                _foundColliders
            );

            for (int i = 0; i < count; i++)
            {
                if (_foundColliders[i] != null &&
                    _foundColliders[i].TryGetComponent(out player))
                {
                    return true;
                }

            }

            player = null;
            return false;
        }
    }
}