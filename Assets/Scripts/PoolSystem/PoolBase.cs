using System.Collections.Generic;
using UnityEngine;

namespace PoolSystem
{
    public class PoolBase<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        private T prefab;
        
        private readonly Queue<T> _tPool = new ();
        private readonly HashSet<T> _activePooledObjects = new ();

        public T GetObject()
        {
            var pooledObject = _tPool.Count > 0 ? _tPool.Dequeue() : Instantiate(prefab, transform);
            _activePooledObjects.Add(pooledObject);
            OnGetObject(pooledObject);
            return pooledObject;
        }
        
        protected virtual void OnGetObject(T pooledObject)
        {
            pooledObject.gameObject.SetActive(true);
        }
        
        public void ReturnObject(T pooledObject)
        {
            OnReturnObject(pooledObject);
            _tPool.Enqueue(pooledObject);
        }
        
        protected virtual void OnReturnObject(T pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }

        public void ClearAllHighlights()
        {
            foreach (var pooledObject in _activePooledObjects)
            {
                pooledObject.gameObject.SetActive(false);
                ReturnObject(pooledObject);
            }
            _activePooledObjects.Clear();
        }
    }
}