using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUtils : MonoBehaviour
{
    public static GameUtils Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy() {
        if (Instance == this) Instance = null;
    }

    public void delay_then(float seconds, Action callback) {
        if (callback == null) {
            return;
        }

        StartCoroutine(delay_coroutine(seconds, callback));
    }

    public GameEntity get_closest_player() {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        Player result = null;

        float best = 999.0f;

        for (int i = 0; i < players.Length; i++) {
            var pos = players[i].transform.position;
            float distance = Vector3.Distance(transform.position, pos);

            if (distance < best) {
                result = players[i];
                best = distance;
            }
        }

        if (result != null) {
            return result.ent;
        }

        return null;
    }

    public void drawArrow(Vector2 start, Vector2 end, Color color) {
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);

        Vector2 dir = (end - start).normalized;
        Vector2 right = new(dir.y, -dir.x);

        float headSize = 0.2f;
        Vector2 tip       = end;
        Vector2 baseLeft  = tip - dir * headSize + right * (headSize * 0.5f);
        Vector2 baseRight = tip - dir * headSize - right * (headSize * 0.5f);

        Gizmos.DrawLine(tip, baseLeft);
        Gizmos.DrawLine(tip, baseRight);
        Gizmos.DrawLine(baseLeft, baseRight);
    }

    public void drawBounds(Bounds bounds) {
        var min = bounds.min;
        var max = bounds.max;

        Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z), Color.green);
        Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z), Color.green);
        Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(min.x, min.y, max.z), Color.green);
        Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, min.y, min.z), Color.green);

        Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z), Color.green);
        Debug.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z), Color.green);
        Debug.DrawLine(new Vector3(max.x, max.y, max.z), new Vector3(min.x, max.y, max.z), Color.green);
        Debug.DrawLine(new Vector3(min.x, max.y, max.z), new Vector3(min.x, max.y, min.z), Color.green);

        Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, max.y, min.z), Color.green);
        Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z), Color.green);
        Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, max.y, max.z), Color.green);
        Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, max.y, max.z), Color.green);
    }

    private IEnumerator delay_coroutine(float seconds, Action callback) {
        yield return new WaitForSeconds(seconds);
        if (callback == null) yield break;
        callback();
    }

    public bool isSceneLoaded(string name) {
        Scene scene = SceneManager.GetSceneByName(name);
        return scene.IsValid() && scene.isLoaded;
    }
};
