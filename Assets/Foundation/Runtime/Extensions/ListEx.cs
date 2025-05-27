using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Foundation {
    /// <summary>
    /// List의 확장 메서드
    /// </summary>
    public static class ListEx {
        /// <summary>
        /// 리스트 아이템들을 랜덤하게 섞는다.
        /// </summary>
        /// <param name="list">리스트</param>
        public static void Shuffle<T>(this IList<T> list) {
            for (int i = list.Count - 1; i > 0; i--) {
                int j = Random.Range(0, i + 1);

                if (i != j) {
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
        }

        /// <summary>
        /// 리스트에서 랜덤하게 아이템을 원하는 갯수만큼 선택한다.
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="selectCount">갯수</param>
        public static List<T> SelectRandom<T>(this IList<T> list, int selectCount) {
            selectCount = Mathf.Min(selectCount, list.Count);

            List<T> temp = new List<T>(list);

            for (int i = 0; i < selectCount; i++) {
                int j = Random.Range(i, temp.Count);
                (temp[i], temp[j]) = (temp[j], temp[i]);
            }

            return temp.GetRange(0, selectCount);
        }
    }
}