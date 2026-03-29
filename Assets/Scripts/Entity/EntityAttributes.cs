using System;
using UnityEngine;

public struct Attr<T> where T : IComparable<T>
{
    private T _value;
    private T _max;

    public T max {
        readonly get => _max;
        set {
            _max = value;
            _value = Clamp(_value);
        }
    }

    public T value {
        readonly get => _value;
        set => _value = Clamp(value);
    }

    private readonly T Clamp(T v) {
        if (v.CompareTo(_max) > 0) { return _max; }
        if (v.CompareTo(default) < 0) { return default; }
        return v;
    }

    public Attr(T value) {
        _value = default;
        _max = value;
        this.value = value;
    }
}

public class EntityAttributes : MonoBehaviour 
{
    public GameEntity ent;

    // events
    public Action<HitInfo> OnDeath;
    public Action<HitInfo> OnDamage;

    public Attr<float> velocity = new(10.0f);
    public Attr<float> health = new(10.0f);
    public Attr<float> stamina = new(5.0f);
    public Attr<float> push_force = new(3.5f);
    public Attr<float> jump_force = new(5.0f);
    public Attr<float> wall_jump_delay = new(0.050f); // seconds

    void Awake() {
        ent = GetComponent<GameEntity>();
    }

    public void TakeDamage(HitInfo hit) {
        health.value -= hit.damage;
        OnDamage?.Invoke(hit);

        Vector3 dir = (transform.position - hit.position).normalized;
        ent.movement.add_impulse(dir.x * hit.force, dir.y);

        if (health.value <= 0) {
            OnDeath?.Invoke(hit);
        }
    }

    public void ClearEvents() {
        OnDeath = null;
        OnDamage = null;
    }
};
