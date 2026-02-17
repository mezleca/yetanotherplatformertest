using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Entity : MonoBehaviour
{
    [SerializeField] public float jump_y = 10.0f;
    [SerializeField] public float walk_speed = 10.0f;
    
    private Rigidbody2D rb;
    private EntityState state = new();
    private KeyState input;
    private int ray_floor_mask;
    
    private float player_height = 0.0f;
    private float distance_to_ground = 999.0f;
    private float ground_threshold = 0.25f;

    void raycast_to_floor() {
        Vector2 origin = (Vector2)transform.position + Vector2.down * (player_height + 0.1f);
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(
            origin, 
            direction, 
            0.6f,
            ray_floor_mask
        );

        // ignore hits on jump
        if (hit && rb.linearVelocityY >= 0f) {
            return;
        }

        if (hit.collider != null) {
            distance_to_ground = hit.distance;
            
            if (distance_to_ground <= ground_threshold) {
                if (!state.has_action(Action.GROUND)) {
                    state.add_action(Action.GROUND);
                }

                if (state.has_action(Action.JUMP)) {
                    state.remove_action(Action.JUMP);
                }
            } else {
                // too far to the ground, so remove the ground action
                state.remove_action(Action.GROUND);
            }
        } else {
            // if we're also not hitting shit, assume not on ground
            state.remove_action(Action.GROUND);
            distance_to_ground = 999.0f;
        }
    }

    void Start() {
        input = KeyState.instance;
        ray_floor_mask = LayerMask.GetMask("Floor");
        rb = GetComponent<Rigidbody2D>();

        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        player_height = bc.bounds.extents.y;
    }

    void Update() {
        raycast_to_floor();

        bool is_jump_down = input.is_down(GameKey.SPACE);
        bool is_pressing_sprint = input.is_pressed(GameKey.SHIFT);
        bool is_pressing_slide = input.is_pressed(GameKey.C);
        
        // jump
        if (is_jump_down && state.has_action(Action.GROUND)) {
            rb.AddForceY(jump_y, ForceMode2D.Impulse);
            state.add_action(Action.JUMP);
        }

        float h = Input.GetAxis("Horizontal");

        // slide
        if (is_pressing_slide && state.has_action(Action.WALK) && !state.has_action(Action.SLIDE)) {
            state.add_action(Action.SLIDE);
        }

        float abs_h = Mathf.Abs(h);

        // horizontal velocity
        if (abs_h > 0) {
            float target_speed = walk_speed;

            if (state.has_action(Action.SPRINT)) {
                target_speed *= 2f;
            }

            // cancel slide if too slow
            if (abs_h < 0.25f && state.has_action(Action.SLIDE)) {
                state.remove_action(Action.SLIDE);
            }

            rb.linearVelocity = new Vector2(target_speed * h, rb.linearVelocityY);
            state.add_action(Action.WALK);
        } else {
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
            state.remove_action(Action.WALK);
        }

        // add sprint
        if (Math.Abs(h) > 0 && is_pressing_sprint && !state.has_action(Action.SPRINT)) {
            state.add_action(Action.SPRINT);
        }

        // remove sprint
        if (!is_pressing_sprint && state.has_action(Action.SPRINT)) {
            state.remove_action(Action.SPRINT);
        }
    }
}
