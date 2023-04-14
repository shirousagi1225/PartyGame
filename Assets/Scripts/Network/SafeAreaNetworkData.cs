using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaNetworkData : NetworkBehaviour
{
    [Networked, Capacity(5)] public NetworkDictionary<SafeAreaCode, Vector3> safeAreaDict => default;
    [Networked, Capacity(5)] public NetworkLinkedList<int> validSafeAreaList => default;
    [Networked, Capacity(5)] public NetworkLinkedList<NetworkObject> openSafeAreaList => default;
    [Networked] public TickTimer attackTimer { get; set; }

    [Networked(OnChanged = nameof(OnCaptionChanged))] private string caption { get; set; }
    [Networked] private int roomCount { get; set; }

    public int attackCycleTime;
    [SerializeField] private int safeAreaOpenTime;
    [SerializeField] private int attackStartTime;
    [SerializeField] private int attackEndTime;
    public string attackWarning;
    [SerializeField] private string safeAreaOpenHint;
    [SerializeField] private string attackHint;
    [SerializeField] private string warningRemove;

    private GameManager gameManager=null;

    public override void Spawned()
    {
        transform.SetParent(SafeAreaManager.Instance.transform);
        gameManager=GameManager.Instance;
    }

    public override void FixedUpdateNetwork()
    {
        if (gameManager.Runner.GameMode == GameMode.Host && !attackTimer.ExpiredOrNotRunning(gameManager.Runner))
        {
            if (attackTimer.RemainingTicks(gameManager.Runner) == safeAreaOpenTime*60)
            {
                Debug.Log(safeAreaOpenTime);
                SetCaption_RPC(safeAreaOpenHint, 0);
                EventHandler.CallSafeAreaOpenEvent();
            }
            else if (attackTimer.RemainingTicks(gameManager.Runner) == attackStartTime*60)
            {
                Debug.Log(attackStartTime);
                SetCaption_RPC(attackHint, 0);
            }
            else if (attackTimer.RemainingTicks(gameManager.Runner) == attackEndTime*60)
            {
                Debug.Log(attackEndTime);
                SetCaption_RPC(warningRemove, 0);
            }
            else if (attackTimer.RemainingTicks(gameManager.Runner) == 1f)
            {
                Debug.Log(1);
                EventHandler.CallSafeAreaCloseEvent();
            }
        }
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void SetAttackTimer_RPC(int time)
    {
        attackTimer = TickTimer.CreateFromTicks(gameManager.Runner,time*60);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void SetCaption_RPC(string caption, int roomCount)
    {
        this.caption= caption;
        this.roomCount= roomCount;
    }

    private static void OnCaptionChanged(Changed<SafeAreaNetworkData> changed)
    {
        EventHandler.CallUseSafeAreaUIEvent(changed.Behaviour.caption, changed.Behaviour.roomCount);
    }
}
