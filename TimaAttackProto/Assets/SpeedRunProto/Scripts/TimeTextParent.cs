using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MoreMountains.Tools;

public class TimeTextParent : MonoBehaviour
{
    public static TimeTextParent instance;
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
        //DontDestroyOnLoad(gameObject);
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
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }

}
