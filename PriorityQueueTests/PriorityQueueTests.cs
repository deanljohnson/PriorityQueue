using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PriorityQueue;

namespace PriorityQueueTests
{
    public class TestData
    {
    }

    // TODO: add tests for infinity and NaN priorities
    [TestClass]
    public class PriorityQueueTests
    {
        [TestMethod]
        public void PriorityQueueTest()
        {
            var pq = new PriorityQueue<TestData>();

            //Test adding items
            var items = new TestData[10];
            for (var i = 0; i < 10; i++)
            {
                items[i] = new TestData();
                pq.Enqueue(items[i], 10f - i);
                Assert.IsTrue(pq.Contains(items[i]));
                Assert.AreEqual(10f - i, pq.GetPriority(items[i]));
            }

            Assert.AreEqual(pq.Count, 10);

            //Test dequeue ordering
            var lastItemPriority = pq.GetPriority(pq.Peek());
            var lastItem = pq.Dequeue();
            Assert.IsNotNull(lastItem);
            while (pq.Count > 0)
            {
                var nextItemPriority = pq.GetPriority(pq.Peek());
                var nextItem = pq.Dequeue();
                Assert.IsNotNull(nextItem);
                Assert.IsTrue(lastItemPriority <= nextItemPriority);
            }

            var random = new Random();
            var dict = new Dictionary<TestData, float>();
            //Test with many random values
            for (var i = 0; i < 100000; i++)
            {
                var p = random.Next(0, 100000);
                var t = new TestData();
                dict.Add(t, p);
                pq.Enqueue(t, p);
            }

            lastItemPriority = pq.GetPriority(pq.Peek());
            lastItem = pq.Dequeue();
            Assert.IsNotNull(lastItem);
            while (pq.Count > 0)
            {
                var nextItemPriority = pq.GetPriority(pq.Peek());
                var nextItem = pq.Dequeue();
                Assert.IsNotNull(nextItem);
                Assert.IsTrue(lastItemPriority <= nextItemPriority);
                Assert.AreEqual(nextItemPriority, dict[nextItem]);
            }
        }

        [TestMethod]
        public void SetPriorityTest()
        {
            var dict = new Dictionary<TestData, double>();

            var initQueue = new Action<PriorityQueue<TestData>>((pq) =>
            {
                var items = new List<TestData>();
                var random = new Random();

                for (var i = 0; i < 10000; i++)
                {
                    items.Add(new TestData());
                    pq.Enqueue(items[i], i);
                }

                for (var i = 0; i < 10000; i++)
                {
                    var p = random.NextDouble() * 10000.0;
                    dict.Add(items[i], p);

                    pq.SetPriority(items[i], p);
                }
            });

            // Test minimum queue (default)
            var minQueue = new PriorityQueue<TestData>();
            initQueue(minQueue);
            var lastItemPriority = minQueue.GetPriority(minQueue.Peek());
            var lastItem = minQueue.Dequeue();
            Assert.IsNotNull(lastItem);
            while (minQueue.Count > 0)
            {
                var nextItemPriority = minQueue.GetPriority(minQueue.Peek());
                var nextItem = minQueue.Dequeue();
                Assert.IsNotNull(nextItem);
                Assert.IsTrue(lastItemPriority <= nextItemPriority, $"{lastItemPriority} <= {nextItemPriority}");
                Assert.AreEqual(nextItemPriority, dict[nextItem]);
            }

            // Test maximum queue
            dict.Clear();
            var maxQueue = new PriorityQueue<TestData>(true);
            initQueue(maxQueue);
            lastItemPriority = maxQueue.GetPriority(maxQueue.Peek());
            lastItem = maxQueue.Dequeue();
            Assert.IsNotNull(lastItem);
            while (maxQueue.Count > 0)
            {
                var nextItemPriority = maxQueue.GetPriority(maxQueue.Peek());
                var nextItem = maxQueue.Dequeue();
                Assert.IsNotNull(nextItem);
                Assert.IsTrue(lastItemPriority >= nextItemPriority, $"{lastItemPriority} <= {nextItemPriority}");
                Assert.AreEqual(nextItemPriority, dict[nextItem]);
            }
        }

        [TestMethod]
        public void PeekLastTest()
        {
            // Min queue tests
            var queue = new PriorityQueue<TestData>();

            // Nothing in queue
            Assert.AreEqual(default(TestData), queue.PeekLast());

            var one = new TestData();
            var five = new TestData();
            var ten = new TestData();

            // Enqueue order one
            queue.Enqueue(one, 1);
            queue.Enqueue(five, 5);
            queue.Enqueue(ten, 10);
            Assert.AreEqual(ten, queue.PeekLast());
            queue.Clear();

            // Enqueue order two
            queue.Enqueue(ten, 10);
            queue.Enqueue(five, 5);
            queue.Enqueue(one, 1);
            Assert.AreEqual(ten, queue.PeekLast());
            queue.Clear();

            // Max queue tests
            queue = new PriorityQueue<TestData>(true);

            // Enqueue order one
            queue.Enqueue(one, 1);
            queue.Enqueue(five, 5);
            queue.Enqueue(ten, 10);
            Assert.AreEqual(one, queue.PeekLast());
            queue.Clear();

            // Enqueue order two
            queue.Enqueue(ten, 10);
            queue.Enqueue(five, 5);
            queue.Enqueue(one, 1);
            Assert.AreEqual(one, queue.PeekLast());
            queue.Clear();
        }

        [TestMethod]
        public void ClearTest()
        {
            var queue = new PriorityQueue<TestData>();

            // Make sure empty queue doesn't cause a problem
            queue.Clear();

            // Single object enqueued
            queue.Enqueue(new TestData(), 10);
            Assert.AreEqual(1, queue.Count);
            Assert.AreNotEqual(default(TestData), queue.PeekLast());
            queue.Clear();
            Assert.AreEqual(0, queue.Count);
            Assert.AreEqual(default(TestData), queue.PeekLast()); // Verify nulling of last item

            // Many objects enqueued
            for (int i = 0; i < 1000; i++)
                queue.Enqueue(new TestData(), 10);
            Assert.AreEqual(1000, queue.Count);
            Assert.AreNotEqual(default(TestData), queue.PeekLast());
            queue.Clear();
            Assert.AreEqual(0, queue.Count);
            Assert.AreEqual(default(TestData), queue.PeekLast()); // Verify nulling of last item
        }

        [TestMethod]
        public void EnumerationTest()
        {
            HashSet<TestData> data = new HashSet<TestData>();
            for (int i = 0; i < 1000; i++) data.Add(new TestData());

            var queue = new PriorityQueue<TestData>();
            foreach (var d in data) queue.Enqueue(d, 0);

            foreach (object dataItem in (IEnumerable)queue)
            {
                Assert.IsTrue(data.Contains((TestData)dataItem));
            }
        }

        [TestMethod]
        public void EmptyQueueTests()
        {
#if DEBUG
            var queue = new PriorityQueue<TestData>();
            AssertThrows<InvalidOperationException>(() => queue.Peek());
            AssertThrows<InvalidOperationException>(() => queue.Dequeue());
#endif
        }

        internal static void AssertThrows<TException>(Action method)
            where TException : Exception
        {
            try
            {
                method.Invoke();
            }
            catch (TException)
            {
                return; // Expected exception.
            }
            catch (Exception ex)
            {
                Assert.Fail("Wrong exception thrown: " + ex.Message);
            }
            Assert.Fail("No exception thrown");
        }
    }
}