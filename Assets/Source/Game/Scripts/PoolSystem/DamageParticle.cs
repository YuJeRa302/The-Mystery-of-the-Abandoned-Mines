using Assets.Source.Game.Scripts;
using UnityEngine;

public class DamageParticle : PoolParticle
{
    private float _damage;
    private DamageParametr _damageParametr;

    public void Initialaze(float damage, DamageParametr damageParametr)
    {
        Debug.Log("Init");
        _damage = damage;
        _damageParametr = damageParametr;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            Debug.Log("Enter");
            //enemy.TakeDamage(_damage);
            enemy.TakeDamageTest(_damageParametr);
        }
    }
}