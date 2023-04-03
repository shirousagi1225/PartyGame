using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T>
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;

        DontDestroyOnLoad(gameObject);
    }

    public static bool IsInitialized
    {
        get { return instance != null; }
    }

    protected virtual void OnDestroy()
    {
        if (instance==this)
        {
            instance = null;
        }
    }
}