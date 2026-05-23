using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Damnbro.Player;

namespace Damnbro.Core
{
    public class WaveSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class Wave
        {
            public string label = "Wave";
            public int meleeCount = 3;
            public int rangedCount = 1;
            public float spawnInterval = 0.6f;
            public float postWaveDelay = 5f;
        }

        [Header("Schedule")]
        public List<Wave> waves = new();
        public float startDelay = 3f;
        public bool loopFinalWave = true;

        [Header("Spawning")]
        public Transform[] spawnPoints;
        public float fallbackRadiusMin = 14f;
        public float fallbackRadiusMax = 22f;

        [Header("Optional prefab overrides")]
        public GameObject meleePrefab;
        public GameObject rangedPrefab;

        [Header("UI")]
        public TMP_Text waveLabel;

        public int CurrentWaveIndex { get; private set; } = -1;
        public int AliveEnemies { get; private set; }

        float _nextEventTime;
        int _spawnQueueMelee;
        int _spawnQueueRanged;
        bool _inWave;
        Transform _player;

        void Start()
        {
            if (waves.Count == 0)
            {
                waves.Add(new Wave { label = "Wave 1", meleeCount = 3, rangedCount = 0 });
                waves.Add(new Wave { label = "Wave 2", meleeCount = 4, rangedCount = 1 });
                waves.Add(new Wave { label = "Wave 3", meleeCount = 5, rangedCount = 2 });
            }
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) _player = p.transform;
            _nextEventTime = Time.time + startDelay;
            UpdateHud("Wave starts in...");
        }

        void Update()
        {
            if (!_inWave && Time.time >= _nextEventTime) StartNextWave();

            if (_inWave && (_spawnQueueMelee + _spawnQueueRanged) > 0 && Time.time >= _nextEventTime)
            {
                SpawnOne();
                _nextEventTime = Time.time + CurrentWave.spawnInterval;
            }

            if (_inWave && _spawnQueueMelee == 0 && _spawnQueueRanged == 0 && AliveEnemies <= 0)
            {
                _inWave = false;
                _nextEventTime = Time.time + CurrentWave.postWaveDelay;
                UpdateHud($"Wave {CurrentWaveIndex + 1} cleared");
            }
        }

        Wave CurrentWave => waves[Mathf.Clamp(CurrentWaveIndex, 0, waves.Count - 1)];

        void StartNextWave()
        {
            int next = CurrentWaveIndex + 1;
            if (next >= waves.Count)
            {
                if (!loopFinalWave) { UpdateHud("All waves cleared"); enabled = false; return; }
                next = waves.Count - 1;
            }
            CurrentWaveIndex = next;
            var w = CurrentWave;
            _spawnQueueMelee = w.meleeCount;
            _spawnQueueRanged = w.rangedCount;
            _inWave = true;
            _nextEventTime = Time.time;
            UpdateHud($"{w.label}  —  {w.meleeCount}M / {w.rangedCount}R");
        }

        void SpawnOne()
        {
            bool spawnMelee = _spawnQueueMelee > 0 && (_spawnQueueRanged == 0 || Random.value < 0.65f);
            if (_spawnQueueMelee == 0) spawnMelee = false;
            Vector3 pos = ChooseSpawnPoint();

            GameObject enemy;
            if (spawnMelee)
            {
                enemy = meleePrefab != null ? Instantiate(meleePrefab, pos + Vector3.up, Quaternion.identity)
                                            : EnemyFactory.BuildMelee(pos);
                _spawnQueueMelee--;
            }
            else
            {
                enemy = rangedPrefab != null ? Instantiate(rangedPrefab, pos + Vector3.up, Quaternion.identity)
                                             : EnemyFactory.BuildRanged(pos);
                _spawnQueueRanged--;
            }

            var hp = enemy.GetComponentInParent<HealthSystem>();
            if (hp != null) hp.OnDied += _ => AliveEnemies = Mathf.Max(0, AliveEnemies - 1);
            AliveEnemies++;
        }

        Vector3 ChooseSpawnPoint()
        {
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                var t = spawnPoints[Random.Range(0, spawnPoints.Length)];
                if (t != null) return t.position;
            }
            Vector3 center = _player != null ? _player.position : transform.position;
            Vector2 r = Random.insideUnitCircle.normalized * Random.Range(fallbackRadiusMin, fallbackRadiusMax);
            return new Vector3(center.x + r.x, 0f, center.z + r.y);
        }

        void UpdateHud(string text)
        {
            if (waveLabel != null) waveLabel.text = text;
        }
    }
}
