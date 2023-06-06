using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.VFX;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class PlayerNetworkData : NetworkBehaviour
{
    [Header("玩家設置(可更改)")]
    public PlayerRef playerRef;

    [SerializeField] private float maxHp;
    [SerializeField] private float HPRefreshRate;

    [SerializeField] private float maxSp;
    [SerializeField] private float SPReductionRate;
    [SerializeField] private float SPRecoveryRate;
    [SerializeField] private float SPRefreshRate;

    [SerializeField] private float invisibilityTime;
    [SerializeField] private float invisibilityCDTime;
    [SerializeField] private float invisibilityStiffTime;

    [Header("玩家設置(不可更改)")]
    public VisualEffect[] VFXs;
    public Material[] materials;
    public Animator playerAni;

    [Networked(OnChanged = nameof(OnPlayerNameChanged))] public string playerName { get; set; }
    [Networked(OnChanged = nameof(OnIsReadyChanged))] public NetworkBool isReady { get; set; }
    [Networked] public NetworkBool isInitData { get; set; }
    [Networked(OnChanged = nameof(OnRemakeRoundChanged))] public NetworkBool isRemakeRound { get; set; }
    [Networked(OnChanged = nameof(OnHpChanged))] public float hp { get; set; }
    [Networked] public float sp { get; set; }
    [Networked] public NetworkBool isHurt { get; set; }
    [Networked] public NetworkBool isAttack { get; set; }
    [Networked(OnChanged = nameof(OnClothesChanged))] public ClothesName clothes { get; set; }
    [Networked(OnChanged = nameof(OnTaskChanged))] public FeatureName task { get; set; }
    [Networked(OnChanged = nameof(OnWeaponChanged))] public WeaponName weapon { get; set; }
    [Networked(OnChanged = nameof(OnMoveAniTypeChanged))] public MoveAniType moveAniType { get; set; }
    [Networked(OnChanged = nameof(OnActionAniTypeChanged))] public ActionAniType actionAniType { get; set; }
    [Networked(OnChanged = nameof(OnInvisibilityChanged))] public NetworkBool isInvisibility { get; set; }
    [Networked(OnChanged = nameof(OnDeadChanged))] public NetworkBool isDead { get; set; }

    public override void Spawned()
    {
        transform.SetParent(GameManager.Instance.transform);
        playerRef = Object.InputAuthority;
        GameManager.Instance.playerDict.Add(Object.InputAuthority,this);

        sp = maxSp;

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
    public void TakeDamage_RPC(float damage)
    {
        hp -= damage;

        if (hp < 0)
            hp = 0;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void SetSP_RPC(float variation)
    {
        sp += variation;

        if (sp > maxSp)
            sp = maxSp;

        if(sp<0)
            sp = 0;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetIsHurt_RPC(bool value)
    {
        isHurt = value;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetIsAttack_RPC(bool value)
    {
        isAttack = value;
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
    public void SetMoveAniType_RPC(MoveAniType aniType)
    {
        moveAniType = aniType;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    public void SetActionAniType_RPC(ActionAniType aniType)
    {
        actionAniType = aniType;
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
        /*if (changed.Behaviour.hp == changed.Behaviour.maxHp)
            return;*/

        if (GameManager.Instance.playerDict[GameManager.Instance.Runner.LocalPlayer].playerName == changed.Behaviour.playerName)
            CustomEventHandler.CallHPUIUpdateEvent(changed.Behaviour.maxHp, changed.Behaviour.HPRefreshRate);

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

    private static void OnMoveAniTypeChanged(Changed<PlayerNetworkData> changed)
    {
        if(changed.Behaviour.moveAniType==MoveAniType.Jump)
            CustomEventHandler.CallSetJumpAniEvent(GameManager.Instance.gameNetworkData.playerDict[changed.Behaviour.playerRef].transform, changed.Behaviour.playerAni);

        if (GameManager.Instance.playerDict[GameManager.Instance.Runner.LocalPlayer].playerName == changed.Behaviour.playerName)
        {
            changed.LoadOld();

            var oldAniType = changed.Behaviour.moveAniType;

            changed.LoadNew();

            CustomEventHandler.CallSPUIUpdateEvent(oldAniType,changed.Behaviour.moveAniType, changed.Behaviour.maxSp, changed.Behaviour.SPReductionRate
                , changed.Behaviour.SPRecoveryRate, changed.Behaviour.SPRefreshRate);
        }

        CustomEventHandler.CallSetMoveAniEvent(changed.Behaviour, changed.Behaviour.playerAni, changed.Behaviour.moveAniType);
    }

    private static void OnActionAniTypeChanged(Changed<PlayerNetworkData> changed)
    {
        if(changed.Behaviour.actionAniType!=ActionAniType.None)
            CustomEventHandler.CallSetActionAniEvent(changed.Behaviour, changed.Behaviour.playerAni, changed.Behaviour.actionAniType);
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
        sp = maxSp;
        isHurt=false;
        weapon = WeaponName.Fist;
        moveAniType = MoveAniType.None;
        actionAniType=ActionAniType.None;
        isDead = false;

        if (GameManager.Instance.Runner.GameMode == GameMode.Host)
            CustomEventHandler.CallPlayerRebornEvent(playerRef);

        //適用於玩家殺對目標就重製回合
        /*if (GameManager.Instance.Runner.LocalPlayer == playerRef)
        {
            if(GameManager.Instance.Runner.GameMode == GameMode.Host)
                CustomEventHandler.CallRemakeRoundEvent();
        }*/
    }
}
