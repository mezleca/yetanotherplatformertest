using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class Main : MonoBehaviour
{
    // root
    public VisualElement ui;

    // stuff
    private readonly GameCore core = GameCore.Instance;

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
        // load level selector
        yield return core.LoadLevelStart();
        
        // unload main menu
        yield return core.UnloadCurrentLevel();
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
