using System;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public event Action<PoolObject> PoolReturned;

    public void ReturObjectPool()
    {
        ReturnToPool();
    }

    protected virtual void ReturnToPool()
    {
        gameObject.SetActive(false);
        PoolReturned?.Invoke(this);
    }
}