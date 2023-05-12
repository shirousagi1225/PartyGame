using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetworkData : NetworkBehaviour
{
    [Networked] public int readyCount { get; set; }
    //�`�N�G���a�̤j�ƶq���i��ק��,�ݭק缾�a�M�檺�e�q�j�p
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
