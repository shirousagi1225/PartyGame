using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : Singleton<GameManager>, IPanel
{
    [SerializeField] private NetworkRunner runner=null;

    public NetworkRunner Runner
    {
        get
        {
            if (runner==null)
            {
                runner=gameObject.AddComponent<NetworkRunner>();
                runner.ProvideInput=true;
            }

            return runner;
        }
    }

    [SerializeField] private CanvasGroup playerCanvas = null;
    public string playerName = null;
    public Dictionary<PlayerRef, PlayerNetworkData> playerDict = new Dictionary<PlayerRef, PlayerNetworkData>();
    [HideInInspector]public GameNetworkData gameNetworkData=null;

    protected override void Awake()
    {
        base.Awake();
        Runner.ProvideInput = true;
        DontDestroyOnLoad(playerCanvas.gameObject);
    }

    private void OnEnable()
    {
        CustomEventHandler.LoadScenesScheduleEvent += OnLoadScenesScheduleEvent;
        CustomEventHandler.InitDataEvent += OnInitDataEvent;
        CustomEventHandler.LoadScheduleEvent += OnLoadScheduleEvent;
        CustomEventHandler.StartGameEvent += OnStartGameEvent;
    }

    private void OnDisable()
    {
        CustomEventHandler.LoadScenesScheduleEvent -= OnLoadScenesScheduleEvent;
        CustomEventHandler.InitDataEvent -= OnInitDataEvent;
        CustomEventHandler.LoadScheduleEvent -= OnLoadScheduleEvent;
        CustomEventHandler.StartGameEvent -= OnStartGameEvent;
    }

    //加載場景進度事件
    private void OnLoadScenesScheduleEvent()
    {
        while (gameNetworkData.readyCount != playerDict.Count)
            continue;
    }

    //初始化資料事件
    private void OnInitDataEvent()
    {
        if(playerDict.TryGetValue(runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
        {
            while (!playerNetworkData.isInitData)
            {
                int readyPlayer = 0;

                foreach(var player in playerDict.Values)
                {
                    if(player.materials.Length!=0)
                        readyPlayer++;
                }

                //Debug.Log(playerNetworkData.playerName+ "："+playerDict.Count+"---"+ readyPlayer);

                if(readyPlayer== playerDict.Count)
                    playerNetworkData.SetIsInitData_RPC(true);
            }
        }
    }

    //載入進度事件
    private void OnLoadScheduleEvent()
    {
        foreach(var player in playerDict.Values)
        {
            while (!player.isInitData)
                continue;
        }
    }

    //開始遊戲事件
    private void OnStartGameEvent(NetworkRunner runner, int playerCount)
    {
        DisplayPanel(true);
        //Debug.Log("open");
    }

    private bool CheckAllPlayerIsReady()
    {
        if (!Runner.IsServer)
            return false;

        foreach(var playerData in playerDict.Values)
            if(!playerData.isReady)
                return false;

        foreach(var playerData in playerDict.Values)
            playerData.isReady = false;

        return true;
    }

    public void UpdatePlayerList()
    {
        CustomEventHandler.CallPlayerListUpdateEvent();

        if (CheckAllPlayerIsReady())
            Runner.SetActiveScene("PlayMap01");
    }

    public void SetPlayerNetworkData()
    {
        if (playerDict.TryGetValue(runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
        {
            playerNetworkData.SetPlayerName_RPC(playerName);
        }
    }

    public void DisplayPanel(bool value)
    {
        playerCanvas.alpha = value ? 1 : 0;
        playerCanvas.interactable = value;
        playerCanvas.blocksRaycasts = value;
    }
}
