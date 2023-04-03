using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropsDataList_SO", menuName = "Props/PropsDataList_SO")]
public class PropsDataList_SO : ScriptableObject
{
    public List<PropsDetails> propsDetailsList;

    public PropsDetails GetPropsDetails(PropsName propsName)
    {
        return propsDetailsList.Find(i => i.propsName == propsName);
    }
}

[System.Serializable]
public class PropsDetails
{
    public PropsName propsName;
    public GameObject propsType;
}
