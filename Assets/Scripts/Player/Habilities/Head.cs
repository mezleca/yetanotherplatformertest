using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHead : MonoBehaviour 
{
    public Action onDestroy;
    public Action<int> onHit;

    public Rigidbody2D rb;

    private GameCore core;

    public Vector2 focus_target = Vector2.zero;
    private Vector2 move_input = Vector2.zero;

    void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Player Head"); 

        core = GameCore.Instance;

        rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();

        var mat = new PhysicsMaterial2D("HeadFriction")
        {
            friction = 1.0f,
            bounciness = 0.0f
        };

        col.sharedMaterial = mat;
    }

    void MoveInput(InputAction.CallbackContext ctx) {
        move_input = ctx.ReadValue<Vector2>();
        focus_target = new Vector2(0.0f, move_input.y * 3.0f);
    }

    void OnAttack(InputAction.CallbackContext ctx) {
        onDestroy?.Invoke();
        Destroy(gameObject);
    }

    void Update() {
        Vector2 cur_offset = core.camera_controller.cur_offset;
        core.camera_controller.set_focus(gameObject, 20.0f, Vector2.Lerp(cur_offset, focus_target, 0.3f));
    }

    void FixedUpdate() {
        if (rb == null) return;

        rb.linearVelocity = new(
            Math.Clamp(rb.linearVelocityX + move_input.x, float.MinValue, 5.0f),
            rb.linearVelocityY
        );
    }

    void OnCollisionEnter2D(Collision2D collisionInfo) {
        onHit?.Invoke(collisionInfo.collider.gameObject.layer);
    }

    void OnEnable() {
        SubscribeInput();
    }

    void OnDisable() {
        UnsubscribeInput();
    }

    void OnDestroy() {
        UnsubscribeInput();
    }

    private void SubscribeInput() {
        if (core == null || core.input == null) return;

        // movement input
        core.input.Player.Movement.performed += MoveInput;
        core.input.Player.Movement.canceled += MoveInput;

        core.input.Player.Attack.performed += OnAttack;
    }

    private void UnsubscribeInput() {
        if (core == null || core.input == null) return;

        core.input.Player.Movement.performed -= MoveInput;
        core.input.Player.Movement.canceled -= MoveInput;

        core.input.Player.Attack.performed -= OnAttack;
    }
}
