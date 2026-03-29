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
    SpriteRenderer target_sprite;

    RaycastHit2D hit;

    // combat bs
    private float last_hit = 0.0f;
    private readonly float ATTACK_DELAY = 0.350f; // seconds 

    // enemy attributes
    private readonly float MIN_DISTANCE_TRIGGER = 5.0f;
    private readonly float MIN_ATTACK_TRIGGER = 2.0f;
    private readonly float ATTACK_AMMOUNT = 0.5f;

    // enemy state
    private EnemyState state = EnemyState.Idle;
    private float state_time = 0f;

    public GameEntity ent;

    void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        ent = gameObject.AddComponent<GameEntity>();

        // NERF
        ent.attributes.health.max = 5.0f;
        ent.attributes.velocity.max = 5.0f;
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
        LayerMask mask = LayerMask.GetMask("Wall", "Floor", "World", "Enemy", "Player Head");

        if (state == EnemyState.Idle) {
            hit = Physics2D.Raycast(transform.position, direction, MIN_DISTANCE_TRIGGER, ~mask);
        }
    }

    void TickIdle() {
        if (hit.collider == null) return;
        FoundTarget();
    }

    void TickSearching() {
        // give up if we spend more than 5 seconds searching the player with no success
        if (Time.time - state_time > 5) {
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

        // give up if player is too far
        if (distance > 10.0f) {
            UpdateState(EnemyState.Searching);
            return;
        }

        float entity_height = transform.position.y + ent.sprite.bounds.extents.y;
        float target_height = target_pos.y + target_sprite.bounds.extents.y;

        Vector2 normalized_dir = (Vector2)(transform.position - target_pos).normalized;
        ent.movement.set_direction(normalized_dir);

        // if the player is too high, attempt a jump
        if (target_height - entity_height > 0.0f && ent.sensor.is_grounded) {
            ent.movement.jump(true);
        }
        
        if (distance < MIN_ATTACK_TRIGGER && Time.time - last_hit > ATTACK_DELAY) {  
            HitInfo hit = new()
            {
                damage = ATTACK_AMMOUNT,
                force = 5.0f,
                range = 1.0f,
                mask = LayerMask.GetMask("Player"),
                position = transform.position
            };

            ent.combat.Attack(hit);
            last_hit = Time.time;
        }
    }

    void OnDeath(HitInfo hit) {
        Destroy(gameObject);
    }

    void FoundTarget() {
        target = hit.collider.gameObject;
        target_sprite = target.GetComponent<SpriteRenderer>();
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
        target_sprite = null;
    }
};
