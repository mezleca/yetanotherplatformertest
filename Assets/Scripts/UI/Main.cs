using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class Main : MonoBehaviour
{
    // root
    public VisualElement ui;

    // stuff
    private GameCore core = GameCore.Instance;

    // ui buttons
    private Button btn_play;
    private Button btn_settings;
    private Button btn_exit;

    void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;

        // setup elements
        btn_play = ui.Q<Button>("play");
        btn_settings = ui.Q<Button>("settings");
        btn_exit = ui.Q<Button>("exit"); 

        // setup events
        btn_play.clicked += onPlay;
        btn_settings.clicked += onSettings;
        btn_exit.clicked += onExit;
    }

    void OnDestroy()
    {
        btn_play.clicked -= onPlay;
        btn_settings.clicked -= onSettings;
        btn_exit.clicked -= onExit;
    }

    IEnumerator onPlayAsync()
    {
        // load first level / player stuff
        yield return core.level_loader.LoadScene("Level 1");
        yield return core.level_loader.LoadScene("Player", false);
        yield return core.level_loader.LoadScene("PlayerUI", false);

        Debug.Log("current scene: " + core.level_loader.current_scene);

        // kms
        yield return SceneManager.UnloadSceneAsync("MainMenu");
    }

    void onPlay()
    {
        StartCoroutine(onPlayAsync());
    }

    void onSettings()
    {
        Debug.Log("TODO");
    }

    void onExit()
    {
        Debug.Log("TODO");
    }
};
