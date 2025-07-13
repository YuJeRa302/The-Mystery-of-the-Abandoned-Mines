using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.PoolSystem
{
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
}