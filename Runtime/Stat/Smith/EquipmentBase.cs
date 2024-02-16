using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkNaku.Stat.Smith
{
    [Serializable]
    public class EquipmentBase<T1, T2, T3> // T1 : 파츠 타입, T2 : 등급 타입, T3 : 스탯 타입
    {
        [SerializeField] private T1 _part;
        [SerializeField] private T2 _grade;
        [SerializeField] private int _level;
        [SerializeField] private List<KeyAndModifier<T3>> _defaultModifiers;
        [SerializeField] private List<KeyAndModifier<T3>> _extraModifiers;

        public T1 Part => _part;
        public T2 Grade => _grade;
        public int Level => _level;
        public IReadOnlyList<KeyAndModifier<T3>> DefaultModifiers => _defaultModifiers;
        public IReadOnlyList<KeyAndModifier<T3>> ExtraModifiers => _extraModifiers;

        public EquipmentBase(T1 part, T2 grade, int level, List<KeyAndModifier<T3>> defaultModifiers,
            List<KeyAndModifier<T3>> extraModifiers)
        {
            defaultModifiers ??= Enumerable.Empty<KeyAndModifier<T3>>().ToList();
            extraModifiers ??= Enumerable.Empty<KeyAndModifier<T3>>().ToList();

            _part = part;
            _grade = grade;
            _level = level;
            _defaultModifiers = defaultModifiers;
            _extraModifiers = extraModifiers;
        }

        public bool Contains(T3 key)
        {
            for (int i = 0; i < _defaultModifiers.Count; i++)
            {
                if (_defaultModifiers[i].Key.Equals(key)) return true;
            }

            for (int i = 0; i < _extraModifiers.Count; i++)
            {
                if (_extraModifiers[i].Key.Equals(key)) return true;
            }

            return false;
        }
    }
}