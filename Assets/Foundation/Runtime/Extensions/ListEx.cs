using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListEx {
    public static void Shuffle<T>(this IList<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);

            if (i != j) {
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }

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