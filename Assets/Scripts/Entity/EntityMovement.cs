using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]

public class EntityMovement : MonoBehaviour
{
    private GameEntity ent;

    public bool on_walk = false;
    public bool on_jump = false;
    public bool on_sprint = false;
    public bool on_slide = false;

    public GameSide last_hit_side;

    public Vector2 direction = new(0.0f, 0.0f);
    public Vector2 external_velocity = new(0.0f, 0.0f);

    public bool can_wall_jump = false;

    public float sprint_vel = 1.5f;
    public float slide_vel = 1.0f;
    public float last_wall_jump = 0.0f;

    void Start() {
        ent = gameObject.GetComponent<GameEntity>();

        ent.sensor.onHit -= on_hit;
        ent.sensor.onHit += on_hit;
    }

    void OnDestroy() {
        if (ent != null && ent.sensor != null) {
            ent.sensor.onHit -= on_hit;
        }
    }

    public void add_impulse(float x, float y) {
        external_velocity.x += x;
        external_velocity.y += y;
    }

    public void set_direction(Vector2 dir) {
        direction = dir;

        if (direction.x != 0) {
            ent.sprite.flipX = direction.x == -1;
        }

        on_walk = Mathf.Abs(direction.x) > 0;
    }

    public void jump(bool holding) {
        if (!holding) return;

        bool can_jump = ent.sensor.is_grounded && !on_jump && ent.attributes.stamina.value > 0;

        if (can_jump) {
            reset_velocity();
            add_impulse(0.0f, ent.attributes.jump_force.value);

            ent.attributes.stamina.value -= 1.5f;
            on_jump = true;
        } else if (can_wall_jump) {
            float dir = last_hit_side == GameSide.RIGHT ? -1.0f : 1.0f;

            reset_velocity();
            add_impulse(5.0f * dir, ent.attributes.jump_force.value);

            last_wall_jump = Time.time;
            can_wall_jump = false;
            on_jump = true;
        } else {
            reset_y_velocity();
        }
    }

    public void sprint(bool holding) {
        bool can_sprint = holding && ent.attributes.stamina.value > 2.5f;
        on_sprint = can_sprint;
    }

    public void slide(bool holding) {
        bool can_slide = holding && on_walk;
        on_slide = can_slide;
    }

    private void on_hit(GameSide side, Collider2D collider) {
        can_wall_jump = (side == GameSide.RIGHT || side == GameSide.LEFT)
            && ent.attributes.stamina.value > 0 && Time.time - last_wall_jump > ent.attributes.wall_jump_delay.value;
        
        if (side == GameSide.RIGHT || side == GameSide.LEFT) last_hit_side = side;
        if (side == GameSide.BOTTOM) on_jump = false;
    }

    void reset_velocity() {
        external_velocity = Vector2.zero;
        ent.rb.linearVelocity = Vector2.zero;
    }

    void reset_y_velocity() {
        external_velocity = new(external_velocity.x, 0.0f);
        ent.rb.linearVelocity = new(ent.rb.linearVelocityX, 0.0f);
    }

    void Update() {
        float abs_x = MathF.Abs(ent.velocity.x);

        bool is_spriting = on_walk && on_sprint && abs_x > 0;
        bool is_walking = on_walk && abs_x > 0;

        if (is_spriting) {
            ent.attributes.stamina.value -= 1.5f * Time.deltaTime;
        } else if (is_walking) {
            ent.attributes.stamina.value -= 0.5f * Time.deltaTime;
        } else {
            ent.attributes.stamina.value += 5.0f * Time.deltaTime;
        }
    }

    void FixedUpdate() {            
        float max_vel = ent.attributes.velocity.max;
        float multiplier = on_slide ? 3.0f : on_sprint ? 2.0f : 1.0f;
        float speed = ent.attributes.velocity.value * multiplier;
        float target_x = Math.Clamp(direction.x != 0.0f ? direction.x * speed : 0.0f, -max_vel, max_vel);

        ent.rb.linearVelocity = new(
            target_x + external_velocity.x,
            ent.rb.linearVelocity.y + external_velocity.y
        );

        external_velocity.x = Mathf.Lerp(external_velocity.x, 0.0f, 0.3f);
        external_velocity.y = Mathf.Lerp(external_velocity.y, 0.0f, 0.3f);
    }
};
