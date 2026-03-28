using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameCore : MonoBehaviour 
{
    public static GameCore Instance { get; private set; }

    // core helpers
    public CameraController camera_controller;
    public UIManager ui_manager;
    public GameUtils utils;
    public PlayerInput input;

    // TOFIX: meh
    public string last_loaded_level = "";

    private readonly Stack<GameState> state_stack = new();

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        input ??= new PlayerInput();
        input.Enable();

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        ui_manager = UIManager.Instance;
        utils = GameUtils.Instance;
        camera_controller = CameraController.Instance;

        // show main menu by default
        _ = TransitionTo(new MainMenuState(this));
    }

    void OnDestroy() {
        Instance = null;
        input?.Dispose();
        input = null;
    }

    void OnDisable() {
        input?.Disable();
    }

    public async Task PushState(GameState next) {
        if (state_stack.TryPeek(out var current)) {
            await current.OnPause();
        }

        state_stack.Push(next);
        await next.OnEnter();
    }

    public async Task PopState() {
        if (state_stack.TryPop(out var current)) {
            await current.OnExit();
        }

        if (state_stack.TryPeek(out var previous)) {
            await previous.OnResume();
        }
    }
    
    public async Task TransitionTo(GameState state) {
        // exit all previous states
        while (state_stack.Count > 0) {
            await state_stack.Pop().OnExit();
        }

        state_stack.Push(state);
        await state.OnEnter();
    }

    public GameState GetState() => state_stack.Peek();
};
