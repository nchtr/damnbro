using UnityEngine;
using Damnbro.Weapons;

namespace Damnbro.Enemies
{
    public class EnemyRanged : EnemyBase
    {
        [Header("Ranged")]
        public Projectile projectilePrefab;
        public Transform muzzle;
        public float projectileSpeed = 18f;
        public float preferredDistance = 14f;
        public float kiteSpeedMultiplier = 0.85f;

        protected override void Start()
        {
            base.Start();
            if (muzzle == null) muzzle = transform;
            if (agent != null)
            {
                attackRange = preferredDistance;
                agent.speed *= kiteSpeedMultiplier;
            }
        }

        protected override void Update()
        {
            if (_health.IsDead || _target == null) return;
            float dist = Vector3.Distance(transform.position, _target.position);
            if (dist > sightRange) { if (agent != null && agent.isOnNavMesh) agent.ResetPath(); return; }

            if (agent != null && agent.isOnNavMesh)
            {
                Vector3 toTarget = _target.position - transform.position;
                Vector3 desired = dist < preferredDistance * 0.7f
                    ? transform.position - toTarget.normalized * preferredDistance
                    : _target.position;
                agent.SetDestination(desired);
            }

            Vector3 facing = _target.position - transform.position;
            facing.y = 0;
            if (facing.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(facing);

            if (dist <= sightRange && Time.time >= _nextAttackTime && HasLineOfSight())
            {
                _nextAttackTime = Time.time + attackCooldown;
                Attack();
            }
        }

        bool HasLineOfSight()
        {
            if (muzzle == null || _target == null) return false;
            Vector3 dir = _target.position + Vector3.up - muzzle.position;
            return !Physics.Raycast(muzzle.position, dir.normalized, out RaycastHit hit, dir.magnitude, ~0, QueryTriggerInteraction.Ignore)
                   || hit.transform.IsChildOf(_target);
        }

        protected override void Attack()
        {
            if (muzzle == null || _target == null) return;
            Vector3 aim = (_target.position + Vector3.up - muzzle.position).normalized;
            Projectile p = projectilePrefab != null
                ? Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(aim))
                : BuildFallbackShot(muzzle.position, aim);
            p.Launch(aim * projectileSpeed, gameObject);
        }

        Projectile BuildFallbackShot(Vector3 pos, Vector3 dir)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "EnemyShot";
            go.transform.position = pos;
            go.transform.rotation = Quaternion.LookRotation(dir);
            go.transform.localScale = Vector3.one * 0.3f;
            var rend = go.GetComponent<MeshRenderer>();
            if (rend != null) rend.material.color = new Color(1f, 0.4f, 0f);
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            var proj = go.AddComponent<Projectile>();
            proj.damage = 12f;
            proj.lifetime = 5f;
            return proj;
        }
    }
}
