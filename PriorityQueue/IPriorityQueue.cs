using System.Collections.Generic;

namespace PriorityQueue
{
    public interface IPriorityQueue<T> : IEnumerable<T>
    {
        /// <summary>
        /// Enqueue a node to the priority queue. Lower values are placed in front.
        /// </summary>
        void Enqueue(T node, double priority);

        /// <summary>
        /// Removes the head of the queue and returns it.
        /// </summary>
        T Dequeue();

        /// <summary>
        /// Removes every node from the queue.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns whether the given node is in the queue.
        /// </summary>
        bool Contains(T node);

        /// <summary>
        /// Call this method to change the priority of a node.  
        /// </summary>
        void SetPriority(T node, double priority);

        /// <summary>
        /// Gets the priority of the given node.
        /// </summary>
        double GetPriority(T node);

        /// <summary>
        /// Returns the head of the queue, without removing it (use Dequeue() for that).
        /// </summary>
        T Peek();

        /// <summary>
        /// Returns the number of nodes in the queue.
        /// </summary>
        int Count { get; }
    }
}