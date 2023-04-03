using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmManager : Singleton<AlgorithmManager>
{
    private float total = 0;

    //設定機率及初始化
    public float InitProbability(int typeCount)
    {
        float probabilityValue = 1f / typeCount;
        //Debug.Log(probabilityValue);

        //首先計算出概率的總值，用來計算隨機範圍
        for (int i = 0; i < typeCount; i++)
        {
            total += probabilityValue;
        }

        return probabilityValue;
    }

    //計算結果方法
    public int ChooseResult(float probabilityValue, int typeCount)
    {
        float nob = UnityEngine.Random.Range(0, total);

        for (int i = 0; i < typeCount; i++)
        {
            if (nob < probabilityValue)
            {
                return i;
            }
            else
            {
                nob -= probabilityValue;
            }
        }
        return typeCount-1;
    }
}
