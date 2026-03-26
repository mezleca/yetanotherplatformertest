using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GameEntity))]
public class Player : MonoBehaviour
{
    private GameCore core;
    private new CameraController camera;

    public GameEntity ent;    

    private readonly float default_fov = 90.0f;
    private Vector3 camera_offset = new(0, 0, 0);

    void Start()
    {
        camera.set_focus(gameObject, default_fov, camera_offset);
    }

    void Awake()
    {
        if (core == null)
        {
            core = GameCore.Instance;
        }

        if (camera == null)
        {
            camera = CameraController.Instance;
        }

        if (ent == null)
        {
            ent = gameObject.AddComponent<GameEntity>();
        }

        core.input.Player.Movement.performed += do_walk;
        core.input.Player.Movement.canceled += do_walk;

        core.input.Player.Jump.performed += do_jump;
        core.input.Player.Jump.canceled += do_jump;

        core.input.Player.Sprint.performed += do_sprint;
        core.input.Player.Sprint.canceled += do_sprint;

        core.input.Player.Slide.performed += do_slide;
        core.input.Player.Slide.canceled += do_slide;

        ent.movement.on_wall_hit += on_wall_hit;
    }

    void OnDestroy()
    {
        core.input.Player.Movement.performed -= do_walk;
        core.input.Player.Movement.canceled -= do_walk;

        core.input.Player.Jump.performed -= do_jump;
        core.input.Player.Jump.canceled -= do_jump;

        core.input.Player.Sprint.performed -= do_sprint;
        core.input.Player.Sprint.canceled -= do_sprint;

        core.input.Player.Slide.performed -= do_slide;
        core.input.Player.Slide.canceled -= do_slide;

        ent.movement.on_wall_hit -= on_wall_hit;    
    }

    // handle movement stuff
    private void do_walk(InputAction.CallbackContext ctx)
    {
        ent.movement.set_direction(ctx.ReadValue<Vector2>());
    }

    private void do_jump(InputAction.CallbackContext ctx)
    {
        ent.movement.jump();
    }

    private void do_sprint(InputAction.CallbackContext ctx)
    {
        float target_fov = default_fov;

        if (ctx.performed)
        {
            target_fov += 10;
        }

        camera.set_focus(gameObject, target_fov, camera_offset);
        ent.movement.sprint(ctx.performed);
    }

    private void do_slide(InputAction.CallbackContext ctx)
    {
        ent.movement.slide(ctx.performed);
    }

    // custom movement events
    private void on_wall_hit(GameSide side)
    {
        bool has_stamina = ent.movement.attributes.stamina > 0;

        // TOFIX
        InputAction jump_action = core.input.FindAction("Jump");

        if (has_stamina && jump_action.IsPressed() && !ent.sensor.is_grounded && ent.sensor.is_touching[(int)side])
        {
            // TOFIX
            Vector2 move_pos = core.input.FindAction("Movement").ReadValue<Vector2>();
            Vector2 force = new(0, ent.movement.attributes.jump_force);

            // impulse player to the opposite direction
            float dir = side == GameSide.RIGHT ? -1.0f : 1.0f;

            if (move_pos.x == -dir)
            {
                force.x = ent.movement.attributes.push_force * dir;
            }

            ent.movement.rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
};
