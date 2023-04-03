using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : Singleton<TaskManager>
{
    public TaskDataList_SO taskData;

    private Dictionary<int, FeatureName> taskDict = new ();

    public void AddTask(FeatureName featureName)
    {
        if (!taskDict.ContainsValue(featureName))
        {
            taskDict.Add(taskDict.Count, featureName);
        }
    }
}
