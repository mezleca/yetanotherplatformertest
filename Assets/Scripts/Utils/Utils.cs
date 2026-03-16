using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUtils : MonoBehaviour
{
    public static GameUtils Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void delay_then(float seconds, Action callback)
    {
        if (callback == null)
        {
            return;
        }

        StartCoroutine(delay_coroutine(seconds, callback));
    }

    public bool isSceneLoaded(string name)
    {
        Scene scene = SceneManager.GetSceneByName(name);
        return scene.IsValid() && scene.isLoaded;
    }

    private IEnumerator delay_coroutine(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback();
    }
};
