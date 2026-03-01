using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GameEntity))]
public class Player : MonoBehaviour
{
    public GameEntity ent;
    private PlayerInput input;
    private PauseUI pause_ui;
    private bool callbacks_bound = false;
    private readonly float default_fov = 120.0f;
    private Vector3 camera_offset = new(0, 0, 0);
    [SerializeField] public new CameraController camera;

    private void Awake()
    {
        ensure_initialized();
    }

    private void bind_callbacks()
    {
        if (callbacks_bound || input == null || ent == null)
        {
            return;
        }

        input.Player.Pause.performed += on_pause;

        input.Player.Movement.performed += do_walk;
        input.Player.Movement.canceled += do_walk;

        input.Player.Jump.performed += do_jump;
        input.Player.Jump.canceled += do_jump;

        input.Player.Sprint.performed += do_sprint;
        input.Player.Sprint.canceled += do_sprint;

        input.Player.Slide.performed += do_slide;
        input.Player.Slide.canceled += do_slide;

        ent.movement.on_wall_hit += on_wall_hit;

        callbacks_bound = true;
    }

    private void unbind_callbacks()
    {
        if (!callbacks_bound || input == null || ent == null)
        {
            return;
        }

        input.Player.Pause.performed -= on_pause;

        input.Player.Movement.performed -= do_walk;
        input.Player.Movement.canceled -= do_walk;

        input.Player.Jump.performed -= do_jump;
        input.Player.Jump.canceled -= do_jump;

        input.Player.Sprint.performed -= do_sprint;
        input.Player.Sprint.canceled -= do_sprint;

        input.Player.Slide.performed -= do_slide;
        input.Player.Slide.canceled -= do_slide;

        ent.movement.on_wall_hit -= on_wall_hit;

        callbacks_bound = false;
    }

    private void ensure_initialized()
    {
        input ??= new PlayerInput();
        
        if (ent == null)
        {
            ent = gameObject.AddComponent<GameEntity>();
        }

        if (camera == null)
        {
            camera = FindFirstObjectByType<CameraController>();
        }

        if (pause_ui == null)
        {
            pause_ui = FindFirstObjectByType<PauseUI>();
        }

        bind_callbacks();
    }

    void Start()
    {
        camera.set_focus(gameObject, default_fov, camera_offset);  
    }

    private void OnEnable()
    {
        ensure_initialized();
        input.Enable();
    }

    private void OnDisable()
    {
        input?.Disable();
    }

    private void OnDestroy()
    {
        unbind_callbacks();
        input?.Dispose();
        input = null;
    }

    // handle ui stuff
    private void on_pause(InputAction.CallbackContext ctx)
    {
        pause_ui.toggle_pause();
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
        InputAction jump_action = input.FindAction("Jump");
        
        if (has_stamina && jump_action.IsPressed() && !ent.sensor.is_grounded && ent.sensor.is_touching[(int)side])
        {
            // TOFIX
            Vector2 move_pos = input.FindAction("Movement").ReadValue<Vector2>();
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
