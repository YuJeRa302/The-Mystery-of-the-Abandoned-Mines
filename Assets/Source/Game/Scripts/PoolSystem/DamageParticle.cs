using Assets.Source.Game.Scripts;
using UnityEngine;

public class DamageParticle : PoolParticle
{
    private float _damage;

    public void Initialaze(float damage)
    {
        Debug.Log("Init");
        _damage = damage;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            Debug.Log("Enter");
            enemy.TakeDamage(_damage);
        }
    }
}