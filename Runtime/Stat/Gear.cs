using System.Collections.Generic;

namespace DarkNaku.Stat
{
    public class Gear<T>
    {
        private readonly HashSet<ModifierInfo<T>> _modifierInfos;
        private CharacterStats<T> _characterStats;

        public Gear(params ModifierInfo<T>[] modifierInfos)
        {
            if (modifierInfos == null) return;

            _modifierInfos = new();

            for (int i = 0; i < modifierInfos.Length; i++)
            {
                _modifierInfos.Add(modifierInfos[i]);
            }
        }

        public void Equip(CharacterStats<T> characterStats)
        {
            Unequip();
            
            _characterStats = characterStats;

            foreach (var info in _modifierInfos)
            {
                if (_characterStats.All.ContainsKey(info.Key) == false) continue;

                characterStats.AddModifier(info.Key, new Modifier(info.Type, info.Value, false, this));
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