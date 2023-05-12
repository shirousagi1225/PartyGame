using Fusion;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public static class CustomEventHandler
{
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    public static event Action<string,bool,bool> ShowSecUIEvent;
    public static void CallShowSecUIEvent(string canvas,bool canOpenSecUI,bool canSwitch)
    {
        ShowSecUIEvent?.Invoke(canvas,canOpenSecUI, canSwitch);
    }

    public static event Action InitDataEvent;
    public static void CallInitDataEvent()
    {
        InitDataEvent?.Invoke();
    }

    public static event Action LoadScheduleEvent;
    public static void CallLoadScheduleEvent()
    {
        LoadScheduleEvent?.Invoke();
    }

    public static event Action LoadScenesScheduleEvent;
    public static void CallLoadScenesScheduleEvent()
    {
        LoadScenesScheduleEvent?.Invoke();
    }

    public static event Action<NetworkRunner,int> StartGameEvent;
    public static void CallStartGameEvent(NetworkRunner runner,int playerCount)
    {
        StartGameEvent?.Invoke(runner,playerCount);
    }

    public static event Action RemakeRoundEvent;
    public static void CallRemakeRoundEvent()
    {
        RemakeRoundEvent?.Invoke();
    }

    public static event Action PlayerListUpdateEvent;
    public static void CallPlayerListUpdateEvent()
    {
        PlayerListUpdateEvent?.Invoke();
    }

    public static event Action<ClothesName, Material[]> SetClothesEvent;
    public static void CallSetClothesEvent(ClothesName clothesName, Material[] materials)
    {
        SetClothesEvent?.Invoke(clothesName, materials);
    }

    public static event Action<PlayerNetworkData> TaskUpdateEvent;
    public static void CallTaskUpdateEvent(PlayerNetworkData playerNetworkData)
    {
        TaskUpdateEvent?.Invoke(playerNetworkData);
    }

    public static event Func<FeatureName, ClothesName,bool> CheckTargetEvent;
    public static bool CallCheckTargetEvent(FeatureName task, ClothesName clothesName)
    {
        if(CheckTargetEvent!=null)
            return CheckTargetEvent(task, clothesName);
        else
        {
            Debug.LogError("CallCheckTargetEventµù¥U¥¢±Ñ!!!");
            return true;
        } 
    }

    public static event Action<FeatureName> TaskUIUpdateEvent;
    public static void CallTaskUIUpdateEvent(FeatureName task)
    {
        TaskUIUpdateEvent?.Invoke(task);
    }

    public static event Action<WeaponName, NetworkObject> PickUpWeaponEvent;
    public static void CallPickUpWeaponEvent(WeaponName weaponName, NetworkObject localPlayer)
    {
        PickUpWeaponEvent?.Invoke(weaponName, localPlayer);
    }

    public static event Action SafeAreaOpenEvent;
    public static void CallSafeAreaOpenEvent()
    {
        SafeAreaOpenEvent?.Invoke();
    }

    public static event Action SafeAreaRoomCountUpdateEvent;
    public static void CallSafeAreaRoomCountUpdateEvent()
    {
        SafeAreaRoomCountUpdateEvent?.Invoke();
    }

    public static event Action RangeAttackEvent;
    public static void CallRangeAttackEvent()
    {
        RangeAttackEvent?.Invoke();
    }

    public static event Action SafeAreaCloseEvent;
    public static void CallSafeAreaCloseEvent()
    {
        SafeAreaCloseEvent?.Invoke();
    }

    public static event Action<string,int> UseSafeAreaUIEvent;
    public static void CallUseSafeAreaUIEvent(string caption, int roomCount)
    {
        UseSafeAreaUIEvent?.Invoke(caption,roomCount);
    }

    public static event Action<int, WeaponName> StateUIUpdateEvent;
    public static void CallStateUIUpdateEvent(int hp, WeaponName weapon)
    {
        StateUIUpdateEvent?.Invoke(hp, weapon);
    }

    public static event Action<PlayerRef> PlayerDeadEvent;
    public static void CallPlayerDeadEvent(PlayerRef playerRef)
    {
        PlayerDeadEvent?.Invoke(playerRef);
    }

    public static event Action<PlayerRef,bool> PlayerStiffEvent;
    public static void CallPlayerStiffEvent(PlayerRef playerRef, bool isStiff)
    {
        PlayerStiffEvent?.Invoke(playerRef, isStiff);
    }
}
