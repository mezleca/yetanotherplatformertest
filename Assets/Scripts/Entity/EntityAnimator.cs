using UnityEngine;

[RequireComponent(typeof(Animator))]

public class EntityAnimator : MonoBehaviour
{
    private GameEntity ent;
    private bool has_animator;

    public Animator animator;

    void Awake() {
        ent = gameObject.GetComponent<GameEntity>();
        animator = gameObject.GetComponent<Animator>();

        has_animator = animator != null && animator.runtimeAnimatorController != null;
    }

    void Update() {
        if (!has_animator) {
            return;
        }

        animator.SetFloat("Speed", Mathf.Abs(ent.velocity.x / ent.attributes.velocity.max));
        animator.SetFloat("yVel",  ent.velocity.y);
        animator.SetBool("onJump", ent.movement.on_jump);
        animator.SetBool("onGround", ent.on_ground);
    }
};
