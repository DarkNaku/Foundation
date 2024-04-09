using System;
using UnityEngine;

namespace DarkNaku.Stat.Smith
{
    [Serializable]
    public class KeyAndValue<T>
    {
        [SerializeField] private T _key;
        [SerializeField] private Modifier _value;
        
        public T Key => _key;
        public Modifier Value => _value;

        public KeyAndValue(T key, Modifier modifier)
        {
            _key = key;
            _value = modifier;
        }
        
        public override string ToString() => $"{_key} - {_value}";
    }
}