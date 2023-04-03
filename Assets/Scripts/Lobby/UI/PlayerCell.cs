using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCell : MonoBehaviour
{
    [SerializeField] private Text playerNameText=null;
    [SerializeField] private Text isReadyText=null;

    public void SetInfo(string playerName,bool isReady)
    {
        playerNameText.text=playerName;
        isReadyText.text = isReady ? "Ready" : "";
    }
}
