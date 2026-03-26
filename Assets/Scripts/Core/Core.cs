using System.Collections;
using System.Threading.Tasks;
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
        Instance = null;
        input?.Dispose();
        input = null;
    }

    void OnDisable()
    {
        input?.Disable();
    }

    private void Start()
    {
        _ = LoadMainMenu();
    }

    // bullshit

    public async Task LoadAccessPanel()
    {
        if (!utils.isSceneLoaded("AccessPanel")) {
            await SceneManager.LoadSceneAsync("AccessPanel", LoadSceneMode.Additive);
        }
    }

    public async Task LoadMainMenu()
    {
        if (!utils.isSceneLoaded("MainMenu")) {
            await SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        }
    }

    public async Task LoadLevelStart()
    {
        if (!utils.isSceneLoaded("Start")) {
            await SceneManager.LoadSceneAsync("Start", LoadSceneMode.Additive);

            LoadPlayer();
            UnloadMainMenu();
            UnloadLevelSelector();
        }
    }

    public async Task LoadLevelSelector() 
    {
        if (!utils.isSceneLoaded("LevelSelector"))
        {
            await SceneManager.LoadSceneAsync("LevelSelector", LoadSceneMode.Additive);
            UnloadMainMenu();
        }
    }

    public void LoadPlayer()
    {
        if (!utils.isSceneLoaded("Player")) {
            SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        }
    }

    public async void LoadNewLevel(string level)
    {
        // load / unload older level
        await level_loader.Load(level);

        // load player if needed
        LoadPlayer();

        // unload main menu / level selector
        UnloadMainMenu();
        UnloadLevelSelector();
    }

    public void UnloadPlayer()
    {
        if (utils.isSceneLoaded("Player"))
        {
            SceneManager.UnloadSceneAsync("Player");
        }
    }

    public void UnloadAccessPanel()
    {
        if (utils.isSceneLoaded("AccessPanel"))
        {
            SceneManager.UnloadSceneAsync("AccessPanel");
        }
    }

    public void UnloadMainMenu()
    {
        if (utils.isSceneLoaded("MainMenu"))
        {
            SceneManager.UnloadSceneAsync("MainMenu");
        }
    }

    public void UnloadLevelSelector()
    {
        if (utils.isSceneLoaded("LevelSelector"))
        {
            SceneManager.UnloadSceneAsync("LevelSelector");
        }
    }

    public void UnloadCurrentLevel()
    {
        var current_level = level_loader.current_level;

        if (string.IsNullOrEmpty(current_level) || !utils.isSceneLoaded(current_level))
        {
            return;
        }

        SceneManager.UnloadSceneAsync(current_level);
    }
};
