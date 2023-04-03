using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskDataList_SO", menuName = "Task/TaskDataList_SO")]
public class TaskDataList_SO : ScriptableObject
{
    public List<TaskDetails> taskDetailsList;

    public TaskDetails GetTaskDetails(FeatureName featureName)
    {
        return taskDetailsList.Find(i => i.featureName == featureName);
    }
}

[System.Serializable]
public class TaskDetails
{
    public FeatureName featureName;
}
