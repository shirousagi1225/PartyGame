using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using Example;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private NetworkObject playerPrefab = null;
    public GameObject playerSpawnPoints;
    [SerializeField] private List<Transform> playerSpawnPointList;

    private NetworkRunner networkRunner = null;
    private Dictionary<PlayerRef, NetworkObject> playerDict = new Dictionary<PlayerRef, NetworkObject>();
    private HashSet<int> playerSpawnedPoints = new HashSet<int>();

    private void Start()
    {
        networkRunner=GameManager.Instance.Runner;

        for (int i = 0; i < playerSpawnPoints.transform.childCount; i++)
            playerSpawnPointList.Add(playerSpawnPoints.transform.GetChild(i));

        StartGame(GameMode.AutoHostOrClient);
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

    //生成玩家
    private void SpawnAllPlayers()
    {
        int pointNum;

        foreach (var player in GameManager.Instance.playerDict.Keys)
        {
            while (playerSpawnedPoints.Contains(pointNum = UnityEngine.Random.Range(0, playerSpawnPointList.Count - 1)))
                continue;

            NetworkObject networkPlayerObject = networkRunner.Spawn(playerPrefab, playerSpawnPointList[pointNum].position, Quaternion.identity,player);
            networkRunner.SetPlayerObject(player, networkPlayerObject);
            playerSpawnedPoints.Add(pointNum);
            playerDict.Add(player, networkPlayerObject);
        }

        //開始遊戲事件
        EventHandler.CallStartGameEvent(networkRunner, playerDict.Count);
    }
}
