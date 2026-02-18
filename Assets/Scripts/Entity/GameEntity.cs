using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class GameEntity : MonoBehaviour
{
    [SerializeField] public float jump_force = 10.0f;
    [SerializeField] public float walk_speed = 5.0f;
    [SerializeField] public float grip_strength = 2.0f;
    
    private Rigidbody2D rb;
    private int ray_floor_mask;

    // actions
    private bool on_jump = false;
    private bool on_walk = false;
    private bool on_ground = false;
    private bool on_sprint = false;
    private bool on_slide = false;
    
    private float entity_height = 0.0f;
    private float distance_to_ground = 999.0f;
    private Vector2 direction;
    private float velocity_multiplier = 1.0f;
    private readonly float ground_threshold = 0.25f;

    public void walk(InputAction.CallbackContext ctx) {
        direction = ctx.ReadValue<Vector2>();
        on_walk = Math.Abs(direction.x) > 0;
    }

    public void jump(InputAction.CallbackContext ctx) {
        if (ctx.performed && on_ground && !on_jump) {
            rb.AddForceY(jump_force, ForceMode2D.Impulse);
            on_jump = true;
        }
    }

    public void sprint(InputAction.CallbackContext ctx) {
        if (ctx.performed && on_walk) {
            velocity_multiplier += 1.0f;
            on_slide = true;
        } else {
            velocity_multiplier -= 1.0f;
            on_slide = false;
        }

        velocity_multiplier = Mathf.Max(1.0f, velocity_multiplier);
    }

    public void slide(InputAction.CallbackContext ctx) {
        if (ctx.performed && on_walk) {
            velocity_multiplier += 0.5f;
            on_slide = true;
        } else {
            velocity_multiplier -= 0.5f;
            on_slide = false;
        }

        velocity_multiplier = Mathf.Max(1.0f, velocity_multiplier);
    }

    void Awake() {
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        ray_floor_mask = LayerMask.GetMask("Floor");

        rb = GetComponent<Rigidbody2D>();
        entity_height = bc.bounds.extents.y;
    }

    void FixedUpdate() {
        rb.linearVelocity = new Vector2(walk_speed * velocity_multiplier * -direction.x, rb.linearVelocityY);
    }

    void Update() {
        raycast_to_floor();
    }

    void raycast_to_floor() {
        Vector2 origin = (Vector2)transform.position + Vector2.down * (entity_height + 0.1f);
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(
            origin, 
            direction,
            1.0f,
            ray_floor_mask
        );

        // ignore hits on jump
        if (hit && rb.linearVelocityY >= 0) {
            return;
        }

        if (hit.collider != null) {
            distance_to_ground = hit.distance;

            if (distance_to_ground <= ground_threshold) {
                // Debug.DrawRay(origin, direction * 5.0f, Color.blue, 0.5f);
                on_ground = true;
                on_jump = false;
            } else {
                // Debug.DrawRay(origin, direction * 5.0f, Color.green, 0.5f);
                on_ground = false;
            }
        } else {
            // if we're also not hitting shit, assume not on ground
            on_ground = false;
            distance_to_ground = 999.0f;
            // Debug.DrawRay(origin, direction * 5.0f, Color.red, 0.5f);
        }
    }
}
