using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviour, IPanel
{
    [SerializeField] private LobbyManager lobbyManager = null;
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] private InputField roomNameInputField=null;
    [SerializeField] private InputField maxPlayerInputField = null;

    public void DisplayPanel(bool value)
    {
        canvasGroup.alpha = value ? 1 : 0;
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }

    public async void OnCreateBtnClicked()
    {
        await lobbyManager.CreateRoom(roomNameInputField.text,int.Parse(maxPlayerInputField.text));
    }

    public void OnBackBtnClicked()
    {
        lobbyManager.SetPairState(PairState.Lobby);
    }
}
