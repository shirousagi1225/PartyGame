using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Fusion.SimulationInput;

public class StateUI : MonoBehaviour
{
    [Header("���aUI����")]
    [SerializeField] private GameObject HPBar;
    [SerializeField] private Slider HPSlider_B;
    [SerializeField] private Slider HPSlider_F;
    [SerializeField] private Slider SPSlider;
    [SerializeField] private Image invisibilityCDSlip;
    [SerializeField] private Image weaponIcon;

    [Header("�ޯ�ѼƳ]�m")]
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

    //���AUI��s�ƥ�
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

    //��qUI��s�ƥ�
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

    //��OUI��s�ƥ�
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

    //�ޯ�UI��s�ƥ�
    private void OnSkillUIUpdateEvent(SkillType skillType, bool skillState, float skillTime, float skillCDTime)
    {
        if (skillType == SkillType.Invisibility)
            StartCoroutine(SkillOperate(invisibilityCDSlip, skillState, skillTime, skillCDTime));
    }

    //��q���
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

    //��O���
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

    //��O��_
    private IEnumerator SPRecover(PlayerNetworkData playerNetworkData, float maxSp, float SPRecoveryRate, float SPRefreshRate)
    {
        float recoverCount = maxSp * SPRecoveryRate;

        while (playerNetworkData.moveAniType != MoveAniType.Run && SPSlider.value < SPSlider.maxValue)
        {
            playerNetworkData.SetSP_RPC(recoverCount);
            SPSlider.value = playerNetworkData.sp;
            //Debug.Log(SPSlider.value+"�G"+ playerNetworkData.sp);

            yield return new WaitForSeconds(SPRefreshRate);
        }
    }

    //�ޯ�B�@
    private IEnumerator SkillOperate(Image CDSlip,bool skillState, float skillTime, float skillCDTime)
    {
        if (skillState)
        {
            //�ޯ�o��
            float counter = 1 / skillTime* skillRefreshRate;

            while (CDSlip.fillAmount > 0)
            {
                CDSlip.fillAmount -= counter;

                yield return new WaitForSeconds(skillRefreshRate);
            }
        }
        else
        {
            //�ޯ�N�o
            float counter = 1 / skillCDTime* skillRefreshRate;

            while (CDSlip.fillAmount < 1)
            {
                CDSlip.fillAmount += counter;

                yield return new WaitForSeconds(skillRefreshRate);
            }
        }
    }
}
