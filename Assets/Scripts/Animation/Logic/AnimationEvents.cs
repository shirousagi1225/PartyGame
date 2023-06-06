using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] private NetworkObject player;
    [SerializeField] private NetworkObject punchPrefab;

    private GameManager gameManager=GameManager.Instance;
    private AnimationManager animationManager=AnimationManager.Instance;
    private NetworkObject weaponPrefab;

    //拾取事件
    private void PickupEvent(int isStiff)
    {
        if (isStiff == 1)
            CustomEventHandler.CallPlayerStiffEvent(player.InputAuthority, true);
        else if (isStiff == 0)
            CustomEventHandler.CallPlayerStiffEvent(player.InputAuthority, false);
    }

    //拳頭攻擊事件
    private void PunchAttackEvent(int isStiff)
    {
        if (isStiff == 1)
        {
            if (gameManager.Runner.GameMode == GameMode.Host)
            {
                punchPrefab.gameObject.SetActive(true);
                punchPrefab.GetComponent<Weapon>().playerRef = player.InputAuthority;
            }

            CustomEventHandler.CallPlayerStiffEvent(player.InputAuthority, true);
        }
        else if (isStiff == 0)
        {
            if (gameManager.Runner.GameMode == GameMode.Host)
            {
                punchPrefab.GetComponent<Weapon>().playerRef = PlayerRef.None;
                punchPrefab.gameObject.SetActive(false);
            }

            CustomEventHandler.CallPlayerStiffEvent(player.InputAuthority, false);
        }
    }

    //平底鍋攻擊事件
    private void PanAttackEvent(int isStiff)
    {
        if (isStiff == 1)
        {
            if (gameManager.Runner.GameMode == GameMode.Host)
            {
                weaponPrefab = gameManager.Runner.Spawn(animationManager.GetActionAniData(ActionAniType.PanAttack).weaponPrefab, transform.position, transform.rotation);
                weaponPrefab.transform.SetParent(transform);
                weaponPrefab.GetComponent<Weapon>().playerRef = player.InputAuthority;
            }
                
            CustomEventHandler.CallPlayerStiffEvent(player.InputAuthority, true);
        }
        else if (isStiff == 0)
        {
            if (gameManager.Runner.GameMode == GameMode.Host)
            {
                weaponPrefab.GetComponent<Weapon>().playerRef = PlayerRef.None;
                gameManager.Runner.Despawn(weaponPrefab);
                weaponPrefab = null;
            }

            CustomEventHandler.CallPlayerStiffEvent(player.InputAuthority, false);
        }
    }

    //跳繩攻擊事件
    private void RopeAttackEvent(int isStiff)
    {
        if (isStiff == 1)
        {
            if (gameManager.Runner.GameMode == GameMode.Host)
            {
                weaponPrefab = gameManager.Runner.Spawn(animationManager.GetActionAniData(ActionAniType.RopeAttack).weaponPrefab, transform.position, transform.rotation);
                weaponPrefab.transform.SetParent(transform);
                weaponPrefab.GetComponent<Weapon>().playerRef = player.InputAuthority;
            } 

            CustomEventHandler.CallPlayerStiffEvent(player.InputAuthority, true);
        }
        else if (isStiff == 0)
        {
            if (gameManager.Runner.GameMode == GameMode.Host)
            {
                weaponPrefab.GetComponent<Weapon>().playerRef = PlayerRef.None;
                gameManager.Runner.Despawn(weaponPrefab);
                weaponPrefab = null;
            }
                
            CustomEventHandler.CallPlayerStiffEvent(player.InputAuthority, false);
        }
    }

    //攻擊偵測事件
    private void AttackDetectEvent(int isAttack)
    {
        if (gameManager.Runner.GameMode == GameMode.Host)
        {
            if(isAttack==1)
                gameManager.playerDict[player.InputAuthority].SetIsAttack_RPC(true);
            else if(isAttack == 0)
                gameManager.playerDict[player.InputAuthority].SetIsAttack_RPC(false);
        }
    }
}
