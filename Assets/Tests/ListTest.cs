using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DarkNaku.Foundation;

public class ListTest {
    [Test]
    public void ShuffleTest() {
        var original = new List<int> { 1, 2, 3, 4, 5 };
        var shuffled = new List<int>(original);

        shuffled.Shuffle();

        Assert.IsFalse(original.SequenceEqual(shuffled), "리스트가 원래 순서와 동일합니다. 셔플이 제대로 작동하지 않았습니다.");
    }

    [Test]
    public void SelectRandomTest() {
        var original = new List<int> { 1, 2, 3, 4, 5 };
        var selectCount = 3;
        var selected = original.SelectRandom(selectCount);

        Assert.AreEqual(selectCount, selected.Count, "선택된 아이템의 갯수가 일치하지 않습니다.");
        Assert.IsTrue(selected.All(item => original.Contains(item)), "선택된 아이템이 원본 리스트에 존재하지 않습니다.");
    }
}