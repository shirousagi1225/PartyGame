using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskNetworkData : NetworkBehaviour
{
    [Networked,Capacity(12)] public NetworkLinkedList<ClothesName> clothesList =>default;
    [Networked, Capacity(12)] public NetworkLinkedList<FeatureName> initTaskList => default;
    [Networked, Capacity(12)] public NetworkDictionary<FeatureName,int> initTaskDict => default;
    [Networked, Capacity(12)] public NetworkLinkedList<FeatureName> changeTaskList => default;
    [Networked, Capacity(12)] public NetworkDictionary<FeatureName, int> changeTaskDict => default;

    public override void Spawned()
    {
        transform.SetParent(TaskManager.Instance.transform);
    }
}
