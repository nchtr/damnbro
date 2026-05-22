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
            if (agent != null) attackRange = preferredDistance;
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
                agent.speed *= kiteSpeedMultiplier;
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
            if (projectilePrefab == null || muzzle == null || _target == null) return;
            Vector3 aim = (_target.position + Vector3.up - muzzle.position).normalized;
            var p = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(aim));
            p.Launch(aim * projectileSpeed, gameObject);
        }
    }
}
