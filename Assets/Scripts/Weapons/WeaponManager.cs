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
            if (weapons.Count == 0) return;
            int target = _currentIndex >= 0 ? _currentIndex : Mathf.Clamp(startingIndex, 0, weapons.Count - 1);
            Equip(target);
        }

        public void Equip(int index)
        {
            if (index < 0 || index >= weapons.Count) return;
            bool changed = index != _currentIndex;
            if (changed) Current?.OnUnequip();
            _currentIndex = index;
            if (changed) Current?.OnEquip();
            SyncActive();
        }

        void SyncActive()
        {
            for (int i = 0; i < weapons.Count; i++)
                if (weapons[i] != null) weapons[i].gameObject.SetActive(i == _currentIndex);
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
