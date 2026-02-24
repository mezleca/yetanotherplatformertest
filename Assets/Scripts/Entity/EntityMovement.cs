using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityMovement : MonoBehaviour
{
    public float jump_force = 10.0f, walk_speed = 10.0f;
    public bool on_walk = false;
    public bool on_sprint = false;
    public bool on_slide = false;

    public Rigidbody2D rb;
    public Vector2 direction; // to move horizontally and maybe upwards (stairs or something like that) in the future

    public float velocity_multiplier = 1.0f;
    public float wall_jump_vel = 15.0f;
    public float sprint_vel = 1.5f;
    public float slide_vel = 1.0f;

    private EntitySensor sensor;
    private EntityAction input_action;
    private GameUtils utils;

    public void setup(EntitySensor s, EntityAction a)
    {
        sensor = s;
        input_action = a;
        sensor.onHit += on_hit;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnDestroy()
    {
        if (sensor == null)
        {
            return;
        }

        sensor.onHit -= on_hit;
    }

    public void set_direction(Vector2 dir)
    {
        direction = dir;
        on_walk = Mathf.Abs(direction.x) > 0;
    }

    public void jump(float y = 0.0f)
    {
        if (sensor.is_grounded)
        {
            rb.AddForceY(y > 0.0f ? y : jump_force, ForceMode2D.Impulse);
        }
    }

    public void sprint(bool holding)
    {
        if (holding && sensor.is_grounded)
        {
            velocity_multiplier = Math.Min(velocity_multiplier + sprint_vel, 2.0f);
            on_sprint = true;
        }
        else
        {
            velocity_multiplier = Math.Max(velocity_multiplier - sprint_vel, 1.0f);
            on_sprint = false;
        }
    }

    public void slide(bool holding)
    {
        if (holding && on_walk)
        {
            velocity_multiplier = Math.Min(velocity_multiplier + slide_vel, 3.0f);
            on_slide = true;
        }
        else
        {
            velocity_multiplier = Math.Max(velocity_multiplier - slide_vel, 1.0f);
            on_slide = false;
        }
    }

    void on_hit(GameSide side, Collider2D collider)
    {
        if (side == GameSide.RIGHT || side == GameSide.LEFT)
        {
            bool is_holding_jump = input_action.has(EntityActions.JUMP, true);

            if (is_holding_jump && !sensor.is_grounded && sensor.is_touching[(int)side])
            {
                float dir = side == GameSide.RIGHT ? -1.0f : 1.0f;
                Vector2 force = new(wall_jump_vel * dir, wall_jump_vel);
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }

    void FixedUpdate()
    {
        float target_x = walk_speed * velocity_multiplier * -direction.x;
        float target_y = rb.linearVelocityY; 

        // TODO: clamp based on ent attributes
        target_x = Mathf.Clamp(target_x, -30.0f, 30.0f);
        target_y = Mathf.Clamp(target_y, -30.0f, 30.0f);

        // who cares about gravity
        if (!sensor.is_grounded)
        {
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocityX, target_x, 0.10f), target_y);
            return;
        }

        rb.linearVelocity = new Vector2(target_x, target_y);
    }
};
