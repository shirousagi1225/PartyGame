using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProp : NetworkBehaviour
{
    [SerializeField] private WeaponName weaponName;

    //¬B¨úªZ¾¹
    public void PickUpWeapon(NetworkObject localPlayer)
    {
        EventHandler.CallPickUpWeaponEvent(weaponName, localPlayer);
        GameManager.Instance.Runner.Despawn(Object);
    }
}
