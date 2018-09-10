using System;
using System.Collections.Generic;
using System.Threading;
using static PatchworkServer;

namespace PatchworkSim.AI.KerasAlphaZero
{
	// http://mcts.ai/about/index.html
	// https://en.wikipedia.org/wiki/Monte_Carlo_tree_search
	// https://www.nature.com/articles/nature24270.epdf?author_access_token=VJXbVjaSHxFoctQQ4p2k4tRgN0jAjWel9jnR3ZoTv0PVW4gB86EEpGqTRDtpIz-2rmo8-KG06gqVobU5NSCFeHILHcVFUeMsbvwS-lxjqQGg98faovwjxeTUgZAUMnRQ

	/// <summary>
	/// Reusable parts of MonteCarloTreeSearch
	/// </summary>
	public class MonteCarloTreeSearchAlphaZero
	{
		private readonly Random _random = new Random(0);
		private readonly IPatchworkServerClient _client;

		public readonly int Iterations;

		internal static readonly ThreadLocal<SingleThreadedPool<MCTSAZNode>> NodePool = new ThreadLocal<SingleThreadedPool<MCTSAZNode>>(() => new SingleThreadedPool<MCTSAZNode>(), false);

		public MonteCarloTreeSearchAlphaZero(IPatchworkServerClient client, int iterations)
		{
			_client = client;
			Iterations = iterations;
		}

		/// <summary>
		/// Performs a MCTS search starting at the given state.
		/// Returns the root of the search tree, you must call NodePool.Value.ReturnAll() afterwards.
		/// </summary>
		internal MCTSAZNode PerformMCTS(SimulationState state)
		{
			var root = NodePool.Value.Get();
			state.CloneTo(root.State);

			var req = new EvaluateRequest();
			req.State.Add(GameStateFactory.CreateGameState(root.State));
			var res = _client.Evaluate(req);
			root.NetworkResult = res.Evaluations[0];

			for (var i = 0; i < Iterations; i++)
			{
				//Selection
				var leaf = Select(root);

				float leafValue;
				if (leaf.IsGameEnd)
				{
					//Value at a leaf is from the perspective of the player who performed an action to move the simulation to that state
					leafValue = (leaf.State.WinningPlayer == leaf.Parent.State.ActivePlayer) ? 1 : -1;
				}
				else
				{
					//Expansion
					Expand(leaf);

					//Randomly choose one of the newly expanded nodes
					leaf = Select(leaf);

					//Simulation
					leafValue = leaf.NetworkResult.WinRate;
				}

				//Backpropagation
				do
				{
					leaf.ReceiveBackpropagation(leafValue);
					leaf = leaf.Parent;

					//Value at a leaf is from the perspective of the player who performed an action to move the simulation to that state
					//When the player changes we need to negative the value so it is from the other players perspective
					if (leaf != null && leaf.Parent != null && leaf.Parent.State.ActivePlayer != leaf.State.ActivePlayer)
						leafValue = -leafValue;

				} while (leaf != null);
			}

			return root;
		}

		private MCTSAZNode Select(MCTSAZNode root)
		{
			while (root.Children.Count != 0) //Look for a leaf node (one we haven't expanded yet)
			{
				MCTSAZNode bestNext = null;
				double bestValue = double.MinValue;

				for (var i = 0; i < root.Children.Count; i++)
				{
					var c = root.Children[i];
					double uctValue = c.AverageValue + c.CalculateUCT(_random, root, root.NetworkResult.MoveRating[c.NetworkChildIndex]);
					// small random number to break ties randomly in unexpanded nodes
					if (root.Parent == null)
						uctValue *= _random.NextDouble();

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

		private void Expand(MCTSAZNode root)
		{
			root.Expand(_client);
		}

		/// <summary>
		/// Finds the best child based on their VisitCount
		/// </summary>
		/// <returns></returns>
		internal MCTSAZNode FindBestChild(MCTSAZNode root)
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

	internal class MCTSAZNode : IPoolableItem
	{
		public static readonly double Epsilon = 1e-6;
		public static readonly double ExplorationParameter = Math.Sqrt(2);

		public int VisitCount;
		public float Value;
		public float AverageValue;

		public readonly List<MCTSAZNode> Children;

		public int? PieceToPurchase;
		public int NetworkChildIndex;
		public readonly SimulationState State = new SimulationState();

		public MCTSAZNode Parent;

		public bool IsGameEnd => State.GameHasEnded;

		public Evaluation NetworkResult;


		public double CalculateUCT(Random random, MCTSAZNode parent, float priorProbability)
		{
			//https://web.stanford.edu/~surag/posts/alphazero.html
			//Page 8 https://www.nature.com/articles/nature24270.epdf?author_access_token=VJXbVjaSHxFoctQQ4p2k4tRgN0jAjWel9jnR3ZoTv0PVW4gB86EEpGqTRDtpIz-2rmo8-KG06gqVobU5NSCFeHILHcVFUeMsbvwS-lxjqQGg98faovwjxeTUgZAUMnRQ

			return ExplorationParameter * (priorProbability + Epsilon) * Math.Sqrt(parent.VisitCount + 1) / (1 + VisitCount);
		}

		public MCTSAZNode()
		{
			Children = new List<MCTSAZNode>(4);
		}

		public void ReceiveBackpropagation(float value)
		{
			VisitCount++;
			Value += value;

			AverageValue = Value / VisitCount;
		}

		public void Expand(IPatchworkServerClient client)
		{
#if DEBUG
			if (Children.Count != 0)
				throw new Exception("Cannot expand an already expanded node");
			if (IsGameEnd)
				throw new Exception("Cannot expand a GameEnd node");
#endif

			//Advance
			{
				var node = MonteCarloTreeSearchAlphaZero.NodePool.Value.Get();

				State.CloneTo(node.State);
				node.State.Fidelity = SimulationFidelity.NoPiecePlacing;
				node.State.PerformAdvanceMove();

				node.Parent = this;
				node.PieceToPurchase = null;
				node.NetworkChildIndex = 0;
				Children.Add(node);
			}

			//Purchase
			for (var i = 0; i < 3; i++)
			{
				//This cares if they can actually place the piece only when expanding the root node
				if (Helpers.ActivePlayerCanPurchasePiece(State, Helpers.GetNextPiece(State, i)))
				{
					var node = MonteCarloTreeSearchAlphaZero.NodePool.Value.Get();

					State.CloneTo(node.State);
					node.State.Fidelity = SimulationFidelity.NoPiecePlacing;
					var pieceIndex = node.State.NextPieceIndex + i;
					node.State.PerformPurchasePiece(pieceIndex);

					node.Parent = this;
					node.PieceToPurchase = pieceIndex;
					node.NetworkChildIndex = i + 1;
					Children.Add(node);
				}
			}

			//Evaluate the networks of our children
			var req = new EvaluateRequest();
			for (var i = 0; i < Children.Count; i++)
				req.State.Add(GameStateFactory.CreateGameState(Children[i].State));
			var res = client.Evaluate(req);
			for (var i = 0; i < Children.Count; i++)
				Children[i].NetworkResult = res.Evaluations[i];
		}

		public void Reset()
		{
			Children.Clear();
			Value = 0;
			VisitCount = 0;
			AverageValue = 0;
			Parent = null;
			NetworkResult = null;

			PieceToPurchase = null;
			NetworkChildIndex = -1;
		}
	}
}
