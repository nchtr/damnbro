using UnityEngine;
using UnityEngine.AI;
using Damnbro.Player;

namespace Damnbro.Enemies
{
    [RequireComponent(typeof(HealthSystem))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Targeting")]
        public string playerTag = "Player";
        public float sightRange = 30f;
        public float attackRange = 2f;
        public float attackCooldown = 1.2f;

        [Header("Refs")]
        public NavMeshAgent agent;

        protected Transform _target;
        protected HealthSystem _health;
        protected float _nextAttackTime;

        protected virtual void Awake()
        {
            _health = GetComponent<HealthSystem>();
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            _health.OnDied += OnDeath;
        }

        protected virtual void Start()
        {
            var p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) _target = p.transform;
        }

        protected virtual void Update()
        {
            if (_health.IsDead || _target == null) return;
            float dist = Vector3.Distance(transform.position, _target.position);
            if (dist > sightRange) { if (agent != null && agent.isOnNavMesh) agent.ResetPath(); return; }

            if (agent != null && agent.isOnNavMesh) agent.SetDestination(_target.position);

            if (dist <= attackRange && Time.time >= _nextAttackTime)
            {
                _nextAttackTime = Time.time + attackCooldown;
                Attack();
            }
        }

        protected abstract void Attack();

        protected virtual void OnDeath(GameObject source)
        {
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            Destroy(gameObject, 2f);
        }
    }
}
