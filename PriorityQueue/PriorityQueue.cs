using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PriorityQueue
{
    public class PriorityQueue<TItem> : IPriorityQueue<TItem>
    {
        private class MinComparer : IComparer<double>
        {
            public int Compare(double x, double y)
            {
                return (int)Math.Ceiling(x - y);
            }
        }

        private class MaxComparer : IComparer<double>
        {
            public int Compare(double x, double y)
            {
                return (int)Math.Ceiling(y - x);
            }
        }

        private class ItemRecord
        {
            public int Index;
            public readonly TItem Item;
            public double Priority;

            public ItemRecord(int index, TItem item, double priority)
            {
                Index = index;
                Item = item;
                Priority = priority;
            }
        }

        private readonly IComparer<double> m_Comparer; 
        private readonly List<ItemRecord> m_Data;
        private readonly Dictionary<TItem, ItemRecord> m_ItemRecordsMap;
        private ItemRecord m_LastItem;

        public int Count => m_Data.Count;

        public PriorityQueue(IEqualityComparer<TItem> keyComparer, bool maxPriority = false)
        {
            m_Data = new List<ItemRecord>();
            m_ItemRecordsMap = new Dictionary<TItem, ItemRecord>(keyComparer);

            if (maxPriority)
                m_Comparer = new MaxComparer();
            else
                m_Comparer = new MinComparer();
        }

        public PriorityQueue(bool maxPriority = false)
        {
            m_Data = new List<ItemRecord>();
            m_ItemRecordsMap = new Dictionary<TItem, ItemRecord>();

            if (maxPriority)
                m_Comparer = new MaxComparer();
            else
                m_Comparer = new MinComparer();
        }

        public PriorityQueue(IComparer<double> priorityComparer)
        {
            m_Data = new List<ItemRecord>();
            m_ItemRecordsMap = new Dictionary<TItem, ItemRecord>();
            m_Comparer = priorityComparer;
        }

        public PriorityQueue(IEqualityComparer<TItem> keyComparer, IComparer<double> priorityComparer)
        {
            m_Data = new List<ItemRecord>();
            m_ItemRecordsMap = new Dictionary<TItem, ItemRecord>(keyComparer);
            m_Comparer = priorityComparer;
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public void Enqueue(TItem item, double priority)
        {
#if DEBUG
            if (m_Priorities.ContainsKey(item))
                throw new ArgumentException("The PriorityQueue already contains this item");
#endif
            ItemRecord record = new ItemRecord(m_Data.Count, item, priority);
            m_Data.Add(record);
            m_ItemRecordsMap[item] = record;

            HeapifyUp(m_Data.Count - 1);

            if (m_LastItem == null || m_Comparer.Compare(priority, m_LastItem.Priority) > 0)
                m_LastItem = record;
        }

        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public TItem Dequeue()
        {
            #if DEBUG
            if (m_Data.Count == 0)
                throw new InvalidOperationException("Cannot dequeue from an empty PriorityQueue");
            #endif

            var lastIndex = m_Data.Count - 1;

            var frontItem = m_Data[0];
            m_Data[0] = m_Data[lastIndex];
            m_Data.RemoveAt(lastIndex);

            HeapifyDown(0);

            m_ItemRecordsMap.Remove(frontItem.Item);

            if (m_Data.Count == 0)
                m_LastItem = default(ItemRecord);

            return frontItem.Item;
        }

        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public TItem Peek()
        {
            #if DEBUG
            if (m_Data.Count == 0)
                throw new InvalidOperationException("The PriorityQueue is empty");
            #endif

            var frontItem = m_Data[0];
            return frontItem.Item;
        }


        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        /// <summary>
        /// Returns the item with the highest priority value,
        /// or default(TItem) if the queue is empty
        /// </summary>
        public TItem PeekLast()
        {
            return m_LastItem.Item;
        }

        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public void Clear()
        {
            m_Data.Clear();
            m_ItemRecordsMap.Clear();
            m_LastItem = default(ItemRecord);
        }

        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public void SetPriority(TItem item, double priority)
        {
            #if DEBUG
            if (!m_Priorities.ContainsKey(item))
                throw new InvalidOperationException("The PriorityQueue does not contain the given item");
            #endif

            ItemRecord record = m_ItemRecordsMap[item];
            record.Priority = priority;
            if (priority > m_LastItem.Priority)
                m_LastItem = record;

            var index = record.Index;

            var parentIndex = (index) / 2;
            if (m_Comparer.Compare(priority, m_Data[parentIndex].Priority) < 0)
            {
                HeapifyUp(index);
                return;
            }

            var childIndex = index * 2 + 1;
            var rc = childIndex + 1;
            if ((childIndex < m_Data.Count && m_Comparer.Compare(priority, m_Data[childIndex].Priority) > 0) 
                || (rc < m_Data.Count && m_Comparer.Compare(priority, m_Data[childIndex + 1].Priority) > 0))
            {
                HeapifyDown(index);
            }
        }

        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public double GetPriority(TItem item)
        {
            return m_ItemRecordsMap[item].Priority;
        }

        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public bool Contains(TItem item)
        {
            return m_ItemRecordsMap.ContainsKey(item);
        }

        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        private void HeapifyUp(int i)
        {
            var childIndex = i;

            while (childIndex > 0)
            {
                var parentIndex = (childIndex) / 2;

                if (m_Comparer.Compare(m_Data[childIndex].Priority, m_Data[parentIndex].Priority) >= 0)
                    break;

                var tmp = m_Data[childIndex];
                m_Data[childIndex] = m_Data[parentIndex];
                m_Data[childIndex].Index = childIndex;
                m_Data[parentIndex] = tmp;
                m_Data[parentIndex].Index = parentIndex;

                childIndex = parentIndex;
            }
        }

        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        private void HeapifyDown(int i)
        {
            var parentIndex = i;
            var lastIndex = m_Data.Count - 1;

            while (true)
            {
                var childIndex = parentIndex * 2 + 1;

                if (childIndex > lastIndex)
                    break;

                var rc = childIndex + 1;

                if (rc <= lastIndex && m_Comparer.Compare(m_Data[rc].Priority, m_Data[childIndex].Priority) < 0)
                    childIndex = rc;

                if (m_Comparer.Compare(m_Data[parentIndex].Priority, m_Data[childIndex].Priority) <= 0)
                    break;

                var tmp = m_Data[parentIndex];
                m_Data[parentIndex] = m_Data[childIndex];
                m_Data[parentIndex].Index = parentIndex;
                m_Data[childIndex] = tmp;
                m_Data[childIndex].Index = childIndex;

                parentIndex = childIndex;
            }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return m_Data.Select(ir => ir.Item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
