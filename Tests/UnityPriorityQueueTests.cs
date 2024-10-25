using NUnit.Framework;
using Unity.Collections;
using DarkNaku.Foundation;

public class UnityPriorityQueueTests
{
    private UnityPriorityQueue<int> _priorityQueue;

    [SetUp]
    public void Setup()
    {
        // Allocator.TempJob은 PlayMode와 EditMode에서 모두 사용 가능
        _priorityQueue = new UnityPriorityQueue<int>(UnityPriorityQueue<int>.PRIORITY_SORT_TYPE.ASCENDING, 16, Allocator.Temp);
    }

    [TearDown]
    public void Teardown()
    {
        // 테스트 후 메모리 해제
        if (_priorityQueue.Count > 0)
            _priorityQueue.Dispose();
    }

    [Test]
    public void TestAddElements()
    {
        _priorityQueue.Add(10);
        _priorityQueue.Add(20);
        _priorityQueue.Add(5);

        Assert.AreEqual(3, _priorityQueue.Count, "The count should be 3 after adding three elements.");
    }

    [Test]
    public void TestPopElements()
    {
        _priorityQueue.Add(10);
        _priorityQueue.Add(20);
        _priorityQueue.Add(5);

        var minElement = _priorityQueue.Pop();

        Assert.AreEqual(5, minElement, "The minimum element should be 5.");
        Assert.AreEqual(2, _priorityQueue.Count, "The count should be 2 after popping one element.");
    }

    [Test]
    public void TestPeekElement()
    {
        _priorityQueue.Add(15);
        _priorityQueue.Add(10);
        _priorityQueue.Add(30);

        var topElement = _priorityQueue.Peek();

        Assert.AreEqual(10, topElement, "Peek should return the smallest element without removing it.");
        Assert.AreEqual(3, _priorityQueue.Count, "The count should still be 3 after Peek.");
    }

    [Test]
    public void TestPopOrder()
    {
        _priorityQueue.Add(10);
        _priorityQueue.Add(1);
        _priorityQueue.Add(20);
        _priorityQueue.Add(15);

        Assert.AreEqual(1, _priorityQueue.Pop(), "First pop should return 1.");
        Assert.AreEqual(10, _priorityQueue.Pop(), "Second pop should return 10.");
        Assert.AreEqual(15, _priorityQueue.Pop(), "Third pop should return 15.");
        Assert.AreEqual(20, _priorityQueue.Pop(), "Fourth pop should return 20.");
    }

    [Test]
    public void TestPopFromEmptyQueue()
    {
        Assert.DoesNotThrow(() => {
            int result = _priorityQueue.Pop();
            Assert.AreEqual(0, result, "Pop from empty queue should return default value (0 for int).");
        });
    }
}
