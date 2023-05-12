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
        CustomEventHandler.StartGameEvent += OnStartGameEvent;
        CustomEventHandler.SafeAreaOpenEvent += OnSafeAreaOpenEvent;
        CustomEventHandler.SafeAreaRoomCountUpdateEvent += OnSafeAreaRoomCountUpdateEvent;
        CustomEventHandler.SafeAreaCloseEvent += OnSafeAreaCloseEvent;
        CustomEventHandler.RemakeRoundEvent += OnRemakeRoundEvent;
    }

    private void OnDisable()
    {
        CustomEventHandler.StartGameEvent -= OnStartGameEvent;
        CustomEventHandler.SafeAreaOpenEvent -= OnSafeAreaOpenEvent;
        CustomEventHandler.SafeAreaRoomCountUpdateEvent -= OnSafeAreaRoomCountUpdateEvent;
        CustomEventHandler.SafeAreaCloseEvent -= OnSafeAreaCloseEvent;
        CustomEventHandler.RemakeRoundEvent -= OnRemakeRoundEvent;
    }

    //�}�l�C���ƥ�
    private void OnStartGameEvent(NetworkRunner runner, int playerCount)
    {
        if (gameManager.Runner.GameMode == GameMode.Host)
            InitSafeAreaData();
    }

    //�Ұʦw���Ϩƥ�
    private void OnSafeAreaOpenEvent()
    {
        foreach (var safeArea in safeAreaNetworkData.safeAreaDict)
            safeAreaNetworkData.openSafeAreaList.Add(gameManager.Runner.Spawn(safeAreaPrefab, safeArea.Value,Quaternion.identity));

        //Debug.Log(safeAreaNetworkData);
    }

    //��s�w���ϤH�ƨƥ�
    private void OnSafeAreaRoomCountUpdateEvent()
    {
        
    }

    //�����w���Ϩƥ�
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

    //���s�^�X�ƥ�
    private void OnRemakeRoundEvent()
    {
        
    }

    //��l�Ʀw���ϸ�T�G�w���Ϻ��� ���v �ͦ��I
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

    //�a�ϧ����`��
    private void AttackCycle()
    {
        safeAreaNetworkData.SetAttackTimer_RPC(safeAreaNetworkData.attackCycleTime);
        safeAreaNetworkData.SetCaption_RPC(safeAreaNetworkData.attackWarning, 0);
        Debug.Log(safeAreaNetworkData.attackTimer.RemainingTime(gameManager.Runner));
    }
}
