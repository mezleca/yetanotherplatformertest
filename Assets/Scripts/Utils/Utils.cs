using System;
using System.Collections;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    private static GameUtils instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void reset_static()
    {
        instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void init()
    {
        ensure_instance();
    }

    private static void ensure_instance()
    {
        if (instance != null)
        {
            return;
        }

        instance = FindFirstObjectByType<GameUtils>();
        if (instance == null)
        {
            instance = new GameObject("GameUtils").AddComponent<GameUtils>();
        }
        DontDestroyOnLoad(instance.gameObject);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public static void delay_then(float seconds, Action callback)
    {
        if (callback == null)
        {
            return;
        }

        if (instance == null)
        {
            ensure_instance();
        }

        if (instance == null)
        {
            Debug.LogError("[GameUtils] instance is null");
            return;
        }

        instance.StartCoroutine(instance.delay_coroutine(seconds, callback));
    }

    private IEnumerator delay_coroutine(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback();
    }
};
