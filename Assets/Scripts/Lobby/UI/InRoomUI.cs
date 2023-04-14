using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InRoomUI : MonoBehaviour,IPanel
{
    [SerializeField] private LobbyManager lobbyManager = null;
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] private Text roomNameText=null;
    [SerializeField] private PlayerCell playerCellPrefab = null;
    [SerializeField] private Transform contentTrans = null;

    private GameManager gameManager=null;
    private List<PlayerCell> playerCells = new List<PlayerCell>();

    private void OnEnable()
    {
        gameManager = GameManager.Instance;
        EventHandler.PlayerListUpdateEvent += OnPlayerListUpdateEvent;
    }

    private void OnDisable()
    {
        EventHandler.PlayerListUpdateEvent -= OnPlayerListUpdateEvent;
    }

    private void OnPlayerListUpdateEvent()
    {
        foreach(PlayerCell cell in playerCells)
            Destroy(cell.gameObject);

        playerCells.Clear();

        foreach (var player in gameManager.playerDict)
        {
            var playerCell = Instantiate(playerCellPrefab, contentTrans);
            playerCell.SetInfo(player.Value.playerName, player.Value.isReady);
            playerCells.Add(playerCell);
        }
    }

    public void DisplayPanel(bool value)
    {
        canvasGroup.alpha = value ? 1 : 0;
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }

    public void OnReadyBtnClicked()
    {
        if (gameManager.playerDict.TryGetValue(gameManager.Runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
            playerNetworkData.SetReady_RPC();
    }

    public void OnLeaveBtnClicked()
    {
        lobbyManager.SetPairState(PairState.CreatingRoom);
    }

    public void SetRoomName(string roomName)
    {
        roomNameText.text=roomName;
    }
}
