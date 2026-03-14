using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EntityMovement))]
public class EntityAnimator : MonoBehaviour
{
    private Animator animator;
    private EntityMovement movement;

    void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<EntityMovement>();
    }

    void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(movement.velocity.x / movement.attributes.max_x_speed));
        animator.SetFloat("yVel",  movement.velocity.y);
        animator.SetBool("onJump", movement.on_jump);
        animator.SetBool("onGround", movement.on_ground);
    }
};
