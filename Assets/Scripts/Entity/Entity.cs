using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(EntityMovement))]
[RequireComponent(typeof(EntitySensor))]

public class GameEntity : MonoBehaviour
{
    public EntityMovement movement;
    public EntitySensor sensor;

    void Awake()
    {
        movement = GetComponent<EntityMovement>();
        sensor = GetComponent<EntitySensor>();
        
        if (movement == null)
        {
            movement = gameObject.AddComponent<EntityMovement>();
        }
        
        if (sensor == null)
        {
            sensor = gameObject.AddComponent<EntitySensor>();
        }

        movement.setup(sensor);
    }
};
