using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionAniDataList_SO", menuName = "AnimationData/ActionAniDataList_SO")]
public class ActionAniDataList_SO : ScriptableObject
{
    public List<ActionAniDetails> actionAniDetailsList;

    public ActionAniDetails GetActionAniDetails(ActionAniType aniType)
    {
        return actionAniDetailsList.Find(i => i.aniType == aniType);
    }
}

[System.Serializable]
public class ActionAniDetails
{
    public ActionAniType aniType;
    public NetworkObject weaponPrefab;
}
