using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Damnbro.Core
{
    public class RuntimeNavMeshBaker : MonoBehaviour
    {
        public Vector3 boundsCenter = Vector3.zero;
        public Vector3 boundsSize = new(80f, 12f, 80f);
        public int areaMask = ~0;
        public bool bakeOnAwake = true;
        public bool rebuildPeriodically = false;
        public float rebuildInterval = 5f;

        NavMeshData _data;
        NavMeshDataInstance _instance;
        float _lastBake;

        void Awake()
        {
            if (bakeOnAwake) Bake();
        }

        void Update()
        {
            if (!rebuildPeriodically) return;
            if (Time.time - _lastBake >= rebuildInterval) Bake();
        }

        public void Bake()
        {
            var bounds = new Bounds(boundsCenter, boundsSize);
            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();
            NavMeshBuilder.CollectSources(bounds, areaMask, NavMeshCollectGeometry.PhysicsColliders, 0, markups, sources);
            var settings = NavMesh.GetSettingsByID(0);
            if (_data == null) _data = new NavMeshData();
            NavMeshBuilder.UpdateNavMeshData(_data, settings, sources, bounds);
            if (_instance.valid) _instance.Remove();
            _instance = NavMesh.AddNavMeshData(_data);
            _lastBake = Time.time;
        }

        void OnDestroy()
        {
            if (_instance.valid) _instance.Remove();
        }
    }
}
