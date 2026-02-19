using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Collider2D))]

public class GameEntity : MonoBehaviour
{
    public EntityMovement movement;
    public EntityAction input_action;
    public EntitySensor sensor;

    void Start()
    {
        input_action = new EntityAction();
        movement = gameObject.AddComponent<EntityMovement>();
        sensor = gameObject.AddComponent<EntitySensor>();
        movement.setup(sensor, input_action);
    }

    // input events
    public void walk(InputAction.CallbackContext ctx)
    {
        movement.set_direction(ctx.ReadValue<Vector2>());
        input_action.set(EntityActions.WALK, ctx.performed);
    }

    public void jump(InputAction.CallbackContext ctx)
    {
        movement.jump();
        input_action.set(EntityActions.JUMP, ctx.performed);
    }

    public void sprint(InputAction.CallbackContext ctx)
    {
        movement.sprint(ctx.performed);
        input_action.set(EntityActions.SPRINT, ctx.performed);
    }

    public void slide(InputAction.CallbackContext ctx)
    {
        movement.slide(ctx.performed);
        input_action.set(EntityActions.SLIDE, ctx.performed);
    }
};
