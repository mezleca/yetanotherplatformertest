using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Collider2D))]

public class GameEntity : MonoBehaviour
{
    public BoxCollider2D box_collider;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;

    public EntityCombat combat;
    public EntityAttributes attributes;
    public EntityMovement movement;
    public EntitySensor sensor;
    public EntityAnimator animator;

    public Vector3 velocity => rb.linearVelocity;
    public bool on_ground => sensor.is_grounded;
    public bool is_flipped => sprite.flipX;

    void Awake() {
        box_collider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        // setup
        combat = gameObject.AddComponent<EntityCombat>();
        movement = gameObject.AddComponent<EntityMovement>();
        sensor = gameObject.AddComponent<EntitySensor>();
        animator = gameObject.AddComponent<EntityAnimator>();

        attributes ??= new EntityAttributes();

        // we dont want that
        var mat = new PhysicsMaterial2D("NoFriction")
        {
            friction = 0,
            bounciness = 0
        };

        box_collider.sharedMaterial = mat;
    }

    void OnDestroy() {
        attributes?.ClearEvents();
    }
};
