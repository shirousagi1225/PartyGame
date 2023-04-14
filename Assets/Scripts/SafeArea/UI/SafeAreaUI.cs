using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafeAreaUI : MonoBehaviour, IPanel
{
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] private Text CaptionText = null;
    public float displayTime;

    private void OnEnable()
    {
        EventHandler.UseSafeAreaUIEvent += OnUseSafeAreaUIEvent;
    }

    private void OnDisable()
    {
        EventHandler.UseSafeAreaUIEvent -= OnUseSafeAreaUIEvent;
    }

    //��ܴ��ܨƥ�
    private void OnUseSafeAreaUIEvent(string caption, int roomCount)
    {
        if(roomCount==0)
            CaptionText.text= caption;

        StartCoroutine(DisplayCaption());
    }

    public void DisplayPanel(bool value)
    {
        canvasGroup.alpha = value ? 1 : 0;
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }

    private IEnumerator DisplayCaption()
    {
        DisplayPanel(true);
        yield return new WaitForSeconds(displayTime);
        DisplayPanel(false);
    }
}
