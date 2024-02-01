using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Stat
{
    public class Gear
    {
        private readonly HashSet<ModifierInfo> _modifierInfos;
        private CharacterStats _characterStats;

        public Gear(params ModifierInfo[] modifierInfos)
        {
            if (modifierInfos == null) return;

            _modifierInfos = new();

            for (int i = 0; i < modifierInfos.Length; i++)
            {
                _modifierInfos.Add(modifierInfos[i]);
            }
        }

        public void Equip(CharacterStats characterStats)
        {
            Unequip();
            
            _characterStats = characterStats;

            foreach (var info in _modifierInfos)
            {
                if (string.IsNullOrEmpty(info.StatName)) continue;
                if (_characterStats.All.ContainsKey(info.StatName) == false) continue;

                characterStats.AddModifier(info.StatName, new Modifier(info.Type, info.Value, false, this));
            }
        }

        public void Unequip()
        {
            if (_characterStats == null) return;

            _characterStats.RemoveModifierFromSource(this);
            _characterStats = null;
        }
    }
}