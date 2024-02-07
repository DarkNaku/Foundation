using System;
using System.Collections;
using System.Collections.Generic;
using DarkNaku.Stat;
using UnityEngine;

[Serializable]
public class EquipmentBase<T1, T2, T3>
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

    public EquipmentBase(T1 part, T2 grade, int level, List<KeyAndModifier<T3>> defaultModifiers, List<KeyAndModifier<T3>> extraModifiers)
    {
        _part = part;
        _grade = grade;
        _level = level;
        _defaultModifiers = defaultModifiers;
        _extraModifiers = extraModifiers;
    }
}