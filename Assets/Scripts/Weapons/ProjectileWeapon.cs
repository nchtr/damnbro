using UnityEngine;

namespace Damnbro.Weapons
{
    public class ProjectileWeapon : WeaponBase
    {
        [Header("Projectile")]
        public Projectile projectilePrefab;
        public float muzzleVelocity = 35f;

        protected override void Fire()
        {
            if (projectilePrefab == null || muzzle == null) return;
            Vector3 dir = viewCamera != null ? viewCamera.transform.forward : muzzle.forward;
            var p = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(dir));
            p.Launch(dir * muzzleVelocity, gameObject);
        }
    }
}
