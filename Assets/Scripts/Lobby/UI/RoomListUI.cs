using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListUI : MonoBehaviour,IPanel
{
    [SerializeField] private LobbyManager lobbyManager = null;
    [SerializeField]private CanvasGroup canvasGroup = null;
    [SerializeField] private RoomCell roomCellPrefab = null;
    [SerializeField]private Transform contentTrans=null;

    //private List<RoomCell> roomCells = new List<RoomCell>();

    public void DisplayPanel(bool value)
    {
        canvasGroup.alpha = value ? 1 : 0;
        canvasGroup.interactable=value;
        canvasGroup.blocksRaycasts=value;
    }

    public void UpdateRoomList(List<SessionInfo> sessionList)
    {
        foreach(Transform child in contentTrans)
            Destroy(child.gameObject);

        //roomCells.Clear();

        foreach (var session in sessionList)
        {
            var roomCell = Instantiate(roomCellPrefab, contentTrans);
            roomCell.SetInfo(lobbyManager,session.Name);
        }
    }

    public void OnCreateRoomBtnClicked()
    {
        lobbyManager.SetPairState(PairState.CreatingRoom);
    }
}
