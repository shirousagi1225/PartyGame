using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using Example;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class SpawnManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkObject playerPrefab = null;
    public GameObject playerSpawnPoints;
    [SerializeField] private List<Transform> playerSpawnPointList;
    
    //private Dictionary<PlayerRef, NetworkObject> playerDict = new Dictionary<PlayerRef, NetworkObject>();
    private GameManager gameManager;
    private HashSet<int> playerSpawnedPoints = new HashSet<int>();

    private float defSpeedMultiplier=0;
    private bool isStartGame;

    private void Start()
    {
        gameManager = GameManager.Instance;

        for (int i = 0; i < playerSpawnPoints.transform.childCount; i++)
            playerSpawnPointList.Add(playerSpawnPoints.transform.GetChild(i));

        StartGame(GameMode.AutoHostOrClient);
    }

    private void OnEnable()
    {
        CustomEventHandler.PlayerStiffEvent += OnPlayerStiffEvent;
        CustomEventHandler.PlayerDeadEvent += OnPlayerDeadEvent;
        CustomEventHandler.PlayerRebornEvent += OnPlayerRebornEvent;
        CustomEventHandler.RemakeRoundEvent += OnRemakeRoundEvent;
    }

    private void OnDisable()
    {
        CustomEventHandler.PlayerStiffEvent -= OnPlayerStiffEvent;
        CustomEventHandler.PlayerDeadEvent -= OnPlayerDeadEvent;
        CustomEventHandler.PlayerRebornEvent -= OnPlayerRebornEvent;
        CustomEventHandler.RemakeRoundEvent -= OnRemakeRoundEvent;
    }

    //���a�w���ƥ�
    //�u�୭���,��L���a�欰(���D ����)�٨S�i�歭��,�ݦbThirdPersonPlayer�i��P�w
    private void OnPlayerStiffEvent(PlayerRef playerRef,bool isStiff)
    {
        /*foreach (var player in playerDict.Keys)
            Debug.Log(player.ToString()+ playerDict.Count);*/

        if (gameManager.Runner.GameMode == GameMode.Host)
        {
            if (defSpeedMultiplier == 0&& gameManager.gameNetworkData.playerDict[playerRef].TryGetComponent<ThirdPersonPlayer>(out ThirdPersonPlayer player))
                defSpeedMultiplier = player.SpeedMultiplier;

            if (isStiff)
                gameManager.gameNetworkData.playerDict[playerRef].GetComponent<ThirdPersonPlayer>().SpeedMultiplier = 0f;
            else
            {
                gameManager.gameNetworkData.playerDict[playerRef].GetComponent<ThirdPersonPlayer>().SpeedMultiplier = defSpeedMultiplier;

                gameManager.playerDict[playerRef].SetActionAniType_RPC(ActionAniType.None);
            }     
        }
    }

    //���a���`�ƥ�
    private void OnPlayerDeadEvent(PlayerRef playerRef)
    {
        if (gameManager.Runner.GameMode == GameMode.Host)
            DespawnPlayer(playerRef);
    }

    //���a���ͨƥ�
    private void OnPlayerRebornEvent(PlayerRef playerRef)
    {
        SpawnPlayer(playerRef);
    }

    //���s�^�X�ƥ�
    private void OnRemakeRoundEvent()
    {
        DespawnAllPlayers();
    }

    async void StartGame(GameMode mode)
    {
        await gameManager.Runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        gameManager.gameNetworkData.SetReadyCount_RPC(1);

        await Task.Run(() =>
        {
            CustomEventHandler.CallLoadScenesScheduleEvent();
        });
        
        SpawnAllPlayersStart();
    }

    //�ͦ����a(�C���}�l��)
    //���ݭץ��G�ثe�]����ͦ��ҬO�b�D���ݲΤ@�ͦ�,�]���y���Ȥ�ݵL�k�ϥΪ�����\��,�ɭP�C���^�X���m�Ȥ�ݳ��|�b�ͦ��@�媫��,�Ӥ��O�q��������X
    private async void SpawnAllPlayersStart()
    {
        if (gameManager.Runner.GameMode == GameMode.Host)
        {
            int pointNum;

            foreach (var player in gameManager.playerDict.Keys)
            {
                NetworkObject networkPlayerObject;

                while (playerSpawnedPoints.Contains(pointNum = UnityEngine.Random.Range(0, playerSpawnPointList.Count)))
                    continue;

                networkPlayerObject = gameManager.Runner.Spawn(playerPrefab, playerSpawnPointList[pointNum].position, Quaternion.identity, player);

                gameManager.Runner.SetPlayerObject(player, networkPlayerObject);
                playerSpawnedPoints.Add(pointNum);
                gameManager.gameNetworkData.SetPlayerDict_RPC(player, networkPlayerObject);
                //gameManager.gameNetworkData.SetReadyCount_RPC(-1);

                Debug.Log("���a" + player + "�G" + gameManager.gameNetworkData.playerDict[player] + gameManager.gameNetworkData.playerDict.Count);
            }

            playerSpawnedPoints.Clear();
        }

        if (!isStartGame)
        {
            await Task.Run(() =>
            {
                //�I�s��l�Ƹ�ƨƥ�
                CustomEventHandler.CallInitDataEvent();
            });

            if (gameManager.Runner.GameMode == GameMode.Host)
            {
                await Task.Run(() =>
                {
                    //�I�s���J�i�רƥ�
                    CustomEventHandler.CallLoadScheduleEvent();
                });
            }

            //�I�s�}�l�C���ƥ�
            CustomEventHandler.CallStartGameEvent(gameManager.Runner, gameManager.gameNetworkData.playerDict.Count);
            isStartGame = true;
        }
    }

    //�ͦ����a(���a���`��)
    private void SpawnPlayer(PlayerRef playerRef)
    {
        int pointNum = UnityEngine.Random.Range(0, playerSpawnPointList.Count);

        NetworkObject networkPlayerObject;

        networkPlayerObject = gameManager.Runner.Spawn(playerPrefab, playerSpawnPointList[pointNum].position, Quaternion.identity, playerRef);

        gameManager.Runner.SetPlayerObject(playerRef, networkPlayerObject);
        gameManager.gameNetworkData.SetPlayerDict_RPC(playerRef, networkPlayerObject);

        Debug.Log("���a" + playerRef + "�G" + gameManager.gameNetworkData.playerDict[playerRef] + gameManager.gameNetworkData.playerDict.Count);
    }

    //�������a(���a���`��)
    private void DespawnPlayer(PlayerRef playerRef)
    {
        if(gameManager.gameNetworkData.playerDict.ContainsKey(playerRef) && gameManager.gameNetworkData.playerDict.Count > 1)
        {
            gameManager.Runner.Despawn(gameManager.gameNetworkData.playerDict[playerRef]);
            gameManager.gameNetworkData.playerDict.Remove(playerRef);
        }

        gameManager.playerDict[playerRef].SetRemakeRound_RPC(true);
    }

    //�����������a(���s�^�X��)
    private void DespawnAllPlayers()
    {
        foreach (var playerRef in gameManager.playerDict.Keys)
        {
            if (gameManager.gameNetworkData.playerDict.ContainsKey(playerRef))
                gameManager.Runner.Despawn(gameManager.gameNetworkData.playerDict[playerRef]);
        }

        gameManager.gameNetworkData.playerDict.Clear();

        SpawnAllPlayersStart();
    }

    #region - Used Callbacks -
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (gameManager.playerDict.TryGetValue(player, out PlayerNetworkData playerNetworkData))
        {
            runner.Despawn(playerNetworkData.Object);

            gameManager.playerDict.Remove(player);
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
