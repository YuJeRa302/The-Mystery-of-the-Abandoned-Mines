using System;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    private string _nameObject;

    public string NameObject => _nameObject;

    public event Action<PoolObject> PoolReturned;

    public void InitializeObject(string name)
    {
        _nameObject = name;
    }

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