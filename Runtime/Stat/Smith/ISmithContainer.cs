using System.Collections.Generic;

namespace DarkNaku.Stat.Smith
{
    public interface ISmithContainer<T1, T2, T3> // T1 : 파츠 타입, T2 : 등급 타입, T3 : 스탯 타입
    {
        int GetLevel();
        IReadOnlyList<T1> GetPartTable();
        IReadOnlyList<T2> GetGradeTable();
        IReadOnlyList<float> GetProbabilityTable();
        List<KeyAndValue<T3>> GetDefaultModifiers(T1 part, T2 grade, int level);
        List<KeyAndValue<T3>> GetExtraModifiers(T1 part, T2 grade, int level);

        EquipmentBase<T1, T2, T3> CreateEquipment(T1 part, T2 grade, int level,
            List<KeyAndValue<T3>> defaultModifiers, List<KeyAndValue<T3>> extraModifiers);
    }
}