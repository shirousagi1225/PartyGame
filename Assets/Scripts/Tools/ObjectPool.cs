using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool_test<T> where T : MonoBehaviour
{
    private Queue<T> _objectQueue;
    private GameObject _prefab;

    private static ObjectPool_test<T> instance = null;

    public static ObjectPool_test<T> Instance
    {
        get
        {
            if (instance == null)
                instance = new ObjectPool_test<T>();
            return instance;
        }
    }

    private int queueCount()
    {
        return _objectQueue.Count;
    }

    public void InitPool(GameObject prefab,int warmUpCount)
    {
        _prefab=prefab;
        _objectQueue=new Queue<T>();

        List<T> warmUpList = new List<T>();
        for (int i=0; i< warmUpCount;i++)
        {
            T t = Instance.Spawn(Vector3.zero,Quaternion.identity);
            warmUpList.Add(t);
        }

        for (int i = 0; i < warmUpList.Count; i++)
        {
            Instance.Recycle(warmUpList[i]);
        }
    }

    public T Spawn(Vector3 pos,Quaternion quaternion)
    {
        if (_prefab == null)
        {
            Debug.LogError(typeof(T).ToString()+" prefab not set!");
            return default(T);
        }

        if (queueCount()<=0)
        {
            GameObject gameObject = Object.Instantiate(_prefab,pos,quaternion);
            T t=gameObject.GetComponent<T>();

            if (t == null)
            {
                Debug.LogError(typeof(T).ToString() + " not found in prefab!");
                return default(T);
            }

            _objectQueue.Enqueue(t);
        }

        T obj= _objectQueue.Dequeue();
        obj.gameObject.transform.position=pos;
        obj.gameObject.transform.rotation=quaternion;
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void Recycle(T obj)
    {
        _objectQueue.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }
}
