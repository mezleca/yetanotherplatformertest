using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseUI : MonoBehaviour
{
    public VisualElement ui;
    private GameCore core;

    // ui buttons
    private Button btn_resume;
    private Button btn_return;

    // containers
    private VisualElement pause_container;

    private bool is_paused = false;

    void Awake()
    {
        core = GameCore.Instance;

        ui = GetComponent<UIDocument>().rootVisualElement;

        // setup elements
        pause_container = ui.Q<VisualElement>("pause-screen");
        btn_resume = ui.Q<Button>("resume");
        btn_return = ui.Q<Button>("return");

        // setup events
        core.input.Player.Pause.performed += ui_pause;
        btn_resume.clicked += ui_resume;
        btn_return.clicked += ui_return;
    }

    void OnDestroy()
    {
        core.input.Player.Pause.performed -= ui_pause;
        btn_resume.clicked -= ui_resume;
        btn_return.clicked -= ui_return;
    }

    private void update_visibility(bool value)
    {
        is_paused = value;

        // ensure ui is visible
        pause_container.BringToFront();

        // start transition
        if (is_paused)
        {
            Time.timeScale = 0.0f;

            pause_container.style.display = DisplayStyle.Flex;
            // TOFIX: transition does not work...
            pause_container.schedule.Execute(() => pause_container.RemoveFromClassList("hidden"));

        }
        else
        {
            Time.timeScale = 1.0f;

            pause_container.AddToClassList("hidden");
            pause_container.style.display = DisplayStyle.None;
        }
    }

    public void ui_resume()
    {
        Debug.Log("ui_resume: true");
        Time.timeScale = 1.0f;
        update_visibility(false);
    }

    public async void ui_return()
    {
        Time.timeScale = 1.0f;

        await core.LoadMainMenu();

        core.UnloadPlayer();
        core.UnloadCurrentLevel();
    }

    public void ui_pause()
    {
        Debug.Log("ui_pause: " + is_paused);
        update_visibility(!is_paused);
    }

    private void ui_pause(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        ui_pause();
    }
};
