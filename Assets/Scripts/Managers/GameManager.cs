using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    private bool _isPaused;
    public UnityEventBool onIsPausedChanged;
    public bool isPaused
    {
        get
        {
            return _isPaused;
        }
        set
        {
            _isPaused = value;
            onIsPausedChanged.Invoke(_isPaused);
        }
    } 

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public bool ToggleIsPaused()
    {
        return isPaused = !isPaused;
    }
}

[System.Serializable]
public class UnityEventBool : UnityEvent<bool> { }