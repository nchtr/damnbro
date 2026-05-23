using UnityEngine;
using UnityEngine.AI;
using Damnbro.Enemies;
using Damnbro.Player;

namespace Damnbro.Core
{
    public static class EnemyFactory
    {
        public static GameObject BuildMelee(Vector3 floorPosition)
        {
            var go = MakeCapsule(floorPosition, new Color(0.85f, 0.2f, 0.2f));
            go.name = "EnemyMelee_Spawned";
            var agent = AddAgent(go, 5f);
            var hp = go.AddComponent<HealthSystem>();
            hp.maxHealth = 40f;
            var ai = go.AddComponent<EnemyMelee>();
            ai.agent = agent;
            ai.meleeDamage = 15f;
            ai.attackCooldown = 1.2f;
            ai.attackRange = 2f;
            return go;
        }

        public static GameObject BuildRanged(Vector3 floorPosition)
        {
            var go = MakeCapsule(floorPosition, new Color(0.25f, 0.45f, 0.95f));
            go.name = "EnemyRanged_Spawned";
            var agent = AddAgent(go, 3.5f);
            var hp = go.AddComponent<HealthSystem>();
            hp.maxHealth = 30f;
            var ai = go.AddComponent<EnemyRanged>();
            ai.agent = agent;
            ai.attackCooldown = 1.8f;
            ai.preferredDistance = 14f;
            ai.projectileSpeed = 18f;

            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(go.transform, false);
            muzzle.localPosition = new Vector3(0, 1.2f, 0.6f);
            ai.muzzle = muzzle;
            return go;
        }

        static GameObject MakeCapsule(Vector3 floorPos, Color tint)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.transform.position = floorPos + Vector3.up;
            var rend = go.GetComponent<MeshRenderer>();
            if (rend != null)
            {
                var mat = new Material(rend.sharedMaterial);
                mat.color = tint;
                rend.material = mat;
            }
            var col = go.GetComponent<CapsuleCollider>();
            if (col != null) col.height = 2f;
            return go;
        }

        static NavMeshAgent AddAgent(GameObject go, float speed)
        {
            var agent = go.AddComponent<NavMeshAgent>();
            agent.height = 2f;
            agent.radius = 0.5f;
            agent.speed = speed;
            agent.angularSpeed = 720f;
            agent.acceleration = 24f;
            agent.stoppingDistance = 0.5f;
            return agent;
        }
    }
}
