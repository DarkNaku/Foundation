using System.Linq;
using UnityEngine;

namespace DarkNaku.Stat.Smith
{
    public class Smith<T1, T2, T3> // T1 : 파츠 타입, T2 : 등급 타입, T3 : 스탯 타입
    {
        private ISmithContainer<T1, T2, T3> _container;

        public Smith(ISmithContainer<T1, T2, T3> container)
        {
            _container = container;
        }

        public T Create<T>() where T : EquipmentBase<T1, T2, T3>
        {
            var part = GetPart();
            var grade = GetGrade();
            var level = _container.GetLevel();
            var defaultModifiers = _container.GetDefaultModifiers(part, grade, level);
            var extraModifiers = _container.GetExtraModifiers(part, grade, level);

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
            var randomValue = Random.Range(0f, probabilities.Sum());
            var sum = 0f;
            var gradeIndex = 0;

            for (int i = 0; i < probabilities.Count; i++)
            {
                sum += probabilities[i];

                if (randomValue <= sum)
                {
                    gradeIndex = i;
                    break;
                }
            }

            return _container.GetGradeTable()[gradeIndex];
        }
    }
}