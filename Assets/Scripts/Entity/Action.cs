using System;
using UnityEngine;

// NOTE: no use for now 
// wanted to use on entity system but it ended up being useless

[Flags]
public enum EntityActions : int {
    NONE = 0,
    GROUND = 1 << 0,
    JUMP = 1 << 1,
    WALK = 1 << 2,
    SPRINT = 1 << 3,
    SLIDE = 1 << 4
};

public class EntityAction {
    public EntityActions actions = EntityActions.NONE;

    public event Action<EntityActions, bool> onAction;

    public void add(EntityActions action) {
        bool has_action = (actions & action) != 0;

        if (!has_action) {
            actions |= action;
            onAction?.Invoke(action, true);
            debug_action("ADDED", action);
        }
    }

    public void remove(EntityActions action) {
        bool has_action = (actions & action) != 0;

        if (!has_action) {
            actions &= ~action;
            onAction?.Invoke(action, false);
            debug_action("REMOVED", action);
        }
    }

    private void debug_action(string op, EntityActions action) {
        Debug.Log($"{op}: {action} | current_actions: {actions}");
    }
};