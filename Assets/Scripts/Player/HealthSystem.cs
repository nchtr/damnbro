using System;
using UnityEngine;

namespace Damnbro.Player
{
    public class HealthSystem : MonoBehaviour
    {
        public float maxHealth = 100f;
        public float regenDelay = 3f;
        public float regenPerSecond = 0f;

        public float Current { get; private set; }
        public bool IsDead => Current <= 0f;
        public event Action<float, float> OnChanged;
        public event Action<GameObject> OnDied;

        float _lastDamageTime = -999f;

        void Awake() => Current = maxHealth;

        void Update()
        {
            if (IsDead || regenPerSecond <= 0f) return;
            if (Time.time - _lastDamageTime < regenDelay) return;
            if (Current < maxHealth)
            {
                Current = Mathf.Min(maxHealth, Current + regenPerSecond * Time.deltaTime);
                OnChanged?.Invoke(Current, maxHealth);
            }
        }

        public void ApplyDamage(float amount, GameObject source = null)
        {
            if (IsDead || amount <= 0f) return;
            Current = Mathf.Max(0f, Current - amount);
            _lastDamageTime = Time.time;
            OnChanged?.Invoke(Current, maxHealth);
            if (Current <= 0f) OnDied?.Invoke(source);
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            Current = Mathf.Min(maxHealth, Current + amount);
            OnChanged?.Invoke(Current, maxHealth);
        }
    }
}
