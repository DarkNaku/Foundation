using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DarkNaku.Stat
{
    public interface IStat
    {
        float InitialValue { get; }
        float BaseValue { get; set; }
        float Value { get; }    
        float PermanentValue { get; }
        string Name { get; }
        IReadOnlyList<Modifier> GetModifiers(ModifierType modifierType);
    }
    
    public class Stat<T> : IStat
    {
        public float InitialValue => _initialValue;

        public float BaseValue 
        { 
            get => _parent?.Value ?? _baseValue;
            set
            {
                if (_parent == null)
                {
                    _baseValue = value;
                }
                else
                {
                    Debug.LogError("[Stat] Can't set base value to child stat.");
                }
            }
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

        public string Name => Key.ToString();

        public T Key => (_parent == null) ? _key : _parent.Key;

        public UnityEvent<Stat<T>> OnChangeValue { get; } = new();
        public UnityEvent<Stat<T>> OnChangeValuePermanent { get; } = new();

        private float _initialValue;
        private float _baseValue;
        private Stat<T> _parent;

        private T _key;
        private bool _isDirty = true;
        private bool _isDirtyPermanent = true;
        private float _lastBaseValue;
        private float _lastPermanentBaseValue;
        private float _value;
        private float _permanentValue;

        private readonly Dictionary<ModifierType, List<Modifier>> _modifiers;

        public Stat()
        {
            _modifiers = new Dictionary<ModifierType, List<Modifier>>
            {
                { ModifierType.Add, new List<Modifier>() },
                { ModifierType.Percent, new List<Modifier>() },
                { ModifierType.Multiply, new List<Modifier>() }
            };
        }

        public Stat(float initialValue, T key = default) : this()
        {
            _initialValue = initialValue;
            _baseValue = _initialValue;
            _key = key;
        }
        
        public Stat(Stat<T> parent, T key = default) : this()
        {
            _parent = parent;
            
            _parent.OnChangeValue.AddListener((stat) =>
            {
                _isDirty = true;
                OnChangeValue.Invoke(this);
            });
            
            _parent.OnChangeValuePermanent.AddListener((stat) =>
            {
                _isDirtyPermanent = true;
                OnChangeValuePermanent.Invoke(this);
            });
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
            
            if (modifier.IsTemporary == false)
            {
                _isDirtyPermanent = true;
                OnChangeValuePermanent.Invoke(this);
            }
        }

        public void RemoveModifier(Modifier modifier)
        {
            if (_modifiers.ContainsKey(modifier.Type))
            {
                if (_modifiers[modifier.Type].Remove(modifier))
                {
                    _isDirty = true;
                    
                    OnChangeValue.Invoke(this);
                    
                    if (modifier.IsTemporary == false)
                    {
                        _isDirtyPermanent = true;
                        OnChangeValuePermanent.Invoke(this);
                    }
                }
            }
        }

        public void RemoveModifiersFromID(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            int numberRemoved = 0;
            var permanentModifierRemoved = false;
            
            foreach (var modifiers in _modifiers.Values)
            {
                for (int i = modifiers.Count - 1; i >= 0; i--)
                {
                    if (modifiers[i].ID == id)
                    {
                        if (modifiers[i].IsTemporary == false)
                        {
                            _isDirtyPermanent = true;
                            permanentModifierRemoved = true;
                        }
                        
                        modifiers.RemoveAt(i);
                        numberRemoved++;
                    }
                }
            }

            if (numberRemoved > 0)
            {
                _isDirty = true;
                OnChangeValue.Invoke(this);

                if (permanentModifierRemoved)
                {
                    OnChangeValuePermanent.Invoke(this);
                }
            }
        }

        public void RemoveModifiersFromSource(object source)
        {
            if (source == null) return;

            int numberRemoved = 0;
            var permanentModifierRemoved = false;

            foreach (var modifiers in _modifiers.Values)
            {
                for (int i = modifiers.Count - 1; i >= 0; i--)
                {
                    if (modifiers[i].Source == source)
                    {
                        if (modifiers[i].IsTemporary == false)
                        {
                            _isDirtyPermanent = true;
                            permanentModifierRemoved = true;
                        }
                        
                        modifiers.RemoveAt(i);
                        numberRemoved++;
                    }
                }
            }

            if (numberRemoved > 0)
            {
                _isDirty = true;
                OnChangeValue.Invoke(this);

                if (permanentModifierRemoved)
                {
                    OnChangeValuePermanent.Invoke(this);
                }
            }
        }
        
        public void RemoveTemporaryModifiers()
        {
            int numberRemoved = 0;

            foreach (var modifiers in _modifiers.Values)
            {
                numberRemoved += modifiers.RemoveAll(item => item.IsTemporary);
            }

            if (numberRemoved > 0)
            {
                _isDirty = true;
                OnChangeValue.Invoke(this);
            }
        }
        
        public IReadOnlyList<Modifier> GetModifiers(ModifierType modifierType)
        {
            if (_modifiers.TryGetValue(modifierType, out var modifiers))
            {
                return modifiers;
            }

            return Enumerable.Empty<Modifier>().ToList();
        }

        protected virtual float CalculateFinalValue(bool includeTemporary)
        {
            float finalValue = BaseValue;

            finalValue = CalculateSum(finalValue, includeTemporary);
            finalValue = CalculatePercent(finalValue, includeTemporary);
            finalValue = CalculateMultiply(finalValue, includeTemporary);

            return finalValue;
        }

        protected float CalculateSum(float baseValue, bool includeTemporary)
        {
            var modifiers = _modifiers[ModifierType.Add];

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (includeTemporary)
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

        protected float CalculatePercent(float baseValue, bool includeTemporary)
        {
            var modifiers = _modifiers[ModifierType.Percent];

            float percentAddSum = 0f;

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (includeTemporary)
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

        protected float CalculateMultiply(float baseValue, bool includeTemporary)
        {
            var modifiers = _modifiers[ModifierType.Multiply];

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (includeTemporary)
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