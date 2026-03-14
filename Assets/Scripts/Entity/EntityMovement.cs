using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EntityMovement : MonoBehaviour
{
    public bool on_walk = false;
    public bool on_jump = false;
    public bool on_sprint = false;
    public bool on_slide = false;

    public EntityAttributes attributes;
    public Rigidbody2D rb;
    public Vector2 direction;

    public float velocity_multiplier = 1.0f;
    public float sprint_vel = 1.5f;
    public float slide_vel = 1.0f;

    private EntitySensor sensor;
    private SpriteRenderer sprite;

    // movement events
    public Action<GameSide> on_wall_hit;

    public void setup(EntitySensor s)
    {
        sensor = s;

        if (sensor != null)
        {
            sensor.onHit -= on_hit;
            sensor.onHit += on_hit;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        attributes ??= new EntityAttributes();
    }

    void OnDestroy()
    {
        if (sensor == null)
        {
            return;
        }

        sensor.onHit -= on_hit;
    }

    public Vector3 velocity => rb.linearVelocity;
    public bool on_ground => sensor.is_grounded;

    public void set_direction(Vector2 dir)
    {
        direction = dir;
        if (direction.x != 0) sprite.flipX = direction.x == 1;
        on_walk = Mathf.Abs(direction.x) > 0;
    }

    public void jump(float y = 0.0f)
    {
        bool can_jump = sensor.is_grounded && !on_jump && attributes.stamina > 0;

        if (can_jump)
        {
            rb.AddForceY(y > 0.0f ? y : attributes.jump_force, ForceMode2D.Impulse);
            attributes.stamina -= 3.0f;
            on_jump = true;
        }
    }

    public void sprint(bool holding)
    {
        bool can_sprint = on_walk && holding && attributes.stamina > 2.5f;

        velocity_multiplier = can_sprint ?
            Math.Min(velocity_multiplier + sprint_vel, 2.0f) :
            Math.Max(velocity_multiplier - sprint_vel, 1.0f);

        on_sprint = can_sprint;
    }

    public void slide(bool holding)
    {
        bool can_slide = holding && on_walk;

        velocity_multiplier = can_slide ?
            Math.Min(velocity_multiplier + slide_vel, 3.0f) :
            Math.Max(velocity_multiplier - slide_vel, 1.0f);

        on_slide = can_slide;
    }

    private void on_hit(GameSide side, Collider2D collider)
    {
        if (side == GameSide.RIGHT || side == GameSide.LEFT)
        {
            on_wall_hit?.Invoke(side);
        }

        if (side == GameSide.BOTTOM)
        {
            if (on_ground && on_jump) on_jump = false;
        }
    }

    void Update()
    {
        // update stamina
        if (on_sprint)
        {
            attributes.stamina -= 2.0f * Time.deltaTime;
        }
        else
        {
            attributes.stamina += 5.0f * Time.deltaTime;
        }

        attributes.stamina = Mathf.Clamp(attributes.stamina, 0.0f, attributes.max_stamina);
    }

    void FixedUpdate()
    {
        float target_x = attributes.max_x_speed * velocity_multiplier * -direction.x;
        float target_y = rb.linearVelocityY;

        // TODO: clamp based on ent attributes
        target_x = Mathf.Clamp(target_x, -15.0f, 15.0f);
        target_y = Mathf.Clamp(target_y, -50.0f, 15.0f);

        if (!sensor.is_grounded)
        {
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocityX, target_x, 0.10f), target_y);
            return;
        }

        rb.linearVelocity = new Vector2(target_x, target_y);
    }
};
