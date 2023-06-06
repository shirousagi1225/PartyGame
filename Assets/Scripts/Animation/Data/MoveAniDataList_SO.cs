using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveAniDataList_SO", menuName = "AnimationData/MoveAniDataList_SO")]
public class MoveAniDataList_SO : ScriptableObject
{
    public List<MoveAniDetails> moveAniDetailsList;

    public MoveAniDetails GetMoveAniDetails(MoveAniType aniType)
    {
        return moveAniDetailsList.Find(i => i.aniType == aniType);
    }
}

[System.Serializable]
public class MoveAniDetails
{
    public MoveAniType aniType;
    public float aniThreshold;
    public AnimationCurve HeatDistortionRate;
}
