using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Stat
{
    public class ModifierInfo
    {
        public string StatName { get; }
        public ModifierType Type { get; }
        public float Value { get; }

        public ModifierInfo(string statName, ModifierType modifierType, float value)
        {
            StatName = statName;
            Type = modifierType;
            Value = value;
        }
    }
}