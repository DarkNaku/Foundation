using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Stat
{
    public class ModifierInfo<T>
    {
        public T Key { get; }
        public ModifierType Type { get; }
        public float Value { get; }

        public ModifierInfo(T key, ModifierType modifierType, float value)
        {
            Key = key;
            Type = modifierType;
            Value = value;
        }
    }
}