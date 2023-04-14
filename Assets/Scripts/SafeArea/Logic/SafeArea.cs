using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafeArea : NetworkBehaviour
{
    [SerializeField] private SafeAreaCode areaCode;
    [SerializeField] private float colRadius;
    [SerializeField] private Text roomCountText;

    [SerializeField,Networked(OnChanged = nameof(OnRoomCountChanged))] private int roomCount { get; set; }

    public override void FixedUpdateNetwork()
    {
        DetectTrigger();
    }

    private void DetectTrigger()
    {
        if(!Object.HasStateAuthority)
            return;

        var colliders = Physics.OverlapSphere(transform.position, colRadius, 1 << LayerMask.NameToLayer("Player"));

        if(roomCount!= colliders.Length)
            roomCount = colliders.Length;
    }

    private static void OnRoomCountChanged(Changed<SafeArea> changed)
    {
        changed.Behaviour.roomCountText.text = changed.Behaviour.roomCount.ToString();
        //Debug.Log(changed.Behaviour.roomCount);
        //EventHandler.CallSafeAreaRoomCountUpdateEvent();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, colRadius);
    }
}
