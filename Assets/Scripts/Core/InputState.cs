using System.Collections.Generic;

public class GameInputState 
{
    private readonly Dictionary<string, bool> held = new();
    private readonly HashSet<string> pressed = new();

    public void SetHeld(string action, bool value) => held[action] = value;
    public bool IsHeld(string action) => held.TryGetValue(action, out bool v) && v;

    public void SetPressed(string action, bool is_pressed) {
        if (is_pressed) pressed.Add(action);
        else if (!is_pressed && pressed.Contains(action)) pressed.Remove(action);
    }

    public bool ConsumePress(string action) {
        bool had = pressed.Contains(action);
        pressed.Remove(action);
        return had;
    }

    public void Reset(string action) {
        held[action] = false;
        pressed.Remove(action);
    }
}