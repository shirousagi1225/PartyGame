using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPoolManager : NetworkSingleton<ObjectPoolManager>
{
    public WeaponDataList_SO weaponData;
    public PropsDataList_SO propsData;

    public int weaponSpawnValue;
    public GameObject weaponSpawnPoints;
    [SerializeField]private List<Transform> weaponSpawnPointList;

    private Dictionary<int, WeaponName> weaponDict = new ();
    private Dictionary<int, PropsName> propsDict = new ();
    private HashSet<int> weaponSpawnedPoints=new HashSet<int> ();
    private int _playerCount;
    private float weaponProbability;

    private void Start()
    {
        InitWeaponData();
    }

    private void OnEnable()
    {
        EventHandler.StartGameEvent += OnStartGameEvent;
        EventHandler.RemakeRoundEvent += OnRemakeRoundEvent;
    }

    private void OnDisable()
    {
        EventHandler.StartGameEvent -= OnStartGameEvent;
        EventHandler.RemakeRoundEvent -= OnRemakeRoundEvent;
    }

    //�}�l�C���ƥ�
    private void OnStartGameEvent(NetworkRunner runner, int playerCount)
    {
        _playerCount = playerCount;

        SpawnWeapon(runner,_playerCount + weaponSpawnValue);
    }

    //���s�^�X�ƥ�
    private void OnRemakeRoundEvent()
    {
        
    }

    //��l�ƪZ����T�G�Z������ ���v �ͦ��I
    private void InitWeaponData()
    {
        foreach(int weaponData in Enum.GetValues(typeof(WeaponName)))
        {
            if(weaponData == 0)
                continue;

            weaponDict.Add(weaponData-1,(WeaponName)weaponData);
            //Debug.Log(weaponDict[weaponData - 1]);
        }

        for(int i=0;i< weaponSpawnPoints.transform.childCount;i++)
            weaponSpawnPointList.Add(weaponSpawnPoints.transform.GetChild(i));

        weaponProbability = AlgorithmManager.Instance.InitProbability(weaponDict.Count);
    }

    //�ͦ��Z��
    private void SpawnWeapon(NetworkRunner runner,int spawnCount)
    {
        int pointNum;

        for (int i=0;i< spawnCount;i++)
        {
            while (weaponSpawnedPoints.Contains(pointNum=UnityEngine.Random.Range(0, weaponSpawnPointList.Count - 1)))
                continue;

            runner.Spawn(weaponData.GetWeaponDetails(weaponDict[AlgorithmManager.Instance.ChooseResult(weaponProbability, weaponDict.Count)]).weaponType,
                weaponSpawnPointList[pointNum].position, Quaternion.identity);
            weaponSpawnedPoints.Add(pointNum);
            Debug.Log("yes");
        }
    }
}
