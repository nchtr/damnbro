using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Damnbro.Player;
using Damnbro.Weapons;

namespace Damnbro.UI
{
    public class HUD : MonoBehaviour
    {
        public HealthSystem health;
        public PlayerController player;
        public PlayerInteraction interaction;
        public WeaponManager weapons;
        public StyleMeter style;

        [Header("UI Refs")]
        public Slider healthBar;
        public TMP_Text healthLabel;
        public TMP_Text ammoLabel;
        public TMP_Text dashLabel;
        public TMP_Text promptLabel;
        public TMP_Text styleRankLabel;
        public Slider styleBar;

        void OnEnable()
        {
            if (health != null) health.OnChanged += UpdateHealth;
            if (style != null)
            {
                style.OnRankChanged += r => { if (styleRankLabel != null) styleRankLabel.text = r.ToString(); };
                style.OnScoreChanged += (s, m) => { if (styleBar != null) styleBar.value = s / m; };
            }
        }

        void OnDisable()
        {
            if (health != null) health.OnChanged -= UpdateHealth;
        }

        void Start()
        {
            if (health != null) UpdateHealth(health.Current, health.maxHealth);
        }

        void Update()
        {
            if (ammoLabel != null && weapons != null && weapons.Current != null)
            {
                var w = weapons.Current;
                ammoLabel.text = w.magazineSize > 0 ? $"{w.AmmoInMag}/{w.magazineSize}" : "∞";
            }
            if (dashLabel != null && player != null) dashLabel.text = new string('●', player.DashChargesRemaining);
            if (promptLabel != null && interaction != null)
            {
                promptLabel.text = interaction.CurrentTarget != null ? interaction.CurrentTarget.Prompt : "";
            }
        }

        void UpdateHealth(float current, float max)
        {
            if (healthBar != null) healthBar.value = current / max;
            if (healthLabel != null) healthLabel.text = Mathf.CeilToInt(current).ToString();
        }
    }
}
