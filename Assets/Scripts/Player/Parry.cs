using UnityEngine;
using Damnbro.Weapons;
using Damnbro.UI;

namespace Damnbro.Player
{
    public class Parry : MonoBehaviour
    {
        [Header("Refs")]
        public Camera viewCamera;
        public StyleMeter styleMeter;

        [Header("Tuning")]
        public float range = 3f;
        public float radius = 1.2f;
        public float cooldown = 0.45f;
        public float meleeDamage = 20f;
        public float parryWindow = 0.18f;
        public float parryStylePoints = 60f;
        public LayerMask hitMask = ~0;

        [Header("FX")]
        public GameObject parryFxPrefab;

        public bool IsParrying { get; private set; }
        float _nextStrikeTime;
        float _parryEndsAt;

        void Update()
        {
            if (IsParrying && Time.time >= _parryEndsAt) IsParrying = false;
        }

        public void TryStrike()
        {
            if (Time.time < _nextStrikeTime || viewCamera == null) return;
            _nextStrikeTime = Time.time + cooldown;
            _parryEndsAt = Time.time + parryWindow;
            IsParrying = true;

            Vector3 origin = viewCamera.transform.position;
            Vector3 dir = viewCamera.transform.forward;
            bool reflected = false;

            var hits = Physics.SphereCastAll(origin, radius, dir, range, hitMask, QueryTriggerInteraction.Collide);
            foreach (var h in hits)
            {
                var proj = h.collider.GetComponentInParent<Projectile>();
                if (proj != null)
                {
                    proj.Reflect(gameObject);
                    reflected = true;
                    if (parryFxPrefab != null) Instantiate(parryFxPrefab, h.point, Quaternion.LookRotation(-dir));
                    continue;
                }
                var hp = h.collider.GetComponentInParent<HealthSystem>();
                if (hp != null && hp.gameObject != gameObject) hp.ApplyDamage(meleeDamage, gameObject);
            }

            if (reflected && styleMeter != null) styleMeter.AddPoints(parryStylePoints);
        }
    }
}
