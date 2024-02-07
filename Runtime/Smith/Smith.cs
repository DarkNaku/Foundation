using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkNaku.Stat;

// T1 : 파츠 타입
// T2 : 등급 타입
// T3 : 스탯 타입

public interface ISmithContainer<T1, T2, T3>
{
    IReadOnlyList<T1> GetPartTable();
    IReadOnlyList<T2> GetGradeTable();
    IReadOnlyList<float> GetProbabilityTable();
    List<KeyAndModifier<T3>> GetDefaultModifiers(T1 part, T2 grade);
    List<KeyAndModifier<T3>> GetExtraModifiers(T1 part, T2 grade);

    EquipmentBase<T1, T2, T3> CreateEquipment(T1 part, T2 grade, int level,
        List<KeyAndModifier<T3>> defaultModifiers, List<KeyAndModifier<T3>> extraModifiers);
}

public class Smith<T1, T2, T3>
{
    private ISmithContainer<T1, T2, T3> _container;
    
    public Smith(ISmithContainer<T1, T2, T3> container)
    {
        _container = container;
    }
    
    public T Create<T>(int minLevel, int maxLevel) where T : EquipmentBase<T1, T2, T3>
    {
        var part = GetPart();
        var grade = GetGrade();
        var level = Random.Range(minLevel, maxLevel + 1);
        var defaultModifiers = _container.GetDefaultModifiers(part, grade);
        var extraModifiers = _container.GetExtraModifiers(part, grade);

        return _container.CreateEquipment(part, grade, level, defaultModifiers, extraModifiers) as T;
    }

    private T1 GetPart()
    {
        var parts = _container.GetPartTable();
        
        return parts[Random.Range(0, parts.Count)];   
    }

    private T2 GetGrade()
    {
        var probabilities = _container.GetProbabilityTable();
        var randomValue = Random.Range(0f, 1f);
        var sum = 0f;
        var gradeIndex = 0;

        for (int i = 0; i < probabilities.Count; i++)
        {
            sum += probabilities[i];
            
            if (randomValue <= 0f)
            {
                gradeIndex = i;
                break;
            }
        }

        return _container.GetGradeTable()[gradeIndex];
    }
}