using PriorityQueue;
using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace PriorityQueueBenchmark
{
	public class BenchmarkingClass
	{
		private class TestClass
		{

		}

		private struct TestStruct
		{

		}

		private readonly PriorityQueue<TestClass> m_ClassQueue;
		private readonly PriorityQueue<TestStruct> m_StructQueue;
		private readonly Random m_Random;

		public BenchmarkingClass()
		{
			m_ClassQueue = new PriorityQueue<TestClass>();
			m_StructQueue = new PriorityQueue<TestStruct>();
			m_Random = new Random(0);
		}

		[Params(1000,10000,100000)]
		public int N;
		
		[Benchmark]
		public void EnqueueDequeueClass()
		{
			for (int i = 0; i < N; i++)
				m_ClassQueue.Enqueue(new TestClass(), m_Random.NextDouble());
			for (int i = 0; i < N; i++)
				m_ClassQueue.Dequeue();
		}

		[Benchmark]
		public void EnqueueDequeueStruct()
		{
			for (int i = 0; i < N; i++)
				m_StructQueue.Enqueue(new TestStruct(), m_Random.NextDouble());
			for (int i = 0; i < N; i++)
				m_StructQueue.Dequeue();
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<BenchmarkingClass>();
		}
	}
}
