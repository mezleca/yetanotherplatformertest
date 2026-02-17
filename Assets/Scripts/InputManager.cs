using UnityEngine;
using System.Collections.Generic;

public enum GameKey
{
    W,
    A,
    S,
    D,
    C,
    SHIFT,
    SPACE,
    CTRL
}

public class KeyState : MonoBehaviour
{
    public static KeyState instance { get; private set; }

    private readonly Dictionary<GameKey, KeyCode> key_mapping = new()
    {
        { GameKey.W, KeyCode.W },
        { GameKey.A, KeyCode.A },
        { GameKey.S, KeyCode.S },
        { GameKey.D, KeyCode.D },
        { GameKey.C, KeyCode.C },
        { GameKey.SHIFT, KeyCode.LeftShift },
        { GameKey.SPACE, KeyCode.Space },
        { GameKey.CTRL, KeyCode.LeftControl }
    };

    private readonly Dictionary<GameKey, bool> current_state = new();
    private readonly Dictionary<GameKey, bool> previous_state = new();

    void Start() {}

    void Awake() {
        if (instance != null && instance != this) {    
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (GameKey key in System.Enum.GetValues(typeof(GameKey))) {
            current_state[key] = false;
            previous_state[key] = false;
        }
    }

    void Update() {
        if (instance != this) { 
            return; 
        }

        foreach (GameKey key in key_mapping.Keys) {
            previous_state[key] = current_state[key];
            current_state[key] = Input.GetKey(key_mapping[key]);
        }
    }

    public bool is_pressed(GameKey key) {
        return current_state.ContainsKey(key) && current_state[key];
    }

    public bool is_down(GameKey key) {
        return current_state.ContainsKey(key) && current_state[key] && !previous_state[key];
    }

    public bool is_up(GameKey key) {
        return current_state.ContainsKey(key) && !current_state[key] && previous_state[key];
    }
};