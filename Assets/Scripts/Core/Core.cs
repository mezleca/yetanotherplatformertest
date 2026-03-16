using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCore : MonoBehaviour {
    public static GameCore Instance { get; set; }

    // level stuff
    public LevelLoader level_loader;

    // other stuff
    public GameUtils utils;
    public PlayerInput input;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        level_loader = gameObject.AddComponent<LevelLoader>();
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
        StartCoroutine(LoadMainMenu());
    }

    public IEnumerator LoadMainMenu()
    {
        if (!utils.isSceneLoaded("MainMenu")) {
            yield return SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        }
    }

    public IEnumerator LoadLevelStart()
    {
        if (!utils.isSceneLoaded("LevelStart")) {
            yield return SceneManager.LoadSceneAsync("LevelStart", LoadSceneMode.Additive);
        }
    }

    public IEnumerator LoadPlayer()
    {
        if (!utils.isSceneLoaded("Player")) {
            yield return SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        }

        if (!utils.isSceneLoaded("PlayerUI")) {
            yield return SceneManager.LoadSceneAsync("PlayerUI", LoadSceneMode.Additive);
        }
    }

    public IEnumerator LoadLevel(string level)
    {
        yield return level_loader.Load(level);

        // load player if needed
        yield return LoadPlayer();

        // unload main menu / level selector
        yield return UnloadMainMenu();
        yield return UnloadLevelStart();
    }

    public IEnumerator UnloadPlayer()
    {
        if (utils.isSceneLoaded("Player"))
        {
            Debug.Log("unload player");
            yield return SceneManager.UnloadSceneAsync("Player");
        }

        if (utils.isSceneLoaded("PlayerUI"))
        {
            Debug.Log("unload player ui");
            yield return SceneManager.UnloadSceneAsync("PlayerUI");
        }
    }

    public IEnumerator UnloadMainMenu()
    {
        if (utils.isSceneLoaded("MainMenu"))
        {
            Debug.Log("unload main menu");
            yield return SceneManager.UnloadSceneAsync("MainMenu");
        }
    }

    public IEnumerator UnloadLevelStart()
    {
        if (utils.isSceneLoaded("LevelStart"))
        {
            Debug.Log("unload level start");
            yield return SceneManager.UnloadSceneAsync("LevelStart");
        }
    }

    public IEnumerator UnloadCurrentLevel()
    {
        var current_level = level_loader.current_level;

        if (string.IsNullOrEmpty(current_level) || !utils.isSceneLoaded(current_level))
        {
            yield break;
        }

        yield return SceneManager.UnloadSceneAsync(current_level);
    }
};
