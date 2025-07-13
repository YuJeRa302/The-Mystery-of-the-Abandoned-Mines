using Assets.Source.Game.Scripts.Characters;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Traps
{
    public class Mine : Trap
    {
        [SerializeField] private GameObject _explosionEffect;

        private Coroutine _coroutine;

        protected override void ApplyDamage(Player player)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(Explode(player));
        }

        private IEnumerator Explode(Player player)
        {
            yield return new WaitForSeconds(1);
            Vector3 directionToTarget = transform.position - player.transform.position;
            float distance = directionToTarget.magnitude;

            if (distance <= 4f)
                player.TakeDamage(_damage);

            Instantiate(_explosionEffect, new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z),
                Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}