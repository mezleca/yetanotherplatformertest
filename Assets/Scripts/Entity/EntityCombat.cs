using UnityEngine;

public struct HitInfo
{
    public float damage;
    public float range;
    public float force;
    public LayerMask mask;
    public Vector3 position;
}

public class EntityCombat : MonoBehaviour 
{
    // DEBUG
    bool should_draw_sphere = false;
    float sphere_radius = 0.0f;

    private GameCore core;
    private GameEntity ent;

    void Awake() {
        core = GameCore.Instance;
        ent = gameObject.GetComponent<GameEntity>();
    }

    public void Attack(HitInfo info) {
        var hits = Physics2D.OverlapCircleAll(transform.position, info.range, info.mask);

        should_draw_sphere = true;
        sphere_radius = info.range;
        core.utils.delay_then(0.5f, () => should_draw_sphere = false);

        for (int i = 0; i < hits.Length; i++) {
            var target = hits[i].GetComponentInParent<GameEntity>();

            if (target == null || target == ent) {
                continue;
            }

            target.attributes.TakeDamage(info);
        }
    }

    void OnDrawGizmos() {
        if (!should_draw_sphere) {
            return;
        }

        Gizmos.color = Color.crimson;
        Gizmos.DrawSphere(transform.position, sphere_radius);
    }
};