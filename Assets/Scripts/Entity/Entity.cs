using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(EntityMovement))]
[RequireComponent(typeof(EntitySensor))]
[RequireComponent(typeof(EntityAnimator))]

public class GameEntity : MonoBehaviour
{
    public EntityMovement movement;
    public EntitySensor sensor;
    public EntityAnimator animator;

    void Awake()
    {
        movement = GetComponent<EntityMovement>();
        sensor = GetComponent<EntitySensor>();
        animator = GetComponent<EntityAnimator>();

        if (movement == null)
        {
            movement = gameObject.AddComponent<EntityMovement>();
        }

        if (sensor == null)
        {
            sensor = gameObject.AddComponent<EntitySensor>();
        }

        if (animator == null)
        {
            animator = gameObject.AddComponent<EntityAnimator>();
        }

        movement.setup(sensor);
    }
};
