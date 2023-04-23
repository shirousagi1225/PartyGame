using Example;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPoolManager : NetworkSingleton<ObjectPoolManager>
{
    public WeaponDataList_SO weaponData;

    public int weaponSpawnValue;
    public GameObject weaponSpawnPoints;
    [SerializeField]private List<Transform> weaponSpawnPointList;

    private Dictionary<int, WeaponName> weaponDict = new ();
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
        EventHandler.PickUpWeaponEvent += OnPickUpWeaponEvent;
        EventHandler.RemakeRoundEvent += OnRemakeRoundEvent;
    }

    private void OnDisable()
    {
        EventHandler.StartGameEvent -= OnStartGameEvent;
        EventHandler.PickUpWeaponEvent -= OnPickUpWeaponEvent;
        EventHandler.RemakeRoundEvent -= OnRemakeRoundEvent;
    }

    //開始遊戲事件
    private void OnStartGameEvent(NetworkRunner runner, int playerCount)
    {
        _playerCount = playerCount;
        SpawnWeaponStart(runner,_playerCount + weaponSpawnValue);
    }

    //拾取武器事件
    private void OnPickUpWeaponEvent(WeaponName weaponName, NetworkObject localPlayer)
    {
        if (GameManager.Instance.Runner.GameMode == GameMode.Host)
            SpawnWeaponPickUp(weaponName, localPlayer);
    }

    //重製回合事件
    private void OnRemakeRoundEvent()
    {
        
    }

    //間接獲取武器資料
    public WeaponDetails GetWeaponData(WeaponName weaponName)
    {
        return weaponData.GetWeaponDetails(weaponName);
    }

    //初始化武器資訊：武器種類 機率 生成點
    private void InitWeaponData()
    {
        foreach(int weaponData in Enum.GetValues(typeof(WeaponName)))
        {
            if(weaponData <=1)
                continue;

            //因武器及道具合併於同個資料集,因此在新增資料時需修改weaponData條件,將武器及道具分開
            if (weaponData>=4)
                continue;

            weaponDict.Add(weaponData-2,(WeaponName)weaponData);
            //Debug.Log(weaponDict[weaponData - 1]);
        }

        for(int i=0;i< weaponSpawnPoints.transform.childCount;i++)
            weaponSpawnPointList.Add(weaponSpawnPoints.transform.GetChild(i));

        weaponProbability = AlgorithmManager.Instance.InitProbability(weaponDict.Count);
    }

    //生成武器(回合開始時)
    private void SpawnWeaponStart(NetworkRunner runner,int spawnCount)
    {
        int pointNum;

        for (int i=0;i< spawnCount;i++)
        {
            while (weaponSpawnedPoints.Contains(pointNum=UnityEngine.Random.Range(0, weaponSpawnPointList.Count)))
                continue;

            runner.Spawn(weaponData.GetWeaponDetails(weaponDict[AlgorithmManager.Instance.ChooseResult(weaponProbability, weaponDict.Count)]).weaponProp,
                weaponSpawnPointList[pointNum].position, Quaternion.identity);
            weaponSpawnedPoints.Add(pointNum);
            //Debug.Log(runner.GameMode.ToString()+"：Weapon has spawn");
        }
    }

    //生成武器(拾取武器時)：用於拾取武器,根據玩家目前武器欄是否有武器做區分
    //1.有武器：更換手持武器,並丟棄原武器
    //2.沒武器：新增武器至武器欄
    //[Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void SpawnWeaponPickUp(WeaponName weaponName, NetworkObject localPlayer)
    {
        if(GameManager.Instance.playerDict.TryGetValue(localPlayer.InputAuthority, out PlayerNetworkData playerNetworkData))
        {
            //var spawnPos = localPlayer.GetComponent<ThirdPersonPlayer>().weaponTrans;

            if (playerNetworkData.weapon != WeaponName.Fist)
            {
                GameManager.Instance.Runner.Spawn(weaponData.GetWeaponDetails(playerNetworkData.weapon).weaponProp, localPlayer.transform.position+Vector3.up*0.4f, Quaternion.identity);

                //runner.Despawn(spawnPos.GetChild(0).GetComponent<NetworkObject>());
                //Destroy(spawnPos.GetChild(0).gameObject);
            }

            playerNetworkData.SetWeapon_RPC(weaponName);

            //var weapon=runner.Spawn(weaponData.GetWeaponDetails(weaponName).weapon, spawnPos.position, spawnPos.rotation);
            //weapon.transform.SetParent(spawnPos);
            //Instantiate(weaponData.GetWeaponDetails(weaponName).weapon, spawnPos.position, spawnPos.rotation, spawnPos);

            //Debug.Log("Weapon has spawn");
        }
    }
}
