using System;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    private string _nameObject;

    public string NameObject => _nameObject;

    public event Action<PoolObject> PoolReturned;

    public virtual void InitializeObject(string name)
    {
        _nameObject = name;
    }

    public virtual void ReturnObjectPool()
    {
        ReturnToPool();
    }

    protected virtual void ReturnToPool()
    {
        gameObject.SetActive(false);
        PoolReturned?.Invoke(this);
    }
}