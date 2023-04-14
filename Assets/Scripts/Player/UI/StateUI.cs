using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateUI : MonoBehaviour
{
    [SerializeField] private GameObject HPBar;

    private void OnEnable()
    {
        EventHandler.StateUIUpdateEvent += OnStateUIUpdateEvent;
    }

    private void OnDisable()
    {
        EventHandler.StateUIUpdateEvent -= OnStateUIUpdateEvent;
    }

    //狀態UI更新事件
    private void OnStateUIUpdateEvent(int hp)
    {
        HPBar.transform.GetChild(hp).gameObject.SetActive(false);
    }
}
