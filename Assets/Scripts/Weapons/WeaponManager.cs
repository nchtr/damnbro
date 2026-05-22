using System.Collections.Generic;
using UnityEngine;

namespace Damnbro.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        public List<WeaponBase> weapons = new();
        public int startingIndex = 0;

        int _currentIndex = -1;
        public WeaponBase Current => (_currentIndex >= 0 && _currentIndex < weapons.Count) ? weapons[_currentIndex] : null;

        void Start()
        {
            for (int i = 0; i < weapons.Count; i++)
                if (weapons[i] != null) weapons[i].gameObject.SetActive(false);
            if (weapons.Count > 0) Equip(Mathf.Clamp(startingIndex, 0, weapons.Count - 1));
        }

        public void Equip(int index)
        {
            if (index < 0 || index >= weapons.Count) return;
            if (index == _currentIndex) return;
            Current?.OnUnequip();
            _currentIndex = index;
            Current?.OnEquip();
        }

        public void SelectSlot(int slot)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i] != null && weapons[i].slot == slot)
                {
                    Equip(i);
                    return;
                }
            }
        }

        public void CycleNext()
        {
            if (weapons.Count == 0) return;
            Equip((_currentIndex + 1) % weapons.Count);
        }

        public void CyclePrev()
        {
            if (weapons.Count == 0) return;
            Equip((_currentIndex - 1 + weapons.Count) % weapons.Count);
        }

        public void TriggerFire() => Current?.TryFire();
        public void TriggerAltFire() => Current?.TryAltFire();
        public void TriggerReload() => Current?.TryReload();
    }
}
