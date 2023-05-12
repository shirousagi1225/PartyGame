using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetworkData : NetworkBehaviour
{
    [Networked] public int readyCount { get; set; }
    //注意：玩家最大數量有進行修改時,需修改玩家清單的容量大小
    [Networked, Capacity(12)] public NetworkDictionary<PlayerRef, NetworkObject> playerDict => default;

    public override void Spawned()
    {
        transform.SetParent(GameManager.Instance.transform);
        GameManager.Instance.gameNetworkData = this;
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void SetReadyCount_RPC(int count)
    {
        readyCount+= count;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void SetPlayerDict_RPC(PlayerRef playerRef, NetworkObject player)
    {
        playerDict.Add(playerRef, player);
    }
}
