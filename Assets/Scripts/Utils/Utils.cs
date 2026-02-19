using System;
using System.Collections;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    private static GameUtils _instance;

    // auto initialize
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void _init() {
        var go = new GameObject("GameUtils");
        _instance = go.AddComponent<GameUtils>();
        DontDestroyOnLoad(go);
        Debug.Log("[GameUtils] initialized");
    }

    public static void delay_then(float seconds, Action callback) {
        if (_instance == null) {
            Debug.LogError("[GameUtils] instance is null");
            return;
        }

        _instance.StartCoroutine(_instance._delay_coroutine(seconds, callback));
    }

    private IEnumerator _delay_coroutine(float seconds, Action callback) {
        yield return new WaitForSeconds(seconds);
        callback();
    }
};