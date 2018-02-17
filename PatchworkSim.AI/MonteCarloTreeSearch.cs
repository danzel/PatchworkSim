using System;
using System.Collections.Generic;

namespace PatchworkSim.AI
{
	// http://mcts.ai/about/index.html
	// https://en.wikipedia.org/wiki/Monte_Carlo_tree_search

	/// <summary>
	/// Reusable parts of MonteCarloTreeSearch
	/// </summary>
	public class MonteCarloTreeSearch<T> where T : MCTSNode<T>
	{
		private readonly Random _random = new Random(0);

		public T Select(T root)
		{
			while (root.Children.Count != 0) //Look for a leaf node (one we haven't expanded yet)
			{
				T bestNext = null;
				double bestValue = Double.MinValue;

				for (var i = 0; i < root.Children.Count; i++)
				{
					var c = root.Children[i];
					double uctValue = c.CalculateUCT(_random, root);
					// small random number to break ties randomly in unexpanded nodes
					if (uctValue > bestValue)
					{
						bestNext = c;
						bestValue = uctValue;
					}
				}

				root = bestNext;
			}

			return root;
		}

		/// <summary>
		/// Finds the best child based on their VisitCount
		/// </summary>
		/// <returns></returns>
		public T FindBestChild(T root)
		{
			//Perform the best move
			var best = root.Children[0];
			int bestVisitCount = root.Children[0].VisitCount;
			for (var index = 1; index < root.Children.Count; index++)
			{
				var child = root.Children[index];
				if (child.VisitCount > bestVisitCount) //TODO: Handle draws
				{
					best = child;
					bestVisitCount = child.VisitCount;
				}
			}

			return best;
		}
	}

	public abstract class MCTSNode
	{
		private static readonly double epsilon = 1e-6;
		private static readonly double explorationParameter = Math.Sqrt(2);

		public int VisitCount;
		public int Value;

		public double CalculateUCT(Random random, MCTSNode parent)
		{
			//https://en.wikipedia.org/wiki/Monte_Carlo_tree_search#Exploration_and_exploitation
			return Value / (VisitCount + epsilon) +
			       explorationParameter * Math.Sqrt(Math.Log(parent.VisitCount + 1) / (VisitCount + epsilon)) +
			       random.NextDouble() * epsilon; // small random number to break ties randomly in unexpanded nodes
		}
	}

	public abstract class MCTSNode<T> : MCTSNode where T : MCTSNode<T>
	{
		public readonly List<T> Children;

		protected MCTSNode(int initialChildrenSize)
		{
			Children = new List<T>(initialChildrenSize);
		}

		public abstract void Expand();
	}
}