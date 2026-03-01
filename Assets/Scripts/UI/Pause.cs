using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PauseUI : MonoBehaviour
{
    private Canvas canvas;
    private bool is_paused = false;

    void Awake()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }
    }

    public void toggle_pause()
    {
        if (is_paused)
        {
            resume();
        }
        else
        {
            pause();
        }
    }

    public void resume()
    {
        Time.timeScale = 1.0f;
        canvas.enabled = false;
        is_paused = false;
    }

    public void pause()
    {
        Time.timeScale = 0.0f;
        canvas.enabled = true;
        is_paused = true;
    }
};