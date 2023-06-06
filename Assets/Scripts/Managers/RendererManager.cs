using Example;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class RendererManager : Singleton<RendererManager>
{
    //���ե�,����n���ʵe�޿褺
    public MoveAniDataList_SO moveAniData;

    [Header("���ѯS�İѼƳ]�m")]
    [SerializeField] private float dissolveRate = 0.0125f;
    [SerializeField] private float refreshRate = 0.025f;

    #region - �����S�� -
    //�����S��
    public void InvisibilityEffect(PlayerRef playerRef, PlayerNetworkData playerNetworkData, VisualEffect[] VFXs, Material[] materials,float time, float CDTime, float stiffTime)
    {
        if (GameManager.Instance.Runner.GameMode == GameMode.Host)
            playerNetworkData.SetIsHurt_RPC(false);

        StartCoroutine(DissolveStart(playerRef, playerNetworkData, VFXs, materials, time, CDTime, stiffTime));
    }

    //���ե�,����n�[��ʵe�޿褺
    //���Z�ʱ���
    public void HeatDistortionCtrl(VisualEffect[] VFXs, MoveAniType aniType)
    {
        VFXs[1].SetAnimationCurve("HeatDistortion", moveAniData.GetMoveAniDetails(aniType).HeatDistortionRate);
        //Debug.Log(VFXs[1].GetAnimationCurve("HeatDistortion").keys);
    }

    //���ѱҰ�
    private IEnumerator DissolveStart(PlayerRef playerRef, PlayerNetworkData playerNetworkData, VisualEffect[] VFXs, Material[] materials, float time, float CDTime, float stiffTime)
    {
        if (VFXs.GetValue(0)!=null)
            VFXs[0].Play();

        if (materials.Length > 0)
        {
            float counter = 0;

            while (materials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;

                for(int i=0;i< materials.Length; i++)
                    materials[i].SetFloat("_DissolveAmount", counter);

                yield return new WaitForSeconds(refreshRate);
            }
        }

        if (VFXs.GetValue(1) != null)
            VFXs[1].Play();

        StartCoroutine(DissolveRelieve(playerRef, playerNetworkData, VFXs, materials, time, CDTime, stiffTime));

        HeatDistortionCtrl(VFXs, playerNetworkData.moveAniType);
    }

    //���ѸѰ�
    private IEnumerator DissolveRelieve(PlayerRef playerRef, PlayerNetworkData playerNetworkData, VisualEffect[] VFXs, Material[] materials, float time, float CDTime, float stiffTime)
    {
        CustomEventHandler.CallSkillUIUpdateEvent(SkillType.Invisibility, true, time, CDTime);
        yield return StartCoroutine(SkillTime(playerNetworkData, time));

        //CustomEventHandler.CallPlayerStiffEvent(playerRef, true);

        if (VFXs.GetValue(1) != null)
            VFXs[1].Stop();

        if (materials.Length > 0)
        {
            float counter = 1;

            while (materials[0].GetFloat("_DissolveAmount") > 0)
            {
                counter -= dissolveRate;

                for (int i = 0; i < materials.Length; i++)
                    materials[i].SetFloat("_DissolveAmount", counter);

                yield return new WaitForSeconds(refreshRate);
            }
        }

        CustomEventHandler.CallSkillUIUpdateEvent(SkillType.Invisibility, false, time, CDTime);

        yield return new WaitForSeconds(stiffTime);
        //CustomEventHandler.CallPlayerStiffEvent(playerRef, false);

        yield return new WaitForSeconds(CDTime- stiffTime);

        if (GameManager.Instance.Runner.GameMode == GameMode.Host)
        {
            if (playerNetworkData.isHurt)
                playerNetworkData.SetIsHurt_RPC(false);

            playerNetworkData.SetInvisibility_RPC(false);
        }
    }
    #endregion

    //�ޯ�B�@�ɶ�
    private IEnumerator SkillTime(PlayerNetworkData playerNetworkData, float time)
    {
        for(int i = 0; i < time; i++)
        {
            if (playerNetworkData.isHurt)
                yield return null;
            else
                yield return new WaitForSeconds(1f);
        }
    } 
}
