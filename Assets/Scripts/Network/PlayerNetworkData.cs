using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.VFX;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class PlayerNetworkData : NetworkBehaviour
{
    [Header("ª±®a³]¸m")]
    [SerializeField] private PlayerRef playerRef;
    [SerializeField] private int maxHp;

    [SerializeField] private float invisibilityTime;
    [SerializeField] private float invisibilityCDTime;
    [SerializeField] private float invisibilityStiffTime;

    public VisualEffect[] VFXs;
    public Material[] materials;

    [Networked(OnChanged = nameof(OnPlayerNameChanged))] public string playerName { get; set; }
    [Networked(OnChanged = nameof(OnIsReadyChanged))] public NetworkBool isReady { get; set; }
    [Networked] public NetworkBool isInitData { get; set; }
    [Networked(OnChanged = nameof(OnRemakeRoundChanged))] public NetworkBool isRemakeRound { get; set; }
    [Networked(OnChanged = nameof(OnHpChanged))] public int hp { get; set; }
    [Networked] public NetworkBool isHurt { get; set; }
    [Networked(OnChanged = nameof(OnClothesChanged))] public ClothesName clothes { get; set; }
    [Networked(OnChanged = nameof(OnTaskChanged))] public FeatureName task { get; set; }
    [Networked(OnChanged = nameof(OnWeaponChanged))] public WeaponName weapon { get; set; }
    [Networked(OnChanged = nameof(OnAniTypeChanged))] public AnimationType aniType { get; set; }
    [Networked(OnChanged = nameof(OnInvisibilityChanged))] public NetworkBool isInvisibility { get; set; }
    [Networked(OnChanged = nameof(OnDeadChanged))] public NetworkBool isDead { get; set; }

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

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void SetIsInitData_RPC(bool value)
    {
        isInitData = value;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetRemakeRound_RPC(bool value)
    {
        isRemakeRound = value;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void TakeDamage_RPC(int damage)
    {
        hp -= damage;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetIsHurt_RPC(bool value)
    {
        isHurt = value;
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

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetAniType_RPC(AnimationType aniType)
    {
        this.aniType = aniType;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetInvisibility_RPC(bool value)
    {
        isInvisibility= value;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetDead_RPC(bool value,ClothesName clothesName)
    {
        if(clothesName != ClothesName.None&& !CustomEventHandler.CallCheckTargetEvent(task, clothesName))
            isDead = true;
        else
            isDead = value;
    }

    private static void OnPlayerNameChanged(Changed<PlayerNetworkData> changed)
    {
        GameManager.Instance.UpdatePlayerList();
    }

    private static void OnIsReadyChanged(Changed<PlayerNetworkData> changed)
    {
        GameManager.Instance.UpdatePlayerList();
    }

    private static void OnRemakeRoundChanged(Changed<PlayerNetworkData> changed)
    {
        if (changed.Behaviour.isRemakeRound)
        {
            changed.Behaviour.RemakeData();
            changed.Behaviour.SetRemakeRound_RPC(false);
        }
    }

    private static void OnHpChanged(Changed<PlayerNetworkData> changed)
    {
        if (changed.Behaviour.hp == changed.Behaviour.maxHp)
            return;

        if (GameManager.Instance.playerDict[GameManager.Instance.Runner.LocalPlayer].playerName == changed.Behaviour.playerName)
            CustomEventHandler.CallStateUIUpdateEvent(changed.Behaviour.hp, WeaponName.None);

        if (changed.Behaviour.hp == 0)
        {
            CustomEventHandler.CallPlayerDeadEvent(changed.Behaviour.playerRef);

            CustomEventHandler.CallTaskUpdateEvent(changed.Behaviour);
        }     
    }

    private static void OnClothesChanged(Changed<PlayerNetworkData> changed)
    {
        CustomEventHandler.CallSetClothesEvent(changed.Behaviour.clothes, changed.Behaviour.materials);
    }

    private static void OnTaskChanged(Changed<PlayerNetworkData> changed)
    {
        if (GameManager.Instance.playerDict[GameManager.Instance.Runner.LocalPlayer].playerName== changed.Behaviour.playerName)
            CustomEventHandler.CallTaskUIUpdateEvent(changed.Behaviour.task);
    }

    private static void OnWeaponChanged(Changed<PlayerNetworkData> changed)
    {
        if (GameManager.Instance.playerDict[GameManager.Instance.Runner.LocalPlayer].playerName == changed.Behaviour.playerName)
            CustomEventHandler.CallStateUIUpdateEvent(-1, changed.Behaviour.weapon);
    }

    private static void OnAniTypeChanged(Changed<PlayerNetworkData> changed)
    {
        RendererManager.Instance.HeatDistortionCtrl(changed.Behaviour.VFXs, changed.Behaviour.aniType);
    }

    private static void OnInvisibilityChanged(Changed<PlayerNetworkData> changed)
    {
        if(changed.Behaviour.isInvisibility==true)
            RendererManager.Instance.InvisibilityEffect(changed.Behaviour.playerRef, changed.Behaviour, changed.Behaviour.VFXs, changed.Behaviour.materials
                , changed.Behaviour.invisibilityTime, changed.Behaviour.invisibilityCDTime, changed.Behaviour.invisibilityStiffTime);
    }

    private static void OnDeadChanged(Changed<PlayerNetworkData> changed)
    {
        if (changed.Behaviour.isDead)
            changed.Behaviour.TakeDamage_RPC(changed.Behaviour.hp);
    }

    private void RemakeData()
    {
        hp = maxHp;
        isHurt=false;
        weapon = WeaponName.Fist;
        isDead = false;

        if (GameManager.Instance.Runner.LocalPlayer == playerRef)
        {
            if(GameManager.Instance.Runner.GameMode == GameMode.Host)
                CustomEventHandler.CallRemakeRoundEvent();
        }
    }
}
