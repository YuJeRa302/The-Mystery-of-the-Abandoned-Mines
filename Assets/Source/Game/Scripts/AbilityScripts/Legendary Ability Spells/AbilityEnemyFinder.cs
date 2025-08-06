using Assets.Source.Game.Scripts.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class AbilityEnemyFinder : MonoBehaviour
    {
        [SerializeField] private float _findEnemyRange = 2f;

        private Collider[] _foundEnemyColliders = new Collider[50];

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _findEnemyRange);
        }

        public bool TryFindEnemy(out Enemy enemy)
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _findEnemyRange,
                _foundEnemyColliders
            );

            for (int i = 0; i < count; i++)
            {
                if (_foundEnemyColliders[i] != null &&
                    _foundEnemyColliders[i].TryGetComponent(out Enemy findedEnemy))
                {
                    enemy = findedEnemy;
                    return true;
                }

            }

            enemy = null;
            return false;
        }

        public bool TryFindEnemys(out List<Enemy> enemies)
        {
            enemies = new List<Enemy>();
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _findEnemyRange,
                _foundEnemyColliders
            );

            for (int i = 0; i < count; i++)
            {
                if (_foundEnemyColliders[i] != null &&
                    _foundEnemyColliders[i].TryGetComponent(out Enemy enemy))
                {
                    enemies.Add(enemy);
                }
            }

            return enemies.Count > 0;
        }
    }
}