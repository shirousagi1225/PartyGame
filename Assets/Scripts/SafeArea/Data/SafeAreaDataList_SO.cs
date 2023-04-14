using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GUILayout;

[CreateAssetMenu(fileName = "SafeAreaDataList_SO", menuName = "SafeArea/SafeAreaDataList_SO")]
public class SafeAreaDataList_SO : ScriptableObject
{
    public List<SafeAreaDetails> SafeAreaDataList;

    public SafeAreaDetails GetSafeAreaDetails(SafeAreaCode areaCode)
    {
        return SafeAreaDataList.Find(i => i.areaCode == areaCode);
    }
}

[System.Serializable]
public class SafeAreaDetails
{
    public SafeAreaCode areaCode;
    public int roomCount;
}
