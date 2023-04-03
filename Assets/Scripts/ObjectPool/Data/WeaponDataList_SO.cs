using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataList_SO", menuName = "Weapon/WeaponDataList_SO")]
public class WeaponDataList_SO : ScriptableObject
{
    public List<WeaponDetails> weaponDetailsList;

    public WeaponDetails GetWeaponDetails(WeaponName weaponName)
    {
        return weaponDetailsList.Find(i => i.weaponName == weaponName);
    }
}

[System.Serializable]
public class WeaponDetails
{
    public WeaponName weaponName;
    public GameObject weaponType;
}
