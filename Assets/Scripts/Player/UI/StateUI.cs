using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateUI : MonoBehaviour
{
    [SerializeField] private GameObject HPBar;
    [SerializeField] private Image weaponIcon;

    private void OnEnable()
    {
        EventHandler.StateUIUpdateEvent += OnStateUIUpdateEvent;
    }

    private void OnDisable()
    {
        EventHandler.StateUIUpdateEvent -= OnStateUIUpdateEvent;
    }

    //���AUI��s�ƥ�
    private void OnStateUIUpdateEvent(int hp, WeaponName weapon)
    {
        if(hp>=0)
            HPBar.transform.GetChild(hp).gameObject.SetActive(false);

        if(weapon!= WeaponName.None)
        {
            weaponIcon.sprite = ObjectPoolManager.Instance.GetWeaponData(weapon).weaponSprite;
            weaponIcon.SetNativeSize();
        }
    }
}
