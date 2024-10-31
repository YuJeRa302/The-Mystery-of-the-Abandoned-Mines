using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private List<PoolObject> _poolObjects = new List<PoolObject>();

    public event Action GetPoolObject;

    private void OnDisable()
    {
        foreach (var pollObject in _poolObjects)
        {
            pollObject.PoolReturned -= PoolObject;
        }
    }

    public void InstantiatePoolObject(PoolObject poolObject)
    {
        poolObject.PoolReturned += PoolObject;
    }

    public void PoolObject(PoolObject poolObject)
    {
        _poolObjects.Add(poolObject);
        GetPoolObject?.Invoke();
    }

    public bool TryPoolObject(out PoolObject bullet)
    {
        bullet = _poolObjects.FirstOrDefault(p => p.gameObject.activeSelf == false);

        return bullet != null;
    }
}