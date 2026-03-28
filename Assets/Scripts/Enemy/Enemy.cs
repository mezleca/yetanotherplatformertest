using System;
using UnityEngine;

public enum EnemyState {
    Idle,
    Searching,
    Chasing
}

public class Enemy : MonoBehaviour
{   
    // target
    GameObject target;
    RaycastHit2D hit;

    // combat bs
    private float last_hit = 0.0f;
    private readonly float ATTACK_DELAY = 0.350f; // ms 

    // enemy attributes
    private readonly float MIN_DISTANCE_TRIGGER = 5.0f;
    private readonly float MIN_ATTACK_TRIGGER = 1.0f;
    private readonly float ATTACK_AMMOUNT = 0.5f;

    // enemy state
    private EnemyState state = EnemyState.Idle;
    private float state_time = 0f;

    public GameEntity ent;

    void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        ent = gameObject.AddComponent<GameEntity>();

        // NERF
        ent.attributes.health.max = 4.0f;
        ent.attributes.velocity.max = 3.0f;
        ent.attributes.jump_force.max = 5.0f;
        ent.attributes.stamina.max = 3.0f;

        ent.attributes.OnDeath += OnDeath;
    }

    void OnDestroy() {
        if (ent != null && ent.attributes != null) {
            ent.attributes.OnDeath -= OnDeath;
        }
    }

    void Update() {
        switch (state) {
            case EnemyState.Idle:      TickIdle();      break;
            case EnemyState.Searching: TickSearching(); break;
            case EnemyState.Chasing:   TickChasing();   break;
        }

        Vector2 direction = ent.is_flipped ? Vector2.right : Vector2.left;
        LayerMask mask = LayerMask.GetMask("Wall", "Floor", "World", "Enemy");

        hit = Physics2D.Raycast(transform.position, direction, MIN_DISTANCE_TRIGGER, ~mask);
    }

    void OnDeath() {
        Debug.Log("ENEMY: ded");
        Destroy(gameObject);
    }

    void TickIdle() {
        if (hit.collider == null) {
            return;
        }

        Debug.Log("ENEMY: found a mf");
        FoundTarget();
    }

    void TickSearching() {
        if (Time.time - state_time > 5) {
            Debug.Log("ENEMY: welp, cant find this mf");
            LostTarget(EnemyState.Idle);
            return;
        }

        if (hit.collider != null) {
            FoundTarget();
            return;
        }
    }

    void TickChasing() {
        if (target == null) {
            LostTarget(EnemyState.Searching);
            return;
        }

        Vector3 target_pos = target.transform.position;
        float distance = Vector2.Distance(transform.position, target_pos);

        if (distance > 10.0f) {
            Debug.Log("ENEMY: wheres the fucker");
            UpdateState(EnemyState.Searching);
            return;
        }

        Vector2 normalized_dir = (Vector2)(transform.position - target_pos).normalized;
        ent.movement.set_direction(normalized_dir);

        if (MathF.Abs(normalized_dir.y) > 0.50 && !ent.movement.on_jump) {
            ent.movement.jump();
        }
        
        if (distance < MIN_ATTACK_TRIGGER && Time.time - last_hit > ATTACK_DELAY) {    
            ent.combat.Attack(ATTACK_AMMOUNT, 1.0f, LayerMask.GetMask("Player"));
            last_hit = Time.time;
        }
    }

    void FoundTarget() {
        target = hit.collider.gameObject;
        UpdateState(EnemyState.Chasing);
    }

    void LostTarget(EnemyState state) {
        ClearTarget();
        UpdateState(state);
    }

    void UpdateState(EnemyState _state) {
        state = _state;
        state_time = Time.time;
    }

    void ClearTarget() {
        target = null;
    }
};
