using System;
using UnityEngine;

[Flags]
public enum Action : Int32 {
    NONE   = 0,
    GROUND = 1 << 0,
    JUMP   = 1 << 1,
    WALK   = 1 << 2,
    SPRINT = 1 << 3,
    SLIDE  = 1 << 4,
};

public class EntityState {
    private Action flags = Action.NONE;

    public bool has_action(Action action) {
        return (flags & action) != 0;
    }

    public void add_action(Action action) {
        if (!has_action(action)) {
            flags |= action;
            debug_action("ADDED", action);
        }
    }

    public void remove_action(Action action) {
        if (has_action(action)) {
            flags &= ~action;
            debug_action("REMOVED", action);
        }
    }

    private void debug_action(string op, Action action) {
        Debug.Log($"{op}: {action} | current_flags: {flags}");
    }
};