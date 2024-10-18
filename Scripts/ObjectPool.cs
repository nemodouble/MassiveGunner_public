using System.Collections.Generic;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class ObjectPool : MonoBehaviour
    {
        public GameObject prefab;
        public int initialPoolSize = 1000;
        private Queue<GameObject> objectPool = new Queue<GameObject>();
        

        
        void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject obj = Instantiate(prefab, transform, true);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
        }

        public GameObject GetObject()
        {
            if (objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = Instantiate(prefab);
                return obj;
            }
        }

        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
            objectPool.Enqueue(obj);
            obj.transform.SetParent(transform, false);
        }
    }
}