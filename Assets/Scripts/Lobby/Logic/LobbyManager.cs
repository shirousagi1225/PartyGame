using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private PlayerNetworkData playerNetworkDataPrefab = null;
    [SerializeField] private RoomListUI roomListUI = null;
    [SerializeField] private CreateRoomUI createRoomUI = null;
    [SerializeField] private InRoomUI inRoomUI = null;

    private PairState _pairState = PairState.Lobby;

    private async void Start()
    {
        SetPairState(PairState.Lobby);

        GameManager.Instance.Runner.AddCallbacks(this);
        await JoinLobby(GameManager.Instance.Runner);
    }

    public async Task JoinLobby(NetworkRunner runner)
    {
        var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);

        if(!result.Ok)
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
    }

    public async Task CreateRoom(string roomName, int maxPlayer)
    {
        var result = await GameManager.Instance.Runner.StartGame(new StartGameArgs()
        {
            GameMode=GameMode.Host,
            SessionName=roomName,
            PlayerCount=maxPlayer,
            Scene=SceneManager.GetActiveScene().buildIndex,
            SceneManager= GameManager.Instance.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            ObjectPool=ObjectPoolManager.Instance.gameObject.AddComponent<NetworkObjectPoolRoot>()
        });

        if (result.Ok)
            SetPairState(PairState.InRoom);
        else
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
    }

    public async Task JoinRoom(string roomName)
    {
        var result = await GameManager.Instance.Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = roomName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = GameManager.Instance.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            ObjectPool = ObjectPoolManager.Instance.gameObject.AddComponent<NetworkObjectPoolRoot>()
        });

        if (result.Ok)
            SetPairState(PairState.InRoom);
        else
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
    }

    public void SetPairState(PairState pairState)
    {
        _pairState = pairState;

        switch (_pairState)
        {
            case PairState.Lobby:
                SetPanel(roomListUI);
                break;
            case PairState.CreatingRoom:
                SetPanel(createRoomUI);
                break;
            case PairState.InRoom:
                SetPanel(inRoomUI);
                break;
        }
    }

    private void SetPanel(IPanel panel)
    {
        roomListUI.DisplayPanel(false);
        createRoomUI.DisplayPanel(false);
        inRoomUI.DisplayPanel(false);

        panel.DisplayPanel(true);
    }

    #region - Used Callbacks -
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        roomListUI.UpdateRoomList(sessionList);
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        runner.Spawn(playerNetworkDataPrefab, Vector3.zero, Quaternion.identity, player);
        Debug.Log(player.ToString());
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (GameManager.Instance.playerDict.TryGetValue(player,out PlayerNetworkData playerNetworkData))
        {
            runner.Despawn(playerNetworkData.Object);

            GameManager.Instance.playerDict.Remove(player);
            GameManager.Instance.UpdatePlayerList();
        }
    }
    #endregion

    #region - Unused callbacks -
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    #endregion
}
