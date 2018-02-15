﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace PatchworkSim.AI.MoveMakers
{
	public abstract class BaseMonteCarloTreeSearchMoveMaker : IMoveDecisionMaker
	{
		public abstract string Name { get; }
		protected readonly int _iterations;
		private readonly Random _random = new Random(0);
		protected readonly IMoveDecisionMaker _rolloutMoveMaker;

		/// <summary>
		/// Used by SimulateRollout
		/// </summary>
		private readonly SimulationState _rolloutState = new SimulationState();

		protected static readonly ThreadLocal<SearchNodePool> NodePool = new ThreadLocal<SearchNodePool>(() => new SearchNodePool(), false);

		protected BaseMonteCarloTreeSearchMoveMaker(int iterations, IMoveDecisionMaker rolloutMoveMaker = null)
		{
			_iterations = iterations;
			_rolloutMoveMaker = rolloutMoveMaker ?? new RandomMoveMaker(0);
		}

		public abstract void MakeMove(SimulationState state);

		// http://mcts.ai/about/index.html
		// https://en.wikipedia.org/wiki/Monte_Carlo_tree_search
		/// <summary>
		/// Performs a MCTS search starting at the given state.
		/// Returns the root of the search tree, you must call NodePool.Value.ReturnAll() afterwards.
		/// </summary>
		protected SearchNode PerformMCTS(SimulationState state)
		{
			var root = NodePool.Value.Get();
			state.CloneTo(root.State);

			for (var i = 0; i < _iterations; i++)
			{
				//Selection
				var leaf = Select(root);

				int winningPlayer;
				if (leaf.IsGameEnd)
				{
					winningPlayer = leaf.State.WinningPlayer;
				}
				else
				{
					//Expansion
					leaf.Expand();

					//Randomly choose one of the newly expanded nodes
					leaf = Select(leaf);

					//Simulation
					winningPlayer = SimulateRollout(leaf.State);
				}

				//Backpropagation
				do
				{
					leaf.ReceiveBackpropagation(winningPlayer);
					leaf = leaf.Parent;
				} while (leaf != null);
			}

			return root;
		}

		/// <summary>
		/// Finds the best child based on their CisitCount
		/// </summary>
		/// <returns></returns>
		protected SearchNode FindBestChild(SearchNode root)
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

		private static readonly double epsilon = 1e-6;
		private static readonly double explorationParameter = Math.Sqrt(2);

		private SearchNode Select(SearchNode root)
		{
			while (root.Children.Count != 0) //Look for a leaf node (one we haven't expanded yet)
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

		private int SimulateRollout(SimulationState baseState)
		{
			if (baseState.GameHasEnded)
				return baseState.WinningPlayer;

			_rolloutState.Pieces.Clear();
			baseState.CloneTo(_rolloutState);
			_rolloutState.Fidelity = SimulationFidelity.NoPiecePlacing;

			//Run the game
			while (!_rolloutState.GameHasEnded)
			{
				_rolloutMoveMaker.MakeMove(_rolloutState);
			}

			return _rolloutState.WinningPlayer;
		}

		public class SearchNode
		{
			public readonly SimulationState State = new SimulationState();
			public SearchNode Parent;

			/// <summary>
			/// The piece that was purchased to arrive at this node, or null for advance
			/// </summary>
			public int? PieceToPurchase;

			public readonly List<SearchNode> Children = new List<SearchNode>(4);

			public int VisitCount;
			public int Value;

			public bool IsGameEnd => State.GameHasEnded;

			public void Expand()
			{
#if DEBUG
				if (Children.Count != 0)
					throw new Exception("Cannot expand an already expanded node");
				if (IsGameEnd)
					throw new Exception("Cannot expand a GameEnd node");
#endif
				//Advance
				{
					var node = NodePool.Value.Get();

					State.CloneTo(node.State);
					node.State.Fidelity = SimulationFidelity.NoPiecePlacing; //TODO Could do real placing
					node.State.PerformAdvanceMove();

					node.Parent = this;
					Children.Add(node);
				}

				for (var i = 0; i < 3; i++)
				{
					//This cares if they can actually place the piece only when expanding the root node
					if (Helpers.ActivePlayerCanPurchasePiece(State, Helpers.GetNextPiece(State, i)))
					{
						var node = NodePool.Value.Get();

						State.CloneTo(node.State);
						node.State.Fidelity = SimulationFidelity.NoPiecePlacing; //TODO Could do real placing
						var pieceIndex = node.State.NextPieceIndex + i;
						node.State.PerformPurchasePiece(pieceIndex);

						node.Parent = this;
						node.PieceToPurchase = pieceIndex;
						Children.Add(node);
					}
				}
			}

			public void ReceiveBackpropagation(int winningPlayer)
			{
				VisitCount++;
				if (Parent != null && winningPlayer == Parent.State.ActivePlayer)
					Value++;
			}
		}

		public class SearchNodePool
		{
			private readonly List<SearchNode> _searchNodePool = new List<SearchNode>();
			private int _getIndex;

			public SearchNode Get()
			{
				if (_getIndex < _searchNodePool.Count)
				{
					var index = _getIndex;
					_getIndex++;
					//FetchedFromPool++;
					return _searchNodePool[index];
				}

				var res = new SearchNode();
				_searchNodePool.Add(res);
				_getIndex++;
				return res;
			}

			public void ReturnAll()
			{
				for (var i = 0; i < _getIndex; i++)
				{
					var value = _searchNodePool[i];
					value.Children.Clear();
					value.State.Pieces.Clear();
					value.Value = 0;
					value.VisitCount = 0;
					value.Parent = null;
					value.PieceToPurchase = null;
				}

				_getIndex = 0;
			}
		}
	}
}
