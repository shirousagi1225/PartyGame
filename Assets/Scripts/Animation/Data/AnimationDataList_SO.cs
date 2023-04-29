using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationDataList_SO", menuName = "AnimationData/AnimationDataList_SO")]
public class AnimationDataList_SO : ScriptableObject
{
    public List<AniDetails> aniDetailsList;

    public AniDetails GetAniDetails(AnimationType aniType)
    {
        return aniDetailsList.Find(i => i.aniType == aniType);
    }
}

[System.Serializable]
public class AniDetails
{
    public AnimationType aniType;
    public AnimationCurve HeatDistortionRate;
}
