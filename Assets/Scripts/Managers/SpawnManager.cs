using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using Example;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class SpawnManager : MonoBehaviour,INetworkRunnerCallbacks
{
    [SerializeField] private NetworkObject playerPrefab = null;
    public GameObject playerSpawnPoints;
    [SerializeField] private List<Transform> playerSpawnPointList;

    private NetworkRunner networkRunner = null;
    private Dictionary<PlayerRef, NetworkObject> playerDict = new Dictionary<PlayerRef, NetworkObject>();
    private HashSet<int> playerSpawnedPoints = new HashSet<int>();

    private float defSpeedMultiplier=0;

    private void Start()
    {
        networkRunner=GameManager.Instance.Runner;

        for (int i = 0; i < playerSpawnPoints.transform.childCount; i++)
            playerSpawnPointList.Add(playerSpawnPoints.transform.GetChild(i));

        StartGame(GameMode.AutoHostOrClient);
    }

    private void OnEnable()
    {
        EventHandler.PlayerStiffEvent += OnPlayerStiffEvent;
        EventHandler.PlayerDeadEvent += OnPlayerDeadEvent;
    }

    private void OnDisable()
    {
        EventHandler.PlayerStiffEvent -= OnPlayerStiffEvent;
        EventHandler.PlayerDeadEvent -= OnPlayerDeadEvent;
    }

    //���a�w���ƥ�
    //�u�୭���,��L���a�欰(���D ����)�٨S�i�歭��,�ݦbThirdPersonPlayer�i��P�w
    private void OnPlayerStiffEvent(PlayerRef playerRef,bool isStiff)
    {
        /*foreach (var player in playerDict.Keys)
            Debug.Log(player.ToString()+ playerDict.Count);*/

        if (GameManager.Instance.Runner.GameMode == GameMode.Host)
        {
            if (defSpeedMultiplier == 0&& playerDict[playerRef].TryGetComponent<ThirdPersonPlayer>(out ThirdPersonPlayer player))
                defSpeedMultiplier = player.SpeedMultiplier;

            if (isStiff)
                playerDict[playerRef].GetComponent<ThirdPersonPlayer>().SpeedMultiplier = 0f;
            else
                playerDict[playerRef].GetComponent<ThirdPersonPlayer>().SpeedMultiplier = defSpeedMultiplier;
        }
    }

    //���a���`�ƥ�
    private void OnPlayerDeadEvent(PlayerRef playerRef)
    {
        if (GameManager.Instance.Runner.GameMode == GameMode.Host)
            DespawnPlayer(playerRef);
    }

    async void StartGame(GameMode mode)
    {
        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        SpawnAllPlayers();
    }

    //�ͦ����a
    private void SpawnAllPlayers()
    {
        int pointNum;

        foreach (var player in GameManager.Instance.playerDict.Keys)
        {
            while (playerSpawnedPoints.Contains(pointNum = UnityEngine.Random.Range(0, playerSpawnPointList.Count)))
                continue;

            NetworkObject networkPlayerObject = networkRunner.Spawn(playerPrefab, playerSpawnPointList[pointNum].position, Quaternion.identity,player);
            networkRunner.SetPlayerObject(player, networkPlayerObject);
            playerSpawnedPoints.Add(pointNum);
            playerDict.Add(player, networkPlayerObject);
        }

        //�}�l�C���ƥ�
        EventHandler.CallStartGameEvent(networkRunner, playerDict.Count);
    }

    //�������a
    private void DespawnPlayer(PlayerRef playerRef)
    {
        if(playerDict.Count>1)
            networkRunner.Despawn(playerDict[playerRef]);
    }

    #region - Used Callbacks -
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (GameManager.Instance.playerDict.TryGetValue(player, out PlayerNetworkData playerNetworkData))
        {
            runner.Despawn(playerNetworkData.Object);

            GameManager.Instance.playerDict.Remove(player);
        }
    }
    #endregion

    #region - Unused callbacks -
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
    #endregion
}
