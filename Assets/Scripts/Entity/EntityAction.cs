using System;

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
    public EntityActions flags = EntityActions.NONE;
    public event Action<EntityActions, bool> onAction;

    public void set(EntityActions action, bool _add) {
        if (_add) {
            add(action);
        } else {
            remove(action);
        }
    }

    public void add(EntityActions action) {
        bool has_action = (flags & action) != 0;

        if (!has_action) {
            flags |= action;
            onAction?.Invoke(action, true);
        }
    }

    public void remove(EntityActions action) {
        bool has_action = (flags & action) != 0;

        if (has_action) {
            flags &= ~action;
            onAction?.Invoke(action, false);
        }
    }
};