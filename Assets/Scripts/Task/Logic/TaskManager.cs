using Example;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
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
        EventHandler.StartGameEvent += OnStartGameEvent;
        EventHandler.TaskUpdateEvent += OnTaskUpdateEvent;
        EventHandler.RemakeRoundEvent += OnRemakeRoundEvent;
    }

    private void OnDisable()
    {
        EventHandler.StartGameEvent -= OnStartGameEvent;
        EventHandler.TaskUpdateEvent -= OnTaskUpdateEvent;
        EventHandler.RemakeRoundEvent -= OnRemakeRoundEvent;
    }

    //�}�l�C���ƥ�
    private void OnStartGameEvent(NetworkRunner runner, int playerCount)
    {
        if (gameManager.Runner.GameMode == GameMode.Host)
            AssignClothes(runner);
    }

    //�y�����ȧ�s�ƥ�
    private void OnTaskUpdateEvent(PlayerNetworkData playerNetworkData)
    {
        if (gameManager.Runner.GameMode == GameMode.Host)
            RemoveTask(playerNetworkData);
    }

    //���s�^�X�ƥ�
    private void OnRemakeRoundEvent()
    {
        //RemakeTaskDict();
    }

    //��������y�����ȸ��
    public TaskDetails GetTaskData(FeatureName featureName)
    {
        return taskData.GetTaskDetails(featureName);
    }

    //�s�W�y������(�Ω��l�ƥ��ȲM��)
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

    //�����y������(�Ω��s���ȲM��)
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

    //��l�ƪA�˸�T�G�A�˺����ƶq ���v
    private void InitClothesData()
    {
        clothesCount= Enum.GetValues(typeof(ClothesName)).Length-1;
        clothesProbability = AlgorithmManager.Instance.InitProbability(clothesCount);
    }

    //���t�A�ˡG�̪��a�ƶq�H���t��A��
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

    //��l�ƯS�x��T�G�S�x����
    private void InitTaskData(ClothesName clothesName)
    {
        foreach (var feature in clothesData.GetClothesDetails(clothesName).featureList)
            AddTask(feature);
    }

    //���s�y�����ȲM��
    private void RemakeTaskDict()
    {
        for (int i = 0; i < taskNetworkData.initTaskList.Count; i++)
        {
            taskNetworkData.changeTaskList.Add(taskNetworkData.initTaskList[i]);
            taskNetworkData.changeTaskDict.Add(taskNetworkData.initTaskList[i], taskNetworkData.initTaskDict[taskNetworkData.initTaskList[i]]);
        }

        PostTask();
    }

    //�o���y������
    private void PostTask()
    {
        if (gameManager.playerDict.Count>1)
        {
            int pointNum;
            float taskProbability = AlgorithmManager.Instance.InitProbability(taskNetworkData.changeTaskDict.Count);

            foreach (var playerNetworkData in gameManager.playerDict.Values)
            {
                var featureList = clothesData.GetClothesDetails(playerNetworkData.clothes).featureList;

                while (taskNetworkData.changeTaskDict[taskNetworkData.changeTaskList[pointNum = AlgorithmManager.Instance.ChooseResult(taskProbability, taskNetworkData.changeTaskDict.Count)]] == 1 && featureList.Contains(taskNetworkData.changeTaskList[pointNum]))
                    continue;

                playerNetworkData.SetTask_RPC(taskNetworkData.changeTaskList[pointNum]);
            }
        }    
    }
}
