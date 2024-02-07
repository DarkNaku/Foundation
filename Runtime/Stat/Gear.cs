using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Stat
{
    [Serializable]
    public class KeyAndModifier<T>
    {
        [SerializeField] private T _key;
        [SerializeField] private Modifier _gearModifier;
        
        public T Key => _key;
        public Modifier GearModifier => _gearModifier;

        public KeyAndModifier(T key, Modifier modifier)
        {
            _key = key;
            _gearModifier = modifier;
        }
        
        public override string ToString() => $"{_key} - {_gearModifier}";
    }
    
    [Serializable]
    public class Gear<T>
    {
        [SerializeField] private List<KeyAndModifier<T>> _modifiers;
        
        private CharacterStats<T> _characterStats;

        public Gear(params KeyAndModifier<T>[] modifiers)
        {
            if (modifiers == null) return;

            _modifiers = new(modifiers);
        }

        public void Equip(CharacterStats<T> characterStats)
        {
            Unequip();
            
            _characterStats = characterStats;

            if (_characterStats == null || _modifiers == null) return;

            for (int i = 0; i < _modifiers.Count; i++)
            {
                var key = _modifiers[i].Key;
                var modifier = _modifiers[i].GearModifier;
                
                if (_characterStats.All.ContainsKey(key) == false) continue;

                modifier.Source = this;

                characterStats.AddModifier(key, modifier);
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