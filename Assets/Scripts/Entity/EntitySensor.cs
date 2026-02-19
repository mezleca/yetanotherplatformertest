using System;
using UnityEngine;

public enum GameSide : int {
    LEFT = 0,
    TOP,
    RIGHT,
    BOTTOM
};

[RequireComponent(typeof(BoxCollider2D))]
public class EntitySensor : MonoBehaviour {

    private RaycastHit2D[] hit_buffer = new RaycastHit2D[10];
    private ContactFilter2D contact_filter;

    private readonly Vector2[] CAST_DIRECTIONS = {
        Vector2.left,
        Vector2.up,
        Vector2.right,
        Vector2.down
    };

    private Vector2[] cast_sizes = new Vector2[4];

    public event Action<GameSide, Collider2D> onHit;
    public event Action<GameSide> onClear;

    public float[] distance = new float[4] {
        float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue
    };

    public float entity_height = 0.0f;

    private const float CAST_DISTANCE = 5.0f;
    private const float CAST_THICKNESS = 0.05f;

    void Awake() {
        int layer_mask = LayerMask.GetMask("Floor") | LayerMask.GetMask("Wall");

        contact_filter = new ContactFilter2D();
        contact_filter.SetLayerMask(layer_mask);

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        Vector2 size = col.bounds.size;

        entity_height = col.bounds.extents.y;

        cast_sizes[(int)GameSide.LEFT]   = new Vector2(CAST_THICKNESS, size.y);
        cast_sizes[(int)GameSide.RIGHT]  = new Vector2(CAST_THICKNESS, size.y);
        cast_sizes[(int)GameSide.TOP]    = new Vector2(size.x, CAST_THICKNESS);
        cast_sizes[(int)GameSide.BOTTOM] = new Vector2(size.x, CAST_THICKNESS);
    }

    public void raycast() {
        for (int i = 0; i < CAST_DIRECTIONS.Length; i++) {
            Vector2 dir = CAST_DIRECTIONS[i];
            Vector2 origin = (Vector2)transform.position + dir * entity_height;

            int count = Physics2D.BoxCast(
                origin,
                cast_sizes[i],
                0f,
                dir,
                contact_filter,
                hit_buffer,
                CAST_DISTANCE
            );

            distance[i] = float.MaxValue;

            if (count == 0) {
                onClear?.Invoke((GameSide)i);
                continue;
            }

            for (int j = 0; j < count; j++) {
                float d = hit_buffer[j].distance;
                
                if (d < distance[i]) {
                    distance[i] = d;
                }

                onHit?.Invoke((GameSide)i, hit_buffer[j].collider);
            }
        }
    }

    void FixedUpdate() {
        raycast();
    }
};