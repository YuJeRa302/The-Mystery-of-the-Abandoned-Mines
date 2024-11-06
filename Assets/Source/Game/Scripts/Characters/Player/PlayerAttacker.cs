using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    private const float SearchRadius = 5f;

    [SerializeField] private float _damage;

    private float _attackDelay = 2f;
    private float _timeAfterLastAttack = 0f;
    private Enemy _currentTarget;
    private Dictionary<float, Enemy> _enemies = new Dictionary<float, Enemy>();

    public event Action Attacked;

    private void Update()
    {
        _timeAfterLastAttack += Time.deltaTime;

        if (_timeAfterLastAttack >= _attackDelay)
        {
            FindTarget();
            _timeAfterLastAttack = 0;
        }
    }

    private void FindTarget()
    {
        _currentTarget = null;

        var colliders = Physics.OverlapSphere(transform.position, SearchRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out Enemy enemy))
            {
                float distanceToTarget = Vector3.Distance(enemy.transform.position, transform.position);

                if (distanceToTarget <= SearchRadius)
                    _enemies.Add(distanceToTarget, enemy);
            }
        }

        if (_enemies.Count == 0)
        {
            return;
        }
        else
        {
            _currentTarget = _enemies.OrderBy(distance => distance.Key).First().Value;

            if(_currentTarget != null && _currentTarget.isActiveAndEnabled == true)
            {
                GetHit();
            }
        }
    }

    private void GetHit()
    {
        _currentTarget.TakeDamage(_damage);
        Attacked?.Invoke();
    }
}