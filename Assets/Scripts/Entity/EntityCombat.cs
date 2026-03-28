using UnityEngine;

public class EntityCombat : MonoBehaviour 
{
    private GameEntity ent;

    void Awake() {
        ent = gameObject.GetComponent<GameEntity>();
    }

    public void Attack(float ammount, float range, LayerMask mask) {
        var hits = Physics2D.OverlapCircleAll(transform.position, range, mask);

        for (int i = 0; i < hits.Length; i++) {
            var target = hits[i].GetComponentInParent<GameEntity>();

            if (target == null || target == ent) {
                continue;
            }

            target.attributes.TakeDamage(ammount);
        }
    }
};