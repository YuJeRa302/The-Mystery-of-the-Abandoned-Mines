using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.PoolSystem
{
    public class Pool : MonoBehaviour
    {
        private List<PoolObject> _poolObjects = new List<PoolObject>();

        public event Action GetPoolObject;

        public List<PoolObject> PoolObjects => _poolObjects;

        private void OnDisable()
        {
            foreach (var poolObject in _poolObjects)
            {
                poolObject.PoolReturned -= PoolObject;
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

            foreach (var objectInPool in _poolObjects)
            {
                if (objectInPool.NameObject == soughtObject.name)
                    result = objectInPool;
            }

            poolObject = result;
            _poolObjects.Remove(result);
            return poolObject != null;
        }
    }
}