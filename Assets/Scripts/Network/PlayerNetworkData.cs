using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerNetworkData : NetworkBehaviour
{
    [SerializeField] private PlayerRef playerRef;
    [Networked(OnChanged = nameof(OnPlayerNameChanged))] public string playerName { get; set; }
    [Networked(OnChanged = nameof(OnIsReadyChanged))] public NetworkBool isReady { get; set; }

    [SerializeField] private int maxHp;

    [Networked(OnChanged = nameof(OnHpChanged))] public int hp { get; set; }
    [Networked] public ClothesName clothes { get; set; }
    [Networked(OnChanged = nameof(OnTaskChanged))] public FeatureName task { get; set; }
    [Networked(OnChanged = nameof(OnWeaponChanged))] public WeaponName weapon { get; set; }

    public override void Spawned()
    {
        transform.SetParent(GameManager.Instance.transform);
        playerRef = Object.InputAuthority;
        GameManager.Instance.playerDict.Add(Object.InputAuthority,this);

        if (Object.HasStateAuthority)
        {
            hp = maxHp;
            weapon = WeaponName.Fist;
        }
            
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

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void TakeDamage_RPC(int damage)
    {
        hp -= damage;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetClothes_RPC(ClothesName clothesName)
    {
        clothes = clothesName;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetTask_RPC(FeatureName featureName)
    {
        task = featureName;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetWeapon_RPC(WeaponName weaponName)
    {
        weapon = weaponName;
    }

    private static void OnPlayerNameChanged(Changed<PlayerNetworkData> changed)
    {
        GameManager.Instance.UpdatePlayerList();
    }

    private static void OnIsReadyChanged(Changed<PlayerNetworkData> changed)
    {
        GameManager.Instance.UpdatePlayerList();
    }

    private static void OnHpChanged(Changed<PlayerNetworkData> changed)
    {
        if (changed.Behaviour.hp == changed.Behaviour.maxHp)
            return;

        if (GameManager.Instance.playerDict[GameManager.Instance.Runner.LocalPlayer].playerName == changed.Behaviour.playerName)
            EventHandler.CallStateUIUpdateEvent(changed.Behaviour.hp,WeaponName.None);

        if (changed.Behaviour.hp == 0)
        {
            EventHandler.CallPlayerDeadEvent(changed.Behaviour.playerRef);

            EventHandler.CallTaskUpdateEvent(changed.Behaviour);
        }
            
    }

    private static void OnTaskChanged(Changed<PlayerNetworkData> changed)
    {
        if (GameManager.Instance.playerDict[GameManager.Instance.Runner.LocalPlayer].playerName== changed.Behaviour.playerName)
            EventHandler.CallTaskUIUpdateEvent(changed.Behaviour.task);
    }

    private static void OnWeaponChanged(Changed<PlayerNetworkData> changed)
    {
        if (GameManager.Instance.playerDict[GameManager.Instance.Runner.LocalPlayer].playerName == changed.Behaviour.playerName)
            EventHandler.CallStateUIUpdateEvent(-1, changed.Behaviour.weapon);
    }
}
