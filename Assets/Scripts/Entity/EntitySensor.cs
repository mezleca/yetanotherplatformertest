using System;
using UnityEngine;

public enum GameSide : int {
    LEFT = 0,
    TOP,
    RIGHT,
    BOTTOM
};

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class EntitySensor : MonoBehaviour
{
    private BoxCollider2D box_collider;
    private ContactPoint2D[] contact_buffer = new ContactPoint2D[16];
    private ContactFilter2D contact_filter = new();
    private Collider2D[] hit_per_side = new Collider2D[4];

    private readonly Vector2[] SIDE_DIRECTIONS = {
        Vector2.left,
        Vector2.up,
        Vector2.right,
        Vector2.down
    };

    public bool is_grounded = false;

    public bool[] is_touching = new bool[4];
    private bool[] was_touching = new bool[4];

    public event Action<GameSide, Collider2D> onHit;
    public event Action<GameSide> onClear;

    void Awake() {
        box_collider = GetComponent<BoxCollider2D>();
        contact_filter.useTriggers = false;
    }

    private int get_side(Vector2 normal) {
        int best = 0;
        float best_dot = float.MinValue;

        for (int i = 0; i < SIDE_DIRECTIONS.Length; i++) {
            float dot = Vector2.Dot(SIDE_DIRECTIONS[i], normal);

            if (dot > best_dot) {
                best_dot = dot;
                best = i;
            }
        }

        return best;
    }

    public void check_contacts() {
        for (int i = 0; i < 4; i++) {
            is_touching[i] = false;
            hit_per_side[i] = null;
        }

        int count = box_collider.GetContacts(contact_filter, contact_buffer);

        for (int i = 0; i < count; i++) {
            ContactPoint2D contact = contact_buffer[i];
            int side = get_side(-contact.normal);
            is_touching[side] = true;
            hit_per_side[side] = contact.collider;
        }

        is_grounded = is_touching[(int)GameSide.BOTTOM];

        for (int i = 0; i < 4; i++) {
            bool entered = is_touching[i] && !was_touching[i];
            bool exited  = !is_touching[i] && was_touching[i];

            if (entered) {
                onHit?.Invoke((GameSide)i, hit_per_side[i]);
            }

            if (exited) {
                onClear?.Invoke((GameSide)i);
            }

            was_touching[i] = is_touching[i];
        }
    }

    void FixedUpdate() {
        check_contacts();
    }

    void OnDestroy() {
        onHit = null;
        onClear = null;
    }
}
