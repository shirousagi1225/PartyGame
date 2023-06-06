using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Fusion.SimulationInput;

public class StateUI : MonoBehaviour
{
    [Header("玩家UI元件")]
    [SerializeField] private GameObject HPBar;
    [SerializeField] private Slider HPSlider_B;
    [SerializeField] private Slider HPSlider_F;
    [SerializeField] private Slider SPSlider;
    [SerializeField] private Image invisibilityCDSlip;
    [SerializeField] private Image weaponIcon;

    [Header("技能參數設置")]
    [SerializeField] private float skillRefreshRate;

    private void OnEnable()
    {
        CustomEventHandler.StateUIUpdateEvent += OnStateUIUpdateEvent;
        CustomEventHandler.HPUIUpdateEvent += OnHPUIUpdateEvent;
        CustomEventHandler.SPUIUpdateEvent += OnSPUIUpdateEvent;
        CustomEventHandler.SkillUIUpdateEvent += OnSkillUIUpdateEvent;
    }

    private void OnDisable()
    {
        CustomEventHandler.StateUIUpdateEvent -= OnStateUIUpdateEvent;
        CustomEventHandler.HPUIUpdateEvent -= OnHPUIUpdateEvent;
        CustomEventHandler.SPUIUpdateEvent -= OnSPUIUpdateEvent;
        CustomEventHandler.SkillUIUpdateEvent -= OnSkillUIUpdateEvent;
    }

    //狀態UI更新事件
    private void OnStateUIUpdateEvent(int hp, WeaponName weapon)
    {
        /*if(hp>=0)
            HPBar.transform.GetChild(hp).gameObject.SetActive(false);*/

        if (weapon!= WeaponName.None)
        {
            weaponIcon.sprite = ObjectPoolManager.Instance.GetWeaponData(weapon).weaponSprite;
            weaponIcon.SetNativeSize();
        }
    }

    //血量UI更新事件
    private void OnHPUIUpdateEvent(float maxHp, float HPRefreshRate)
    {
        if (GameManager.Instance.playerDict.TryGetValue(GameManager.Instance.Runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
        {
            if (playerNetworkData.hp == maxHp)
            {
                if (HPSlider_F.maxValue != maxHp)
                {
                    HPSlider_F.maxValue = maxHp;
                    HPSlider_B.maxValue = maxHp;
                }

                HPSlider_F.value = maxHp;
                HPSlider_B.value = maxHp;
            }
            else
                StartCoroutine(HPReduce(playerNetworkData, HPRefreshRate));
        }
    }

    //體力UI更新事件
    private void OnSPUIUpdateEvent(MoveAniType oldAniType, MoveAniType newAniType, float maxSp, float SPReductionRate, float SPRecoveryRate, float SPRefreshRate)
    {
        if (GameManager.Instance.playerDict.TryGetValue(GameManager.Instance.Runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
        {
            if (newAniType == MoveAniType.Run)
                StartCoroutine(SPReduce(playerNetworkData, maxSp, SPReductionRate, SPRefreshRate));
            else if(newAniType == MoveAniType.None)
                SPSlider.value = maxSp;
            else if(oldAniType== MoveAniType.Run)
                StartCoroutine(SPRecover(playerNetworkData, maxSp, SPRecoveryRate, SPRefreshRate));
        }
    }

    //技能UI更新事件
    private void OnSkillUIUpdateEvent(SkillType skillType, bool skillState, float skillTime, float skillCDTime)
    {
        if (skillType == SkillType.Invisibility)
            StartCoroutine(SkillOperate(invisibilityCDSlip, skillState, skillTime, skillCDTime));
    }

    //血量減少
    private IEnumerator HPReduce(PlayerNetworkData playerNetworkData, float HPRefreshRate)
    {
        float HPBuffer;

        HPSlider_F.value = playerNetworkData.hp;
        HPBuffer= playerNetworkData.hp;

        while (HPBuffer == playerNetworkData.hp && HPSlider_B.value!= playerNetworkData.hp)
        {
            HPSlider_B.value = Mathf.Lerp(HPSlider_B.value, playerNetworkData.hp, HPRefreshRate * GameManager.Instance.Runner.DeltaTime);

            yield return new WaitForSeconds(0);
        }
    }

    //體力減少
    private IEnumerator SPReduce(PlayerNetworkData playerNetworkData, float maxSp, float SPReductionRate, float SPRefreshRate)
    {
        if(SPSlider.maxValue!= maxSp)
        {
            SPSlider.maxValue = maxSp;
            SPSlider.value= maxSp;
        }

        float reduceCount = maxSp * SPReductionRate;

        while (playerNetworkData.moveAniType == MoveAniType.Run && playerNetworkData.sp > SPSlider.minValue)
        {
            playerNetworkData.SetSP_RPC(-reduceCount);
            SPSlider.value = playerNetworkData.sp;

            yield return new WaitForSeconds(SPRefreshRate);
        }
    }

    //體力恢復
    private IEnumerator SPRecover(PlayerNetworkData playerNetworkData, float maxSp, float SPRecoveryRate, float SPRefreshRate)
    {
        float recoverCount = maxSp * SPRecoveryRate;

        while (playerNetworkData.moveAniType != MoveAniType.Run && SPSlider.value < SPSlider.maxValue)
        {
            playerNetworkData.SetSP_RPC(recoverCount);
            SPSlider.value = playerNetworkData.sp;
            //Debug.Log(SPSlider.value+"："+ playerNetworkData.sp);

            yield return new WaitForSeconds(SPRefreshRate);
        }
    }

    //技能運作
    private IEnumerator SkillOperate(Image CDSlip,bool skillState, float skillTime, float skillCDTime)
    {
        if (skillState)
        {
            //技能發動
            float counter = 1 / skillTime* skillRefreshRate;

            while (CDSlip.fillAmount > 0)
            {
                CDSlip.fillAmount -= counter;

                yield return new WaitForSeconds(skillRefreshRate);
            }
        }
        else
        {
            //技能冷卻
            float counter = 1 / skillCDTime* skillRefreshRate;

            while (CDSlip.fillAmount < 1)
            {
                CDSlip.fillAmount += counter;

                yield return new WaitForSeconds(skillRefreshRate);
            }
        }
    }
}
