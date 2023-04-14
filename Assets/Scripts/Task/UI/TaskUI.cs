using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskUI : MonoBehaviour
{
    [SerializeField] private Image taskIcon = null;
    [SerializeField] private Text taskText = null;

    private void OnEnable()
    {
        EventHandler.TaskUIUpdateEvent += OnTaskUIUpdateEvent;
    }

    private void OnDisable()
    {
        EventHandler.TaskUIUpdateEvent -= OnTaskUIUpdateEvent;
    }

    //任務UI更新事件
    private void OnTaskUIUpdateEvent(FeatureName task)
    {
        taskText.text = TaskManager.Instance.GetTaskData(task).featureText;
    }
}
