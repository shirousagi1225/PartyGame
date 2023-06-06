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
    private List<NetworkObject> spawnedWeaponList = new();
    private HashSet<int> weaponSpawnedPoints=new HashSet<int> ();
    private int _playerCount;
    private float weaponProbability;

    private void Start()
    {
        InitWeaponData();
    }

    private void OnEnable()
    {
        CustomEventHandler.StartGameEvent += OnStartGameEvent;
        CustomEventHandler.PlayerAttackEvent += OnPlayerAttackEvent;
        CustomEventHandler.PickUpWeaponEvent += OnPickUpWeaponEvent;
        CustomEventHandler.PlayerRebornEvent += OnPlayerRebornEvent;
        CustomEventHandler.RemakeRoundEvent += OnRemakeRoundEvent;
    }

    private void OnDisable()
    {
        CustomEventHandler.StartGameEvent -= OnStartGameEvent;
        CustomEventHandler.PlayerAttackEvent -= OnPlayerAttackEvent;
        CustomEventHandler.PickUpWeaponEvent -= OnPickUpWeaponEvent;
        CustomEventHandler.PlayerRebornEvent -= OnPlayerRebornEvent;
        CustomEventHandler.RemakeRoundEvent -= OnRemakeRoundEvent;
    }

    //�}�l�C���ƥ�
    private void OnStartGameEvent(NetworkRunner runner, int playerCount)
    {
        _playerCount = playerCount;
        SpawnWeaponStart(runner,_playerCount + weaponSpawnValue);
    }

    //���a�����ƥ�
    private void OnPlayerAttackEvent(PlayerNetworkData playerNetworkData)
    {
        if (GameManager.Instance.Runner.GameMode == GameMode.Host)
            AttackMode(playerNetworkData);
    }

    //�B���Z���ƥ�
    private void OnPickUpWeaponEvent(WeaponName weaponName, NetworkObject localPlayer)
    {
        if (GameManager.Instance.Runner.GameMode == GameMode.Host)
            SpawnWeaponPickUp(weaponName, localPlayer);
    }

    //���a���ͨƥ�
    private void OnPlayerRebornEvent(PlayerRef playerRef)
    {
        DespawnAllWeapons();
    }

    //���s�^�X�ƥ�
    private void OnRemakeRoundEvent()
    {
        DespawnAllWeapons();
    }

    //��������Z�����
    public WeaponDetails GetWeaponData(WeaponName weaponName)
    {
        return weaponData.GetWeaponDetails(weaponName);
    }

    //��l�ƪZ����T�G�Z������ ���v �ͦ��I
    private void InitWeaponData()
    {
        foreach(int weaponData in Enum.GetValues(typeof(WeaponName)))
        {
            if(weaponData <=1)
                continue;

            //�]�Z���ιD��X�֩�P�Ӹ�ƶ�,�]���b�s�W��Ʈɻݭק�weaponData����,�N�Z���ιD����}
            if (weaponData>=4)
                continue;

            weaponDict.Add(weaponData-2,(WeaponName)weaponData);
            //Debug.Log(weaponDict[weaponData - 1]);
        }

        for(int i=0;i< weaponSpawnPoints.transform.childCount;i++)
            weaponSpawnPointList.Add(weaponSpawnPoints.transform.GetChild(i));

        weaponProbability = AlgorithmManager.Instance.InitProbability(weaponDict.Count);
    }

    //�ͦ��Z��(�^�X�}�l��)
    //���ݭץ��G�ثe�]����ͦ��ҬO�b�D���ݲΤ@�ͦ�,�]���y���Ȥ�ݵL�k�ϥΪ�����\��,�ɭP�C���^�X���m�Ȥ�ݳ��|�b�ͦ��@�媫��,�Ӥ��O�q��������X
    private void SpawnWeaponStart(NetworkRunner runner,int spawnCount)
    {
        int pointNum;

        for (int i=0;i< spawnCount;i++)
        {
            while (weaponSpawnedPoints.Contains(pointNum=UnityEngine.Random.Range(0, weaponSpawnPointList.Count)))
                continue;

            spawnedWeaponList.Add(runner.Spawn(weaponData.GetWeaponDetails(weaponDict[AlgorithmManager.Instance.ChooseResult(weaponProbability, weaponDict.Count)]).weaponProp,
                weaponSpawnPointList[pointNum].position, Quaternion.identity));
            weaponSpawnedPoints.Add(pointNum);
            //Debug.Log(runner.GameMode.ToString()+"�GWeapon has spawn");
        }

        weaponSpawnedPoints.Clear();
    }

    //�ͦ��Z��(�B���Z����)�G�Ω�B���Z��,�ھڪ��a�ثe�Z����O�_���Z�����Ϥ�
    //1.���Z���G�󴫤���Z��,�å���Z��
    //2.�S�Z���G�s�W�Z���ܪZ����
    //[Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void SpawnWeaponPickUp(WeaponName weaponName, NetworkObject localPlayer)
    {
        if(GameManager.Instance.playerDict.TryGetValue(localPlayer.InputAuthority, out PlayerNetworkData playerNetworkData))
        {
            //var spawnPos = localPlayer.GetComponent<ThirdPersonPlayer>().weaponTrans;

            playerNetworkData.SetActionAniType_RPC(ActionAniType.Pickup);

            if (playerNetworkData.weapon != WeaponName.Fist)
            {
                GameManager.Instance.Runner.Spawn(weaponData.GetWeaponDetails(playerNetworkData.weapon).weaponProp, localPlayer.transform.position+Vector3.up*0.7f, Quaternion.identity);

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

    //���������Z��(���s�^�X��)
    private void DespawnAllWeapons()
    {
        foreach(var weapon in spawnedWeaponList)
        {
            if (weapon.enabled)
                GameManager.Instance.Runner.Despawn(weapon);
        }

        spawnedWeaponList.Clear();

        SpawnWeaponStart(GameManager.Instance.Runner, _playerCount + weaponSpawnValue);
    }

    //�����Ҧ�
    private void AttackMode(PlayerNetworkData playerNetworkData)
    {
        switch (playerNetworkData.weapon)
        {
            case WeaponName.Fist:
                playerNetworkData.SetActionAniType_RPC(ActionAniType.PunchAttack);
                break;
            case WeaponName.Pan:
                playerNetworkData.SetActionAniType_RPC(ActionAniType.PanAttack);
                break;
            case WeaponName.Jumprope:
                playerNetworkData.SetActionAniType_RPC(ActionAniType.RopeAttack);
                break;
        }
    }
}
