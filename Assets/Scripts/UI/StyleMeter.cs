using System;
using UnityEngine;

namespace Damnbro.UI
{
    public class StyleMeter : MonoBehaviour
    {
        public enum Rank { Destructive, Chaotic, Brutal, Anarchic, Supreme, SSadistic, SSShitstorm }

        [Header("Tuning")]
        public float decayPerSecond = 6f;
        public float maxScore = 600f;
        public float[] rankThresholds = { 0, 80, 160, 240, 340, 460, 580 };

        public float Score { get; private set; }
        public Rank CurrentRank { get; private set; }
        public event Action<Rank> OnRankChanged;
        public event Action<float, float> OnScoreChanged;

        public void AddPoints(float amount)
        {
            if (amount <= 0f) return;
            Score = Mathf.Min(maxScore, Score + amount);
            OnScoreChanged?.Invoke(Score, maxScore);
            RecomputeRank();
        }

        void Update()
        {
            if (Score <= 0f) return;
            Score = Mathf.Max(0f, Score - decayPerSecond * Time.deltaTime);
            OnScoreChanged?.Invoke(Score, maxScore);
            RecomputeRank();
        }

        void RecomputeRank()
        {
            Rank next = Rank.Destructive;
            for (int i = 0; i < rankThresholds.Length; i++)
                if (Score >= rankThresholds[i]) next = (Rank)i;
            if (next != CurrentRank)
            {
                CurrentRank = next;
                OnRankChanged?.Invoke(CurrentRank);
            }
        }
    }
}
