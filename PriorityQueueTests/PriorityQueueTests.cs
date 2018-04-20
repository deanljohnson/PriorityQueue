using System;
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