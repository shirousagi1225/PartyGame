using Fusion;
using System;
using UnityEngine;
using UnityEngine.UI;

public static class EventHandler
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
}
