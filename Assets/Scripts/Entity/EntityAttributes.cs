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

    public Attr(T value, T max) {
        _value = default;
        _max = max;
        this.value = value;
    }
}

public class EntityAttributes 
{
    // events
    public Action OnDeath;
    public Action OnDamage;

    public Attr<float> velocity = new(10.0f, 10.0f);
    public Attr<float> health = new(10.0f, 10.0f);
    public Attr<float> stamina = new(5.0f, 5.0f);
    public Attr<float> push_force = new(3.5f, 3.5f);
    public Attr<float> jump_force = new(15.0f, 15.0f);
    public Attr<float> wall_jump_delay = new(0.050f, 0.050f); // seconds

    public void TakeDamage(float value) {
        health.value -= value;
        OnDamage?.Invoke();

        Debug.Log($"ENTITY: took {value} damage | health: {health.value}");

        if (health.value <= 0) {
            Debug.Log("ENTITY: should be dead");
            OnDeath?.Invoke();
        }
    }

    public void ClearEvents() {
        OnDeath = null;
        OnDamage = null;
    }
};
