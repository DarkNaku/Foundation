using System;
using System.Collections.Generic;

namespace DarkNaku.Foundation {
    /// <summary>
    /// IReadOnlyList의 확장 메서드
    /// </summary>
    public static class IReadOnlyListEx {
        /// <summary>
        /// 아이템의 인덱스를 반환한다. 아이템이 존재하지 않을 경우 -1을 반환한다.
        /// </summary>
        /// <param name="item">아이템</param>
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item) {
            for (int i = 0; i < list.Count; i++) {
                if (EqualityComparer<T>.Default.Equals(list[i], item)) return i;
            }

            return -1;
        }

        /// <summary>
        /// 조건에 맞는 아이템의 인덱스를 반환한다. 아이템이 존재하지 않을 경우 -1을 반환한다.
        /// </summary>
        /// <param name="match">조건</param>
        public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> match) {
            for (int i = 0; i < list.Count; i++) {
                if (match(list[i])) return i;
            }

            return -1;
        }

        /// <summary>
        /// 아이템이 리스트에 존재하는지 확인한다.
        /// </summary>
        /// <param name="item">아이템</param>
        public static bool Contains<T>(this IReadOnlyList<T> list, T item) => list.IndexOf(item) >= 0;
    }
}