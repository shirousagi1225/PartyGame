using Example;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private Collider weaponCol;
    [SerializeField] private Vector3 colCenter;
    [SerializeField] private Vector3 colHalfExtents;

    public override void FixedUpdateNetwork()
    {
        //DetectTrigger();
    }

    public void DetectTrigger()
    {
        var colliders = Physics.OverlapBox(transform.position+colCenter, colHalfExtents, Quaternion.identity, 1 << LayerMask.NameToLayer("Player"));
        Debug.Log(colliders.ToString() + "+" + colliders.Length);

        if (colliders.Length>1)
        {
            foreach (var collider in colliders)
            {
                if(collider.TryGetComponent<ThirdPersonPlayer>(out ThirdPersonPlayer localPlayer))
                {
                    Debug.Log(collider.ToString() + "+" + colliders.Length);
                    Debug.Log(localPlayer + "+" + colliders.Length);
                    if (GameManager.Instance.playerDict.TryGetValue(localPlayer.Object.InputAuthority, out PlayerNetworkData playerNetworkData))
                        playerNetworkData.TakeDamage_RPC(1);
                }
            }
        }

        //weaponCol.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + colCenter, colHalfExtents);
    }
}
