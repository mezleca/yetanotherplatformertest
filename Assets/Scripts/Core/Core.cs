using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCore : MonoBehaviour {
    public static GameCore Instance { get; set; }

    // main scenes
    private readonly string player_ui_scene = "PlayerUI";
    private readonly string start_scene = "MainMenu";
    
    // level loader    
    public SceneLoader level_loader;

    // other stuff
    public GameUtils utils;
    public PlayerInput input;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            return;
        }

        level_loader = gameObject.AddComponent<SceneLoader>();
        utils = gameObject.AddComponent<GameUtils>();
        input ??= new PlayerInput();
        input.Enable();

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        // TODO: rest

        input?.Dispose();
        input = null;
    }

    void OnDisable()
    {
        input?.Disable();
    }

    private void Start()
    {
        StartCoroutine(LoadMainScenes());
    }

    public IEnumerator LoadMainScenes()
    {
        Debug.Log("loading main scenes...");

        // load main scenes
        yield return SceneManager.LoadSceneAsync(start_scene, LoadSceneMode.Additive);
        // yield return SceneManager.LoadSceneAsync(player_ui_scene);
    }
};