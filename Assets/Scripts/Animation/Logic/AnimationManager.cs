using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{
    public MoveAniDataList_SO moveAniData;
    public ActionAniDataList_SO actionAniData;

    [Header("�ʵe�ѼƳ]�m")]
    [SerializeField] private float moveAniRefreshRate;

    private void OnEnable()
    {
        CustomEventHandler.SetMoveAniEvent += OnSetMoveAniEvent;
        CustomEventHandler.SetJumpAniEvent += OnSetJumpAniEvent;
        CustomEventHandler.SetActionAniEvent += OnSetActionAniEvent;
    }

    private void OnDisable()
    {
        CustomEventHandler.SetMoveAniEvent -= OnSetMoveAniEvent;
        CustomEventHandler.SetJumpAniEvent -= OnSetJumpAniEvent;
        CustomEventHandler.SetActionAniEvent -= OnSetActionAniEvent;
    }

    //�]�m���ʰʵe�ƥ�
    private void OnSetMoveAniEvent(PlayerNetworkData playerNetworkData,Animator playerAni, MoveAniType moveAniType)
    {
        if (moveAniType != MoveAniType.Jump)
        {
            if (playerAni.GetInteger("Jump") < 0)
                playerAni.SetInteger("Jump", 0);

            StartCoroutine(MoveAniPlay(playerNetworkData, playerAni, moveAniType));
        }

        if (playerNetworkData.isInvisibility)
            RendererManager.Instance.HeatDistortionCtrl(playerNetworkData.VFXs, moveAniType);
    }

    //�]�m���D�ʵe�ƥ�
    private void OnSetJumpAniEvent(Transform playerTran, Animator playerAni)
    {
        StartCoroutine(JumpAniPlay(playerTran, playerAni));
    }

    //�]�m�欰�ʵe�ƥ�
    private void OnSetActionAniEvent(PlayerNetworkData playerNetworkData, Animator playerAni, ActionAniType actionAniType)
    {
        ActionAniPlay(playerNetworkData,playerAni, actionAniType);
    }

    //���ʰʵe����
    private IEnumerator MoveAniPlay(PlayerNetworkData playerNetworkData,Animator playerAni, MoveAniType moveAniType)
    {
        float speed;
        var threshold= moveAniData.GetMoveAniDetails(moveAniType).aniThreshold;

        while ((speed = playerAni.GetFloat("Speed")) < threshold && moveAniType == playerNetworkData.moveAniType)
        {
            if(playerNetworkData.sp==0&& playerNetworkData.moveAniType == MoveAniType.Run)
            {
                playerAni.SetFloat("Speed", moveAniData.GetMoveAniDetails(MoveAniType.Walk).aniThreshold);

                yield break;
            }
            else
                playerAni.SetFloat("Speed",Mathf.SmoothStep(speed, threshold, moveAniRefreshRate * GameManager.Instance.Runner.DeltaTime));

            yield return new WaitForSeconds(0);
        }

        if(moveAniType == playerNetworkData.moveAniType&& speed > threshold)
            playerAni.SetFloat("Speed", threshold);
    }

    //���D�ʵe����
    private IEnumerator JumpAniPlay(Transform playerTran, Animator playerAni)
    {
        playerAni.SetInteger("Jump", 1);
        yield return new WaitForSeconds(0);

        var oldJumpPosY = playerTran.position.y;

        while ((playerTran.position.y- oldJumpPosY) > 0)
        {
            oldJumpPosY = playerTran.position.y;
            yield return new WaitForSeconds(0);
        }

        playerAni.SetInteger("Jump", -1);

        Debug.Log("Jump");
    }

    //�欰�ʵe����
    private void ActionAniPlay(PlayerNetworkData playerNetworkData, Animator playerAni, ActionAniType actionAniType)
    {
        switch (actionAniType)
        {
            case ActionAniType.Pickup:
                playerAni.SetTrigger("Pickup");
                break;
            case ActionAniType.PunchAttack:
                playerAni.SetTrigger("PunchAttack");
                break;
            case ActionAniType.PanAttack:
                playerAni.SetTrigger("PanAttack");
                break;
            case ActionAniType.RopeAttack:
                playerAni.SetTrigger("RopeAttack");
                break;
        }
    }

    //��������欰�ʵe���
    public ActionAniDetails GetActionAniData(ActionAniType aniType)
    {
        return actionAniData.GetActionAniDetails(aniType);
    }
}
