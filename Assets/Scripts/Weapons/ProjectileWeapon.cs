using UnityEngine;

namespace Damnbro.Weapons
{
    public class ProjectileWeapon : WeaponBase
    {
        [Header("Projectile")]
        public Projectile projectilePrefab;
        public float muzzleVelocity = 35f;

        [Header("Fallback (used when projectilePrefab is null)")]
        public float fallbackDamage = 50f;
        public float fallbackExplosionRadius = 4f;
        public Color fallbackColor = new(1f, 0.85f, 0.2f);

        protected override void Fire()
        {
            if (muzzle == null) return;
            Vector3 dir = viewCamera != null ? viewCamera.transform.forward : muzzle.forward;
            Projectile p = projectilePrefab != null
                ? Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(dir))
                : BuildFallback(muzzle.position, dir);
            p.Launch(dir * muzzleVelocity, transform.root.gameObject);
        }

        Projectile BuildFallback(Vector3 pos, Vector3 dir)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = $"{weaponName}_Shot";
            go.transform.position = pos;
            go.transform.rotation = Quaternion.LookRotation(dir);
            go.transform.localScale = Vector3.one * 0.25f;
            var rend = go.GetComponent<MeshRenderer>();
            if (rend != null) rend.material.color = fallbackColor;
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            var proj = go.AddComponent<Projectile>();
            proj.damage = fallbackDamage;
            proj.explosionRadius = fallbackExplosionRadius;
            proj.hitMask = hitMask;
            return proj;
        }
    }
}

