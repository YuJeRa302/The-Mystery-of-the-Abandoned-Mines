using System;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private List<PoolObject> _poolObjects = new List<PoolObject>();

    public event Action GetPoolObject;

    public List<PoolObject> PoolObjects => _poolObjects;

    private void OnDisable()
    {
        foreach (var pollObject in _poolObjects)
        {
            pollObject.PoolReturned -= PoolObject;
        }
    }

    public void InstantiatePoolObject(PoolObject poolObject, string objectName)
    {
        poolObject.InitializeObject(objectName);
        poolObject.PoolReturned += PoolObject;
    }

    public void PoolObject(PoolObject poolObject)
    {
        _poolObjects.Add(poolObject);
        GetPoolObject?.Invoke();
    }

    public bool TryPoolObject(GameObject soughtObject, out PoolObject poolObject)
    {
        PoolObject result = null;

        foreach (var object1InPool in _poolObjects)
        {
            Debug.Log(object1InPool.NameObject);
            if (object1InPool.NameObject == soughtObject.name)
                result = object1InPool;
        }

        poolObject = result;
        _poolObjects.Remove(result);
        return poolObject != null;
    }
}