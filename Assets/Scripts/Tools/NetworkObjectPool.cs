using ExitGames.Client.Photon;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectPool
{
    private List<NetworkObject> _free=new List<NetworkObject>();

    public NetworkObject GetFromPool(Vector3 pos,Quaternion quaternion,Transform parent = null)
    {
        NetworkObject newObj = null;

        //Debug.Log(_free+"¡G"+ _free.Count);

        while (_free.Count>0&& newObj == null)
        {
            var obj = _free[0];
            if (obj)
            {
                Transform trans = obj.transform;
                trans.SetParent(parent,false);
                trans.position = pos;
                trans.rotation = quaternion;
                newObj = obj;
            }
            else
                Debug.LogWarning("Recycled object was destroyed - not re-using!");

            _free.RemoveAt(0);
        }

        return newObj;
    }

    public void Clear()
    {
        foreach(var pool in _free)
        {
            if (pool)
            {
                Debug.Log($"Destroying pooled object: {pool.gameObject.name}");
                Object.Destroy(pool.gameObject);
            }
        }

        _free = new List<NetworkObject>();
    }

    public void ReturnToPool(NetworkObject obj)
    {
        _free.Add(obj);
    }
}
