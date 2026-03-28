using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private GameCore core;
    public GameEntity ent;

    public bool sprint_held = false;

    private readonly float default_fov = 35.0f;
    private Vector3 camera_offset = new(0, 0, 0);

    void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Player");

        if (core == null) {
            core = GameCore.Instance;
        }

        ent = gameObject.AddComponent<GameEntity>();

        // other events
        ent.attributes.OnDeath += do_death;
    }

    void OnEnable() {
        SubscribeInput();
    }

    void OnDisable() {
        UnsubscribeInput();
    }

    void OnDestroy() {
        UnsubscribeInput();

        if (ent != null && ent.attributes != null) {
            ent.attributes.OnDeath -= do_death;
        }
    }

    void Start() {
        core.ui_manager.player_ui.Show();
        core.camera_controller.set_focus(gameObject, default_fov, camera_offset);
    }

    void Update() {
        var attr = ent.attributes;

        // update ui
        core.ui_manager.player_ui.UpdateStamina(attr.stamina.value, attr.stamina.max);
        core.ui_manager.player_ui.UpdateHealth(attr.health.value, attr.health.max);

        ent.movement.sprint(sprint_held);
    }

    private void do_pause(InputAction.CallbackContext ctx) {
        if (ent == null) return;
        if (ent.attributes.health.value == 0.0f) {
            return;
        }        

        // access panel -> pause
        if (core.ui_manager.access_panel.Visible()) {
            core.ui_manager.access_panel.Clear();
            core.ui_manager.access_panel.Hide();
        } else {
            core.ui_manager.pause_panel.Toggle();
        }
    }

    private void do_walk(InputAction.CallbackContext ctx) {
        if (ent == null) return;
        ent.movement.set_direction(ctx.ReadValue<Vector2>());
    }

    private void do_jump(InputAction.CallbackContext ctx) {
        if (ent == null) return;
        ent.movement.jump();
    }

    private void do_sprint(InputAction.CallbackContext ctx) {
        if (ent == null) return;
        float target_fov = default_fov;

        if (ctx.performed) {
            target_fov += 10;
        }

        core.camera_controller.set_focus(gameObject, target_fov, camera_offset);
        sprint_held = ctx.performed;
    }

    private void do_slide(InputAction.CallbackContext ctx) {
        if (ent == null) return;
        ent.movement.slide(ctx.performed);
    }

    private void do_attack(InputAction.CallbackContext ctx) {
        if (ent == null || ent.combat == null) return;
        ent.combat.Attack(1.5f, 1.0f, LayerMask.GetMask("Enemy"));
    }

    private void do_death() {
        core.ui_manager.death_ui.Show();
    }

    private void SubscribeInput() {
        if (core == null || core.input == null) return;

        // movement input
        core.input.Player.Movement.performed += do_walk;
        core.input.Player.Movement.canceled += do_walk;

        core.input.Player.Jump.performed += do_jump;
        core.input.Player.Jump.canceled += do_jump;

        core.input.Player.Sprint.performed += do_sprint;
        core.input.Player.Sprint.canceled += do_sprint;

        core.input.Player.Slide.performed += do_slide;
        core.input.Player.Slide.canceled += do_slide;

        // ui input
        core.input.Player.Pause.performed += do_pause;

        // combat input
        core.input.Player.Attack.performed += do_attack;
    }

    private void UnsubscribeInput() {
        if (core == null || core.input == null) return;

        core.input.Player.Movement.performed -= do_walk;
        core.input.Player.Movement.canceled -= do_walk;

        core.input.Player.Jump.performed -= do_jump;
        core.input.Player.Jump.canceled -= do_jump;

        core.input.Player.Sprint.performed -= do_sprint;
        core.input.Player.Sprint.canceled -= do_sprint;

        core.input.Player.Slide.performed -= do_slide;
        core.input.Player.Slide.canceled -= do_slide;

        core.input.Player.Pause.performed -= do_pause;
        core.input.Player.Attack.performed -= do_attack;
    }
};
