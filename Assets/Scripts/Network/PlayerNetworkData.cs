using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerNetworkData : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnPlayerNameChanged))] public string playerName { get; set; }
    [Networked(OnChanged = nameof(OnIsReadyChanged))] public NetworkBool isReady { get; set; }

    public override void Spawned()
    {
        transform.SetParent(GameManager.Instance.transform);
        GameManager.Instance.playerDict.Add(Object.InputAuthority,this);

        if (Object.HasInputAuthority)
        {
            SetPlayerName_RPC(GameManager.Instance.playerName);
        }
    }

    [Rpc(sources: RpcSources.InputAuthority,targets:RpcTargets.StateAuthority)]
    public void SetPlayerName_RPC(string name)
    {
        playerName = name;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void SetReady_RPC()
    {
        isReady = !isReady;
    }

    private static void OnPlayerNameChanged(Changed<PlayerNetworkData> changed)
    {
        GameManager.Instance.UpdatePlayerList();
    }

    private static void OnIsReadyChanged(Changed<PlayerNetworkData> changed)
    {
        GameManager.Instance.UpdatePlayerList();
    }
}
