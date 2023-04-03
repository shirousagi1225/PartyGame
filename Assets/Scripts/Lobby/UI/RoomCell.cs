using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomCell : MonoBehaviour
{
    private string roomName=null;
    private LobbyManager lobbyManager=null;

    [SerializeField] private Text roomNameText = null;
    [SerializeField] private Button joinBtn=null;

    public void SetInfo(LobbyManager lobbyManager,string roomName)
    {
        this.lobbyManager = lobbyManager;
        this.roomName=roomName;
        roomNameText.text = roomName;
    }

    public async void OnJoinBtnClicked()
    {
        await lobbyManager.JoinRoom(roomName);
    }
}
