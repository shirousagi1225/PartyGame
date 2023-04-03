using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmManager : Singleton<AlgorithmManager>
{
    private float total = 0;

    //�]�w���v�Ϊ�l��
    public float InitProbability(int typeCount)
    {
        float probabilityValue = 1f / typeCount;
        //Debug.Log(probabilityValue);

        //�����p��X���v���`�ȡA�Ψӭp���H���d��
        for (int i = 0; i < typeCount; i++)
        {
            total += probabilityValue;
        }

        return probabilityValue;
    }

    //�p�⵲�G��k
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
