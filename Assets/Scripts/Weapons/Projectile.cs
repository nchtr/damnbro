using UnityEngine;
using Damnbro.Player;

namespace Damnbro.Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        public float damage = 30f;
        public float explosionRadius = 0f;
        public float lifetime = 6f;
        public LayerMask hitMask = ~0;
        public GameObject impactPrefab;

        Rigidbody _rb;
        GameObject _owner;
        bool _consumed;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
        }

        public void Launch(Vector3 velocity, GameObject owner)
        {
            _owner = owner;
            _rb.velocity = velocity;
            Destroy(gameObject, lifetime);
        }

        public void Reflect(GameObject newOwner, float speedMultiplier = 1.5f, float damageMultiplier = 2f)
        {
            _owner = newOwner;
            damage *= damageMultiplier;
            _rb.velocity = -_rb.velocity * speedMultiplier;
            transform.rotation = Quaternion.LookRotation(_rb.velocity.normalized);
        }

        void OnCollisionEnter(Collision col)
        {
            if (_consumed) return;
            if (_owner != null && col.collider.transform.IsChildOf(_owner.transform)) return;
            _consumed = true;
            Vector3 point = col.GetContact(0).point;

            if (explosionRadius > 0f)
            {
                var hits = Physics.OverlapSphere(point, explosionRadius, hitMask);
                foreach (var h in hits)
                {
                    var hp = h.GetComponentInParent<HealthSystem>();
                    if (hp != null)
                    {
                        float d = Mathf.Lerp(damage, damage * 0.3f, Vector3.Distance(point, h.transform.position) / explosionRadius);
                        hp.ApplyDamage(d, _owner);
                    }
                    var rb = h.attachedRigidbody;
                    if (rb != null) rb.AddExplosionForce(damage * 2f, point, explosionRadius, 0.5f, ForceMode.Impulse);
                }
            }
            else
            {
                var hp = col.collider.GetComponentInParent<HealthSystem>();
                hp?.ApplyDamage(damage, _owner);
            }

            if (impactPrefab != null) Instantiate(impactPrefab, point, Quaternion.LookRotation(col.GetContact(0).normal));
            Destroy(gameObject);
        }
    }
}
