using System;
using System.Collections.Generic;

namespace PatchworkSim.AI.MoveMakers
{
	/// <summary>
	/// Performs a MonteCarloTreeSearch of possible moves to figure out the best one to take.
	/// Only considers if the piece can be actually placed for the initial move, any future moves use no placement simulation.
	/// </summary>
	public class MoveOnlyMonteCarloTreeSearchMoveMaker : IMoveDecisionMaker
	{
		public string Name => $"MonteCarloTreeSearchMoveMaker({_iterations})";

		private readonly int _iterations;
		private readonly Random _random = new Random(0);
		private readonly RandomMoveMaker _randomMoveMaker = new RandomMoveMaker(0);

		public MoveOnlyMonteCarloTreeSearchMoveMaker(int iterations)
		{
			_iterations = iterations;
		}

		// http://mcts.ai/about/index.html
		// https://en.wikipedia.org/wiki/Monte_Carlo_tree_search
		public void MakeMove(SimulationState state)
		{
			var root = new SearchNode(state, null, null);

			for (var i = 0; i < _iterations; i++)
			{
				//Selection
				var leaf = Select(root);

				if (leaf.IsGameEnd)
					continue;

				//Expansion
				leaf.Expand();

				var newLeaf = Select(leaf);

				//Simulation
				var winningPlayer = Simulate(newLeaf.State);

				//Backpropagation
				do
				{
					newLeaf.ReceiveBackpropogation(winningPlayer);
					newLeaf = newLeaf.Parent;
				} while (newLeaf != null);
			}

			//Perform the best move
			var best = root.Children[0].PieceToPurchase;
			int bestVisitCount = root.Children[0].VisitCount;
			for (var index = 1; index < root.Children.Count; index++)
			{
				var child = root.Children[index];
				if (child.VisitCount > bestVisitCount) //TODO: Handle draws
				{
					best = child.PieceToPurchase;
					bestVisitCount = child.VisitCount;
				}
			}

			if (best.HasValue)
				state.PerformPurchasePiece(best.Value);
			else
				state.PerformAdvanceMove();
		}

		private static readonly double epsilon = 1e-6;
		private static readonly double explorationParameter = Math.Sqrt(2);

		private SearchNode Select(SearchNode root)
		{
			while (root.Children != null)
			{
				SearchNode bestNext = null;
				double bestValue = Double.MinValue;

				for (var i = 0; i < root.Children.Count; i++)
				{
					var c = root.Children[i];
					//https://en.wikipedia.org/wiki/Monte_Carlo_tree_search#Exploration_and_exploitation
					double uctValue = c.Value / (c.VisitCount + epsilon) +
					                  explorationParameter * Math.Sqrt(Math.Log(root.VisitCount + 1) / (c.VisitCount + epsilon)) +
					                  _random.NextDouble() * epsilon;
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

		private int Simulate(SimulationState baseState)
		{
			if (baseState.GameHasEnded)
				return baseState.WinningPlayer;

			var cloneState = baseState.Clone();
			cloneState.Fidelity = SimulationFidelity.NoPiecePlacing;

			cloneState.PerformAdvanceMove();

			//Run the game
			while (!cloneState.GameHasEnded)
			{
				_randomMoveMaker.MakeMove(cloneState);
			}

			return cloneState.WinningPlayer;
		}

		class SearchNode
		{
			public readonly SimulationState State;
			public readonly SearchNode Parent;
			public readonly int? PieceToPurchase;

			public List<SearchNode> Children;

			public int VisitCount;
			public int Value;

			/// <param name="pieceToPurchase">The piece that was purchased to arrive at this node, or null for advance</param>
			public SearchNode(SimulationState state, SearchNode parent, int? pieceToPurchase)
			{
				State = state;
				Parent = parent;
				PieceToPurchase = pieceToPurchase;
			}

			public bool IsGameEnd => State.GameHasEnded;

			public void Expand()
			{
				if (Children != null)
					throw new Exception("Cannot expand an already expanded node");
				if (IsGameEnd)
					throw new Exception("Cannot expand a GameEnd node");


				Children = new List<SearchNode>(4);

				//Advance
				{
					var stateClone = State.Clone();
					stateClone.Fidelity = SimulationFidelity.NoPiecePlacing; //TODO Could do real placing
					stateClone.PerformAdvanceMove();
					Children.Add(new SearchNode(stateClone, this, null));
				}

				for (var i = 0; i < 3; i++)
				{
					//This cares if they can actually place the piece only when expanding the root node
					if (Helpers.ActivePlayerCanPurchasePiece(State, Helpers.GetNextPiece(State, i)))
					{
						var stateClone = State.Clone();
						stateClone.Fidelity = SimulationFidelity.NoPiecePlacing; //TODO Could do real placing
						var pieceIndex = stateClone.NextPieceIndex + i;
						stateClone.PerformPurchasePiece(pieceIndex);
						Children.Add(new SearchNode(stateClone, this, pieceIndex));
					}
				}
			}

			public void ReceiveBackpropogation(int winningPlayer)
			{
				VisitCount++;
				if (Parent != null && winningPlayer == Parent.State.ActivePlayer)
					Value++;
			}
		}
	}
}