using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClothesDataList_SO", menuName = "Clothes/ClothesDataList_SO")]
public class ClothesDataList_SO : ScriptableObject
{
    public List<ClothesDetails> clothesDetailsList;

    public ClothesDetails GetClothesDetails(ClothesName clothesName)
    {
        return clothesDetailsList.Find(i => i.clothesName == clothesName);
    }
}

[System.Serializable]
public class ClothesDetails
{
    public ClothesName clothesName;
    public List<FeatureName> featureList;
}
