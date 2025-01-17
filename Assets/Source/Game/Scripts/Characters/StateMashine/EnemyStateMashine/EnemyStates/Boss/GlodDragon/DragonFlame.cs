using Assets.Source.Game.Scripts;
using UnityEngine;

public class DragonFlame : MonoBehaviour
{
    [SerializeField] private int _damage;

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.PlayerHealth.TakeDamage(_damage);
        }
    }
}