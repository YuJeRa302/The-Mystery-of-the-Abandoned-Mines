using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class AbilityEnemyFinder : MonoBehaviour
    {
        [SerializeField] private float _findEnemyRange = 2f;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _findEnemyRange);
        }

        public bool TryFindEnemy(out Enemy enemy)
        {
            Collider[] coliderEnemy = Physics.OverlapSphere(transform.position, _findEnemyRange);

            foreach (Collider collider in coliderEnemy)
            {
                if (collider.TryGetComponent(out enemy))
                {
                    Debug.Log(enemy.name);
                    return true;
                }

            }

            enemy = null;
            return false;
        }
    }
}