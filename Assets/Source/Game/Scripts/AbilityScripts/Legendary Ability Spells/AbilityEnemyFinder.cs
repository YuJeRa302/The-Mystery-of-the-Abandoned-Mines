using Assets.Source.Game.Scripts.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
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
                    return true;

            }

            enemy = null;
            return false;
        }

        public bool TryFindEnemys(out List<Enemy> enemys)
        {
            enemys = new List<Enemy>();
            Collider[] coliderEnemy = Physics.OverlapSphere(transform.position, _findEnemyRange);

            foreach (Collider collider in coliderEnemy)
            {
                if (collider.TryGetComponent(out Enemy enemy))
                    enemys.Add(enemy);
            }

            return enemys.Count > 0;
        }
    }
}