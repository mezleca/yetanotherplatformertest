using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityMovement : MonoBehaviour {
    private readonly float GROUND_THRESHOLD = 0.10f;
    private readonly float GRAB_THRESHOLD = 0.05f;
    private Dictionary<GameSide, bool> blocked_sides = new() {
        { GameSide.LEFT, false },
        { GameSide.TOP, false },
        { GameSide.RIGHT, false },
        { GameSide.BOTTOM, false },
    };

    public float jump_force = 10.0f, walk_speed = 5.0f;
    public bool can_jump = false;
    public bool on_walk = false;
    public bool on_sprint = false;
    public bool on_slide = false;

    public Rigidbody2D rb;
    public Vector2 direction;
    public float velocity_multiplier = 1.0f;
    public float sprint_vel = 1.5f;
    public float slide_vel = 1.0f;

    private EntitySensor sensor;
    private EntityAction input_action;
    private GameUtils utils;

    public void setup(EntitySensor s, EntityAction a) {
        sensor = s;
        input_action = a;
        sensor.onHit += on_hit;
        sensor.onClear += on_clear;
    }

    void OnDestroy() {
        if (sensor == null) {
            return;
        }

        sensor.onHit -= on_hit;
        sensor.onClear -= on_clear;
    }

    public void set_direction(Vector2 dir) {
        direction = dir;
        on_walk = Mathf.Abs(direction.x) > 0;
    }

    public void jump(float y = 0.0f) {
        rb.AddForceY(y > 0.0f ? y : jump_force, ForceMode2D.Impulse);
        can_jump = false;
    }

    public void sprint(bool holding) {
        if (holding) {
            velocity_multiplier = Math.Min(velocity_multiplier + sprint_vel, 3.0f);
            on_sprint = true;
        } else {
            velocity_multiplier = Math.Max(velocity_multiplier - sprint_vel, 1.0f);
            on_sprint = false;
        }
    }

    public void slide(bool holding) {
        if (holding && on_walk) {
            velocity_multiplier = Math.Min(velocity_multiplier + slide_vel, 3.0f);
            on_slide = true;
        } else {
            velocity_multiplier = Math.Max(velocity_multiplier - slide_vel, 1.0f);
            on_slide = false;
        }
    }

    void on_clear(GameSide side) {
        if (side == GameSide.BOTTOM) {
            can_jump = false;
        }
    }
    
    void on_hit(GameSide side, Collider2D collider) {
        if (side == GameSide.BOTTOM) {
            // ignore if upwards velocity
            if (rb.linearVelocityY >= 0) {
                return;
            }
            
            can_jump = sensor.distance[(int)side] <= GROUND_THRESHOLD;
        }

        if (side == GameSide.RIGHT || side == GameSide.LEFT) {
            float distance = sensor.distance[(int)side];
            bool is_holding_jump = (input_action.flags & EntityActions.JUMP) != 0;

            if (is_holding_jump && !can_jump && distance <= GRAB_THRESHOLD) {
                // add velocity to the opposite side
                float dir = side == GameSide.RIGHT ? -1 : 1;
                rb.linearVelocity = new Vector2(10.0f * dir, Mathf.Min(rb.linearVelocityY + 5.0f, 15.0f));
            }
        }
    }

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        float target_x = walk_speed * velocity_multiplier * -direction.x;
        bool is_horizontal_side_blocked = blocked_sides[GameSide.LEFT] || blocked_sides[GameSide.RIGHT];

        // if horizontal input is blocked, let the current impulse carry the player
        if (is_horizontal_side_blocked || !can_jump) {
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocityX, target_x, 0.15f), rb.linearVelocityY);
            return;
        }

        rb.linearVelocity = new Vector2(target_x, rb.linearVelocityY);
    }
};