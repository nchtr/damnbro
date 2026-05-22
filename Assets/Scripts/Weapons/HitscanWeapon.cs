using UnityEngine;
using Damnbro.Player;

namespace Damnbro.Weapons
{
    public class HitscanWeapon : WeaponBase
    {
        [Header("Hitscan")]
        public float damage = 25f;
        public float range = 200f;
        public int pellets = 1;
        public float spreadDegrees = 0f;
        public float impulse = 4f;
        public GameObject impactPrefab;
        public LineRenderer tracerPrefab;

        protected override void Fire()
        {
            if (viewCamera == null) return;
            Vector3 origin = viewCamera.transform.position;
            Vector3 baseDir = viewCamera.transform.forward;

            for (int i = 0; i < Mathf.Max(1, pellets); i++)
            {
                Vector3 dir = ApplySpread(baseDir, spreadDegrees);
                if (Physics.Raycast(origin, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
                {
                    var hp = hit.collider.GetComponentInParent<HealthSystem>();
                    hp?.ApplyDamage(damage, gameObject);
                    var rb = hit.collider.attachedRigidbody;
                    if (rb != null) rb.AddForceAtPosition(dir * impulse, hit.point, ForceMode.Impulse);
                    SpawnTracer(muzzle != null ? muzzle.position : origin, hit.point);
                    if (impactPrefab != null) Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                }
                else
                {
                    SpawnTracer(muzzle != null ? muzzle.position : origin, origin + dir * range);
                }
            }
        }

        Vector3 ApplySpread(Vector3 dir, float degrees)
        {
            if (degrees <= 0f) return dir;
            float yaw = Random.Range(-degrees, degrees);
            float pitch = Random.Range(-degrees, degrees);
            return Quaternion.Euler(pitch, yaw, 0) * dir;
        }

        void SpawnTracer(Vector3 a, Vector3 b)
        {
            if (tracerPrefab == null) return;
            var t = Instantiate(tracerPrefab);
            t.positionCount = 2;
            t.SetPosition(0, a);
            t.SetPosition(1, b);
            Destroy(t.gameObject, 0.05f);
        }
    }
}
