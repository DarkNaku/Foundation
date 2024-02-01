using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DarkNaku.Stat
{
    [Serializable]
    public class Stat
    {
        public delegate float CalculateMethod(IReadOnlyDictionary<ModifierType, IReadOnlyList<Modifier>> modifiers);

        public float InitialValue => _initialValue;

        public float BaseValue 
        { 
            get => _baseValue;
            set => _baseValue = value;
        }

        public float Value
        {
            get
            {
                if (_isDirty || _lastBaseValue != BaseValue)
                {
                    _lastBaseValue = BaseValue;
                    _value = CalculateFinalValue(true);
                    _isDirty = false;
                }

                return _value;
            }
        }

        public float PermanentValue
        {
            get
            {
                if (_isDirtyPermanent || _lastBaseValue != BaseValue)
                {
                    _lastBaseValue = BaseValue;
                    _permanentValue = CalculateFinalValue(false);
                    _isDirtyPermanent = false;
                }

                return _permanentValue;
            }
        }

        public string Name { get; }

        public UnityEvent<Stat> OnChangeValue { get; } = new();
        public CalculateMethod CustomCalculateMethod { get; set; }

        [SerializeField] private float _initialValue;
        [SerializeField] private float _baseValue;

        private bool _isDirty = true;
        private bool _isDirtyPermanent = true;
        private float _lastBaseValue;
        private float _value;
        private float _permanentValue;

        private readonly Dictionary<ModifierType, List<Modifier>> _modifiers;

        public Stat()
        {
            _modifiers = new Dictionary<ModifierType, List<Modifier>>
            {
                { ModifierType.Sum, new List<Modifier>() },
                { ModifierType.PercentAdd, new List<Modifier>() },
                { ModifierType.PercentMultiply, new List<Modifier>() }
            };
        }

        public Stat(float initialValue, string name = "") : this()
        {
            _initialValue = initialValue;
            _baseValue = _initialValue;
            Name = name;
        }

        public void AddModifier(Modifier modifier)
        {
            if (_modifiers.ContainsKey(modifier.Type) == false)
            {
                _modifiers.Add(modifier.Type, new List<Modifier>());
            }

            _modifiers[modifier.Type].Add(modifier);
            _isDirty = true;
            OnChangeValue.Invoke(this);
        }

        public void RemoveModifier(Modifier modifier)
        {
            if (_modifiers.ContainsKey(modifier.Type))
            {
                if (_modifiers[modifier.Type].Remove(modifier))
                {
                    _isDirty = true;
                    OnChangeValue.Invoke(this);
                }
            }
        }

        public void RemoveModifiersFromSource(object source)
        {
            if (source == null) return;

            int numberRemoved = 0;

            foreach (var modifiers in _modifiers.Values)
            {
                numberRemoved += modifiers.RemoveAll(item => item.Source == source);
            }

            if (numberRemoved > 0)
            {
                _isDirty = true;
                OnChangeValue.Invoke(this);
            }
        }

        protected virtual float CalculateFinalValue(bool withTemporary)
        {
            if (CustomCalculateMethod == null) 
            {
                float finalValue = BaseValue;

                finalValue = CalculatePlus(finalValue, withTemporary);
                finalValue = CalculatePercentAdd(finalValue, withTemporary);
                finalValue = CalculatePercentMultiply(finalValue, withTemporary);

                return finalValue;
            }
            else
            {
                var modifiers = new Dictionary<ModifierType, IReadOnlyList<Modifier>>();

                foreach (var item in _modifiers)
                {
                    modifiers.Add(item.Key, item.Value);
                }

                return CustomCalculateMethod(modifiers);
            }
        }

        protected float CalculatePlus(float baseValue, bool withTemporary)
        {
            var modifiers = _modifiers[ModifierType.Sum];

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (withTemporary)
                {
                    baseValue += modifiers[i].Value;
                }
                else
                {
                    if (modifiers[i].IsTemporary == false)
                    {
                        baseValue += modifiers[i].Value;
                    }
                }
            }

            return baseValue;
        }

        protected float CalculatePercentAdd(float baseValue, bool withTemporary)
        {
            var modifiers = _modifiers[ModifierType.PercentAdd];

            float percentAddSum = 0f;

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (withTemporary)
                {
                    percentAddSum += modifiers[i].Value;
                }
                else
                {
                    if (modifiers[i].IsTemporary == false)
                    {
                        percentAddSum += modifiers[i].Value;
                    }
                }
            }

            return baseValue * (1f + percentAddSum);
        }

        protected float CalculatePercentMultiply(float baseValue, bool withTemporary)
        {
            var modifiers = _modifiers[ModifierType.PercentMultiply];

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (withTemporary)
                {
                    baseValue *= (1f + modifiers[i].Value);
                }
                else
                {
                    if (modifiers[i].IsTemporary == false)
                    {
                        baseValue *= (1f + modifiers[i].Value);
                    }
                }
            }

            return baseValue;
        }
    }
}