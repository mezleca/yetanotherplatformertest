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
    public Vector2 direction;

    public bool can_wall_jump = false;

    public float velocity_multiplier = 1.0f;
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

    public void set_direction(Vector2 dir) {
        direction = dir;

        if (direction.x != 0) {
            ent.sprite.flipX = direction.x == 1;
        }

        on_walk = Mathf.Abs(direction.x) > 0;
    }

    public void jump(float y = 0.0f) {
        bool can_jump = ent.sensor.is_grounded && !on_jump && ent.attributes.stamina.value > 0;

        if (can_jump) {
            ent.rb.AddForceY(y > 0.0f ? y : ent.attributes.jump_force.value, ForceMode2D.Impulse);
            ent.attributes.stamina.value -= 1.5f;
            on_jump = true;
        }

        if (can_wall_jump) {
            Vector2 force = new(0, ent.attributes.jump_force.value);

            // impulse player to the opposite direction
            float dir = last_hit_side == GameSide.RIGHT ? -1.0f : 1.0f;

            if (direction.x == -dir) {
                force.x = ent.attributes.push_force.value * dir;
            }

            ent.rb.AddForce(force, ForceMode2D.Impulse);

            last_wall_jump = Time.time;
            can_wall_jump = false;
            on_jump = true;
        }
    }

    public void sprint(bool holding) {
        bool can_sprint = holding && ent.attributes.stamina.value > 2.5f;

        velocity_multiplier = can_sprint ?
            Math.Min(velocity_multiplier + sprint_vel, 2.0f) :
            Math.Max(velocity_multiplier - sprint_vel, 1.0f);

        on_sprint = can_sprint;
    }

    public void slide(bool holding) {
        bool can_slide = holding && on_walk;

        velocity_multiplier = can_slide ?
            Math.Min(velocity_multiplier + slide_vel, 3.0f) :
            Math.Max(velocity_multiplier - slide_vel, 1.0f);

        on_slide = can_slide;
    }

    private void on_hit(GameSide side, Collider2D collider) {
        can_wall_jump = (side == GameSide.RIGHT || side == GameSide.LEFT)
            && ent.attributes.stamina.value > 0 && Time.time - last_wall_jump > ent.attributes.wall_jump_delay.value;
        
        if (side == GameSide.RIGHT || side == GameSide.LEFT) last_hit_side = side;
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
        if (ent.sensor.is_grounded && ent.rb.linearVelocityY <= 0.01f) {
            on_jump = false;
        }

        float target_x = ent.attributes.velocity.max * velocity_multiplier * -direction.x;
        float target_y = ent.rb.linearVelocityY;

        target_x = Mathf.Clamp(target_x, -15.0f, 15.0f);
        target_y = Mathf.Clamp(target_y, -50.0f, 15.0f);

        bool just_wall_jumped = Time.time - last_wall_jump < 0.25f;

        if (just_wall_jumped) {
            ent.rb.linearVelocity = new Vector2(
                ent.rb.linearVelocityX,
                Mathf.Clamp(ent.rb.linearVelocityY, -50f, 15f)
            );

            return;
        }
        
        ent.rb.linearVelocity = new Vector2(target_x, target_y);
    }
};
