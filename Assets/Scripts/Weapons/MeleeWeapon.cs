using UnityEngine;
using Damnbro.Player;

namespace Damnbro.Weapons
{
    public class MeleeWeapon : WeaponBase
    {
        [Header("Melee")]
        public float damage = 50f;
        public float range = 2.5f;
        public float radius = 0.9f;
        public float impulse = 8f;
        public GameObject impactPrefab;

        [Header("Swing FX")]
        public float swingDuration = 0.18f;
        public Vector3 swingEulerOffset = new(0, 0, -45f);

        Vector3 _restEuler;
        float _swingEndTime;

        protected override void Awake()
        {
            base.Awake();
            _restEuler = transform.localEulerAngles;
        }

        protected override void Fire()
        {
            if (viewCamera == null) return;
            Vector3 origin = viewCamera.transform.position;
            Vector3 dir = viewCamera.transform.forward;
            var hits = Physics.SphereCastAll(origin, radius, dir, range, hitMask, QueryTriggerInteraction.Ignore);
            foreach (var h in hits)
            {
                var hp = h.collider.GetComponentInParent<HealthSystem>();
                if (hp != null && hp.gameObject != gameObject && !h.collider.transform.IsChildOf(transform.root))
                    hp.ApplyDamage(damage, gameObject);
                if (h.collider.attachedRigidbody != null)
                    h.collider.attachedRigidbody.AddForceAtPosition(dir * impulse, h.point, ForceMode.Impulse);
                if (impactPrefab != null)
                    Instantiate(impactPrefab, h.point, Quaternion.LookRotation(-dir));
            }
            _swingEndTime = Time.time + swingDuration;
        }

        void LateUpdate()
        {
            if (_swingEndTime > Time.time)
            {
                float t = 1f - Mathf.Clamp01((_swingEndTime - Time.time) / swingDuration);
                float curve = Mathf.Sin(t * Mathf.PI);
                transform.localEulerAngles = _restEuler + swingEulerOffset * curve;
            }
            else
            {
                transform.localEulerAngles = _restEuler;
            }
        }
    }
}
