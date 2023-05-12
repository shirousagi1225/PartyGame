using Example;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TaskManager : Singleton<TaskManager>
{
    public ClothesDataList_SO clothesData;
    public TaskDataList_SO taskData;
    [SerializeField] private TaskNetworkData taskNetworkDataPrefab = null;

    private GameManager gameManager;
    private TaskNetworkData taskNetworkData = null;
    private int clothesCount;
    private float clothesProbability;

    private void Start()
    {
        gameManager = GameManager.Instance;
        InitClothesData();
    }

    private void OnEnable()
    {
        CustomEventHandler.StartGameEvent += OnStartGameEvent;
        CustomEventHandler.SetClothesEvent += OnSetClothesEvent;
        CustomEventHandler.TaskUpdateEvent += OnTaskUpdateEvent;
        CustomEventHandler.CheckTargetEvent += OnCheckTargetEvent;
        CustomEventHandler.RemakeRoundEvent += OnRemakeRoundEvent;
    }

    private void OnDisable()
    {
        CustomEventHandler.StartGameEvent -= OnStartGameEvent;
        CustomEventHandler.SetClothesEvent -= OnSetClothesEvent;
        CustomEventHandler.TaskUpdateEvent -= OnTaskUpdateEvent;
        CustomEventHandler.CheckTargetEvent -= OnCheckTargetEvent;
        CustomEventHandler.RemakeRoundEvent -= OnRemakeRoundEvent;
    }

    //開始遊戲事件
    private void OnStartGameEvent(NetworkRunner runner, int playerCount)
    {
        if (gameManager.Runner.GameMode == GameMode.Host)
        {
            AssignClothes(runner);
        }
    }

    //設置服裝事件
    private void OnSetClothesEvent(ClothesName clothesName, Material[] materials)
    {
        foreach (Material material in materials)
            material.SetTexture("_Albedo", clothesData.GetClothesDetails(clothesName).clothesTexture);

        //Debug.Log("PlayerNetworkData："+materials.Length);
    }

    //獵殺任務更新事件
    private void OnTaskUpdateEvent(PlayerNetworkData playerNetworkData)
    {
        if (gameManager.Runner.GameMode == GameMode.Host)
            RemoveTask(playerNetworkData);
    }

    //確認目標事件
    private bool OnCheckTargetEvent(FeatureName task, ClothesName clothesName)
    {
        if (clothesData.GetClothesDetails(clothesName).featureList.Contains(task))
        {
            foreach (var playerNetworkData in gameManager.playerDict.Values)
                playerNetworkData.SetRemakeRound_RPC(true);

            return true;
        }
        else
            return false;
    }

    //重製回合事件
    private void OnRemakeRoundEvent()
    {
        RemakeTaskDict();
    }

    //間接獲取獵殺任務資料
    public TaskDetails GetTaskData(FeatureName featureName)
    {
        return taskData.GetTaskDetails(featureName);
    }

    //新增獵殺任務(用於初始化任務清單)
    //注意：特徵種類有進行修改時,需至TaskNetworkData修改任務清單的容量大小
    private void AddTask(FeatureName featureName)
    {
        if (!taskNetworkData.initTaskDict.ContainsKey(featureName))
        {
            taskNetworkData.initTaskList.Add(featureName);
            taskNetworkData.initTaskDict.Add(featureName, 1);
        }
        else
            taskNetworkData.initTaskDict.Set(featureName, taskNetworkData.initTaskDict[featureName]+1);
    }

    //移除獵殺任務(用於更新任務清單)
    private void RemoveTask(PlayerNetworkData playerNetworkData)
    {
        foreach(var feature in clothesData.GetClothesDetails(playerNetworkData.clothes).featureList)
        {
            if (taskNetworkData.changeTaskDict[feature]>1)
                taskNetworkData.changeTaskDict.Set(feature, taskNetworkData.changeTaskDict[feature] - 1);
            else
            {
                taskNetworkData.changeTaskList.Remove(feature);
                taskNetworkData.changeTaskDict.Remove(feature);
            }
        }

        PostTask();
    }

    //初始化服裝資訊：服裝種類數量 機率
    private void InitClothesData()
    {
        clothesCount= Enum.GetValues(typeof(ClothesName)).Length-1;
        clothesProbability = AlgorithmManager.Instance.InitProbability(clothesCount);
    }

    //分配服裝：依玩家數量隨機配對服裝
    private void AssignClothes(NetworkRunner runner)
    {
        int pointNum;
        taskNetworkData = runner.Spawn(taskNetworkDataPrefab, Vector3.zero, Quaternion.identity);

        foreach (var playerNetworkData in gameManager.playerDict.Values)
        {
            while (taskNetworkData.clothesList.Contains((ClothesName)(pointNum = AlgorithmManager.Instance.ChooseResult(clothesProbability, clothesCount)+1)))
                continue;
            //Debug.Log(pointNum);

            taskNetworkData.clothesList.Add((ClothesName)pointNum);
            if (playerNetworkData.clothes == ClothesName.None)
            {
                playerNetworkData.SetClothes_RPC((ClothesName)pointNum);
                Debug.Log((ClothesName)pointNum);
            }

            InitTaskData((ClothesName)pointNum);
        }

        RemakeTaskDict();
    }

    //初始化特徵資訊：特徵種類
    private void InitTaskData(ClothesName clothesName)
    {
        foreach (var feature in clothesData.GetClothesDetails(clothesName).featureList)
            AddTask(feature);
    }

    //重製獵殺任務清單
    private void RemakeTaskDict()
    {
        taskNetworkData.changeTaskList.Clear();
        taskNetworkData.changeTaskDict.Clear();

        for (int i = 0; i < taskNetworkData.initTaskList.Count; i++)
        {
            taskNetworkData.changeTaskList.Add(taskNetworkData.initTaskList[i]);
            taskNetworkData.changeTaskDict.Add(taskNetworkData.initTaskList[i], taskNetworkData.initTaskDict[taskNetworkData.initTaskList[i]]);
        }

        PostTask();
    }

    //發布獵殺任務
    private void PostTask()
    {
        if (gameManager.playerDict.Count>1)
        {
            int pointNum;
            float taskProbability = AlgorithmManager.Instance.InitProbability(taskNetworkData.changeTaskDict.Count);

            foreach (var playerNetworkData in gameManager.playerDict.Values)
            {
                if (!playerNetworkData.isDead)
                {
                    var featureList = clothesData.GetClothesDetails(playerNetworkData.clothes).featureList;

                    while (taskNetworkData.changeTaskDict[taskNetworkData.changeTaskList[pointNum = AlgorithmManager.Instance.ChooseResult(taskProbability, taskNetworkData.changeTaskDict.Count)]] == 1 && featureList.Contains(taskNetworkData.changeTaskList[pointNum]))
                        continue;

                    playerNetworkData.SetTask_RPC(taskNetworkData.changeTaskList[pointNum]);
                }
            }
        }    
    }
}
