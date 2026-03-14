using System;
using System.Collections;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    private void Awake()
    {
       DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        
    }

    public void delay_then(float seconds, Action callback)
    {
        if (callback == null)
        {
            return;
        }

        StartCoroutine(delay_coroutine(seconds, callback));
    }

    private IEnumerator delay_coroutine(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback();
    }
};
