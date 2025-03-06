using NUnit.Framework;
using Unity.Collections;
using DarkNaku.Foundation;

public class UnityPriorityQueueTests
{
    private UnityPriorityQueue<string, int> _priorityQueue;

    [SetUp]
    public void Setup()
    {
        _priorityQueue = new UnityPriorityQueue<string, int>();
    }

    [Test]
    public void TestAddElements()
    {
        _priorityQueue.Enqueue("B", 10);
        _priorityQueue.Enqueue("C", 20);
        _priorityQueue.Enqueue("A", 5);

        Assert.AreEqual(3, _priorityQueue.Count, "The count should be 3 after adding three elements.");

        _priorityQueue.Clear();
    }

    [Test]
    public void TestPopElements()
    {
        _priorityQueue.Enqueue("B", 10);
        _priorityQueue.Enqueue("C", 20);
        _priorityQueue.Enqueue("A", 5);

        var minElement = _priorityQueue.Dequeue();

        Assert.AreEqual("A", minElement, "The minimum element should be 5.");
        Assert.AreEqual(2, _priorityQueue.Count, "The count should be 2 after popping one element.");

        _priorityQueue.Clear();
    }

    [Test]
    public void TestPeekElement()
    {
        _priorityQueue.Enqueue("B", 10);
        _priorityQueue.Enqueue("C", 20);
        _priorityQueue.Enqueue("A", 5);

        var topElement = _priorityQueue.Peek();

        Assert.AreEqual("A", topElement, "Peek should return the smallest element without removing it.");
        Assert.AreEqual(3, _priorityQueue.Count, "The count should still be 3 after Peek.");

        _priorityQueue.Clear();
    }

    [Test]
    public void TestPopOrder()
    {
        _priorityQueue.Enqueue("B", 10);
        _priorityQueue.Enqueue("A", 1);
        _priorityQueue.Enqueue("D", 20);
        _priorityQueue.Enqueue("C", 15);

        Assert.AreEqual("A", _priorityQueue.Dequeue(), "First pop should return 1.");
        Assert.AreEqual("B", _priorityQueue.Dequeue(), "Second pop should return 10.");
        Assert.AreEqual("C", _priorityQueue.Dequeue(), "Third pop should return 15.");
        Assert.AreEqual("D", _priorityQueue.Dequeue(), "Fourth pop should return 20.");

        _priorityQueue.Clear();
    }
}
