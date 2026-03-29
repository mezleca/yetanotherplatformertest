using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerHability {
    NONE,
    INVISIBLE,
    HEADLESS
}

public class Player : MonoBehaviour
{
    private GameCore core;
    private GameInputState input;

    private PlayerHability hability = PlayerHability.NONE;

    public GameEntity ent;
    public GameObject head_prefab;

    private readonly float default_fov = 35.0f;
    private Vector3 camera_offset = new(0, 0, 0);

    void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Player");

        if (core == null) {
            core = GameCore.Instance;
        }

        ent = gameObject.AddComponent<GameEntity>();
        input ??= new GameInputState();

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

        // update movement
        ent.movement.sprint(input.IsHeld("Sprint"));
        ent.movement.jump(input.ConsumePress("Jump"));
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
        if (ent == null || hability != PlayerHability.NONE) return;
        ent.movement.set_direction(ctx.ReadValue<Vector2>());
    }

    private void do_jump(InputAction.CallbackContext ctx) {
        if (ent == null || hability != PlayerHability.NONE) return;
        input.SetPressed("Jump", ctx.performed);
    }

    private void do_sprint(InputAction.CallbackContext ctx) {
        if (ent == null || hability != PlayerHability.NONE) return;

        float target_fov = default_fov;

        if (ctx.performed) {
            target_fov += 10;
        }

        core.camera_controller.set_focus(gameObject, target_fov, camera_offset);
        input.SetHeld("Sprint", ctx.performed);
    }

    private void do_slide(InputAction.CallbackContext ctx) {
        if (ent == null || hability != PlayerHability.NONE) return;
        input.SetHeld("Slide", ctx.performed);
    }

    private void do_attack(InputAction.CallbackContext ctx) {
        if (ent == null || ent.combat == null || hability != PlayerHability.NONE) return;

        HitInfo hit = new()
        {
            damage = 1.5f,
            force = 2.0f,
            range = 1.0f,
            mask = LayerMask.GetMask("Enemy"),
            position = transform.position
        };

        ent.combat.Attack(hit);
    }

    private void do_death(HitInfo hit) {
        core.ui_manager.death_ui.Show();
    }

    private void do_headless(InputAction.CallbackContext ctx) {
        if (hability != PlayerHability.NONE) return;

        Vector3 target_pos = transform.position + (ent.sprite.bounds.extents * 2);
        GameObject obj = Instantiate(head_prefab, target_pos, transform.rotation);
        PlayerHead head = obj.GetComponent<PlayerHead>();

        head.onDestroy += () => 
        {
            hability = PlayerHability.NONE;
            core.camera_controller.set_focus(gameObject, default_fov, camera_offset);
        };

        head.onHit += layer =>
        {   
            int enemy_layer = LayerMask.NameToLayer("Enemy");

            if (layer == enemy_layer) {
                head.onDestroy?.Invoke(); // ...
                Destroy(head.gameObject);
            }
        };

        hability = PlayerHability.HEADLESS;
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

        // hability input
        core.input.Player.HabilityHeadless.performed += do_headless;

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
        core.input.Player.HabilityHeadless.performed -= do_headless;
    }
};
