using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectPoolRoot : MonoBehaviour,INetworkObjectPool
{
    private Dictionary<object, NetworkObjectPool> _poolByPrefab=new Dictionary<object, NetworkObjectPool>();
    private Dictionary<NetworkObject, NetworkObjectPool> _poolByInstance = new Dictionary<NetworkObject, NetworkObjectPool>();

    public NetworkObjectPool GetPool<T>(T prefab) where T : NetworkObject
    {
        NetworkObjectPool pool;

        if(!_poolByPrefab.TryGetValue(prefab,out pool))
        {
            pool = new NetworkObjectPool();
            _poolByPrefab[prefab]=pool;
        }

        return pool;
    }

    public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
    {
        NetworkObject prefab;

        if(NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab,out prefab))
        {
            NetworkObjectPool pool=GetPool(prefab);
            NetworkObject newObj = pool.GetFromPool(Vector3.zero,Quaternion.identity);

            if (newObj == null)
            {
                newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                _poolByInstance[newObj]=pool;
            }

            newObj.gameObject.SetActive(true);

            /*foreach (var prefabs in _poolByPrefab.Keys)
                Debug.Log("prefabs : "+prefabs+ _poolByPrefab.Count);

            foreach (var instance in _poolByInstance.Keys)
                Debug.Log("instance : "+instance+ _poolByInstance.Count);*/
            
            return newObj;
        }

        Debug.LogError("No prefab for " + info.Prefab);
        return null;
    }

    public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
    {
        Debug.Log($"Releasing {instance} instance, isSceneObject={isSceneObject}");
        if (instance != null)
        {
            NetworkObjectPool pool;

            if (_poolByInstance.TryGetValue(instance,out pool))
            {
                pool.ReturnToPool(instance);
                instance.gameObject.SetActive(false);
                instance.transform.SetParent(transform, false);
                //Debug.Log("In to pool");
            }
            else
            {
                instance.gameObject.SetActive(false);
                instance.transform.SetParent(null, false);
                Destroy(instance.gameObject);
            }
        }
    }

    public void ClearPools()
    {
        foreach(NetworkObjectPool pool in _poolByPrefab.Values)
            pool.Clear();

        foreach (NetworkObjectPool pool in _poolByInstance.Values)
            pool.Clear();

        _poolByPrefab = new Dictionary<object, NetworkObjectPool>();
    }
}
