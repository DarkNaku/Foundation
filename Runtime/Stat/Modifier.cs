using System;
using UnityEngine;

namespace DarkNaku.Stat
{
    [Serializable]
    public class Modifier
    {
        [SerializeField] private ModifierType _type;
        [SerializeField] private float _value;
        [SerializeField] private bool _isTemporary;

        public ModifierType Type => _type;
        public float Value => _value;
        public bool IsTemporary => _isTemporary;
        public object Source { get; set; }
        public string ID { get; set; }

        public Modifier(ModifierType type, float value, bool isTemporary = false, object source = null, string id = "")
        {
            _type = type;
            _value = value;
            _isTemporary = isTemporary;
            Source = source;
            ID = id;
        }

        public override string ToString() => $"Type : {_type}, Value : {_value}, IsTemporary : {_isTemporary}, Source : {nameof(Source)}, ID : {ID}";
    }
}