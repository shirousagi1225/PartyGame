using Example;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GUILayout;

public class SafeAreaManager : Singleton<SafeAreaManager>
{
    public SafeAreaDataList_SO SafeAreaData;
    public NetworkObject safeAreaPrefab;
    [SerializeField] private SafeAreaNetworkData safeAreaNetworkDataPrefab = null;
    public GameObject safeAreaSpawnPoints;
    [SerializeField] private List<Transform> safeAreaSpawnPointList;

    private GameManager gameManager;
    private SafeAreaNetworkData safeAreaNetworkData = null;
    private HashSet<int> safeAreaSpawnedPoints = new HashSet<int>();
    private float safeAreaProbability;

    private void Start()
    {
        gameManager=GameManager.Instance;
    }

    private void OnEnable()
    {
        EventHandler.StartGameEvent += OnStartGameEvent;
        EventHandler.SafeAreaOpenEvent += OnSafeAreaOpenEvent;
        EventHandler.SafeAreaRoomCountUpdateEvent += OnSafeAreaRoomCountUpdateEvent;
        EventHandler.SafeAreaCloseEvent += OnSafeAreaCloseEvent;
        EventHandler.RemakeRoundEvent += OnRemakeRoundEvent;
    }

    private void OnDisable()
    {
        EventHandler.StartGameEvent -= OnStartGameEvent;
        EventHandler.SafeAreaOpenEvent -= OnSafeAreaOpenEvent;
        EventHandler.SafeAreaRoomCountUpdateEvent -= OnSafeAreaRoomCountUpdateEvent;
        EventHandler.SafeAreaCloseEvent -= OnSafeAreaCloseEvent;
        EventHandler.RemakeRoundEvent -= OnRemakeRoundEvent;
    }

    //開始遊戲事件
    private void OnStartGameEvent(NetworkRunner runner, int playerCount)
    {
        if (gameManager.Runner.GameMode == GameMode.Host)
            InitSafeAreaData();
    }

    //啟動安全區事件
    private void OnSafeAreaOpenEvent()
    {
        foreach (var safeArea in safeAreaNetworkData.safeAreaDict)
            safeAreaNetworkData.openSafeAreaList.Add(gameManager.Runner.Spawn(safeAreaPrefab, safeArea.Value,Quaternion.identity));

        //Debug.Log(safeAreaNetworkData);
    }

    //更新安全區人數事件
    private void OnSafeAreaRoomCountUpdateEvent()
    {
        
    }

    //關閉安全區事件
    private void OnSafeAreaCloseEvent()
    {
        foreach (var safeArea in safeAreaNetworkData.openSafeAreaList)
            gameManager.Runner.Despawn(safeArea);

        safeAreaNetworkData.openSafeAreaList.Clear();

        int pointNum = UnityEngine.Random.Range(0, safeAreaNetworkData.validSafeAreaList.Count);
        safeAreaNetworkData.safeAreaDict.Remove((SafeAreaCode)safeAreaNetworkData.validSafeAreaList[pointNum]);
        safeAreaNetworkData.validSafeAreaList.Remove(safeAreaNetworkData.validSafeAreaList[pointNum]);

        AttackCycle();

        //Debug.Log(safeAreaNetworkData);
    }

    //重製回合事件
    private void OnRemakeRoundEvent()
    {
        
    }

    //初始化安全區資訊：安全區種類 機率 生成點
    private void InitSafeAreaData()
    {
        int pointNum;
        safeAreaNetworkData = gameManager.Runner.Spawn(safeAreaNetworkDataPrefab, Vector3.zero, Quaternion.identity);
        //Debug.Log(safeAreaNetworkData);

        for (int i = 0; i < safeAreaSpawnPoints.transform.childCount; i++)
            safeAreaSpawnPointList.Add(safeAreaSpawnPoints.transform.GetChild(i));

        foreach (int areaCode in Enum.GetValues(typeof(SafeAreaCode)))
        {
            if (areaCode == 0)
                continue;

            while (safeAreaSpawnedPoints.Contains(pointNum = UnityEngine.Random.Range(0, safeAreaSpawnPointList.Count)))
                continue;

            safeAreaSpawnedPoints.Add(pointNum);
            safeAreaNetworkData.safeAreaDict.Add((SafeAreaCode)areaCode, safeAreaSpawnPointList[pointNum].position);
            safeAreaNetworkData.validSafeAreaList.Add(areaCode);
        }

        safeAreaProbability=AlgorithmManager.Instance.InitProbability(safeAreaNetworkData.safeAreaDict.Count);
        AttackCycle();
    }

    //地圖攻擊循環
    private void AttackCycle()
    {
        safeAreaNetworkData.SetAttackTimer_RPC(safeAreaNetworkData.attackCycleTime);
        safeAreaNetworkData.SetCaption_RPC(safeAreaNetworkData.attackWarning, 0);
        Debug.Log(safeAreaNetworkData.attackTimer.RemainingTime(gameManager.Runner));
    }
}
