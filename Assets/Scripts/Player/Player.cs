using UnityEngine;

public class Player : MonoBehaviour
{
    private GameEntity ent;
    private PlayerInput input;
    private float default_fov = 120.0f;
    [SerializeField] public CameraController camera;

    private void Awake()
    {
        ent = gameObject.AddComponent<GameEntity>();
        input = new PlayerInput();

        input.Player.Movement.performed += ent.walk;
        input.Player.Movement.canceled += ent.walk;

        input.Player.Jump.performed += ent.jump;
        input.Player.Jump.canceled += ent.jump;

        input.Player.Sprint.performed += onSprint;
        input.Player.Sprint.canceled += onSprint;

        input.Player.Slide.performed += ent.slide;
        input.Player.Slide.canceled += ent.slide;
    }

    void Start()
    {
        camera.set_focus(gameObject, 100.0f);
    }

    private void OnDestroy()
    {
        input.Player.Movement.performed -= ent.walk;
        input.Player.Movement.canceled -= ent.walk;

        input.Player.Jump.performed -= ent.jump;
        input.Player.Jump.canceled -= ent.jump;

        input.Player.Sprint.performed -= onSprint;
        input.Player.Sprint.canceled -= onSprint;

        input.Player.Slide.performed -= ent.slide;
        input.Player.Slide.canceled -= ent.slide;
    }

    private void onSprint(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        float target_fov = default_fov;
        
        if (ent.sensor.is_grounded && ctx.performed)
        {
            target_fov += 10;
        }

        camera.set_focus(gameObject, target_fov);
        ent.sprint(ctx);
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
};
