using UnityEngine;
using MoreMountains.Tools;
using System;

public class TimeTextParent : MonoBehaviour
{
    public static TimeTextParent instance;

    public GameObject timerTextCanvas; // TimerTextCanvas 게임 오브젝트를 여기에 드래그 앤 드롭하여 할당하세요.

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MMSceneLoadingManager.OnLoadingStarted += HideUI;
        MMSceneLoadingManager.OnLoadingCompleted += ShowUI;
    }

    private void OnDestroy()
    {
        MMSceneLoadingManager.OnLoadingStarted -= HideUI;
        MMSceneLoadingManager.OnLoadingCompleted -= ShowUI;
    }

    public void ShowUI()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void HideUI()
    {
        // UICamera의 모든 자식 오브젝트를 순회하며 비활성화
        foreach (Transform child in transform)
        {
            if (child.gameObject != timerTextCanvas)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
