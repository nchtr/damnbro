using UnityEngine;

namespace Damnbro.Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Identity")]
        public string weaponName = "Weapon";
        public int slot = 0;

        [Header("Firing")]
        public float fireRate = 4f;
        public float altFireRate = 1f;
        public int magazineSize = 0;
        public float reloadTime = 1.2f;

        [Header("Refs")]
        public Camera viewCamera;
        public Transform muzzle;
        public LayerMask hitMask = ~0;

        protected float _nextFireTime;
        protected float _nextAltFireTime;
        protected int _ammoInMag;
        protected bool _isReloading;

        public int AmmoInMag => _ammoInMag;
        public bool IsReloading => _isReloading;

        protected virtual void Awake()
        {
            if (magazineSize > 0) _ammoInMag = magazineSize;
        }

        public virtual void OnEquip() => gameObject.SetActive(true);
        public virtual void OnUnequip() => gameObject.SetActive(false);

        public void TryFire()
        {
            if (_isReloading) return;
            if (Time.time < _nextFireTime) return;
            if (magazineSize > 0 && _ammoInMag <= 0) { TryReload(); return; }
            _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, fireRate);
            if (magazineSize > 0) _ammoInMag--;
            Fire();
        }

        public void TryAltFire()
        {
            if (_isReloading) return;
            if (Time.time < _nextAltFireTime) return;
            _nextAltFireTime = Time.time + 1f / Mathf.Max(0.01f, altFireRate);
            AltFire();
        }

        public void TryReload()
        {
            if (_isReloading || magazineSize <= 0 || _ammoInMag >= magazineSize) return;
            _isReloading = true;
            Invoke(nameof(FinishReload), reloadTime);
        }

        void FinishReload()
        {
            _ammoInMag = magazineSize;
            _isReloading = false;
        }

        protected abstract void Fire();
        protected virtual void AltFire() { }
    }
}
