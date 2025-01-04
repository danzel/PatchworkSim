using System;
using System.Collections.Generic;
using System.Threading;
using PatchworkSim.AI.MoveMakers;

namespace PatchworkSim.AI;

// http://mcts.ai/about/index.html
// https://en.wikipedia.org/wiki/Monte_Carlo_tree_search

/// <summary>
/// Reusable parts of MonteCarloTreeSearch
/// </summary>
public class MonteCarloTreeSearch<T> where T : MCTSNode<T>, IPoolableItem, new()
{
	public readonly int Iterations;
	public readonly IMoveDecisionMaker RolloutMoveMaker;
	private readonly Random _random = new Random(0);

	internal static readonly ThreadLocal<SingleThreadedPool<T>> NodePool = new ThreadLocal<SingleThreadedPool<T>>(() => new SingleThreadedPool<T>(), false);

	/// <summary>
	/// Used by SimulateRollout
	/// </summary>
	private readonly SimulationState _rolloutState = new SimulationState();

	public MonteCarloTreeSearch(int iterations, IMoveDecisionMaker rolloutMoveMaker)
	{
		Iterations = iterations;
		RolloutMoveMaker = rolloutMoveMaker ?? new RandomMoveMaker(0);
	}

	/// <summary>
	/// Performs a MCTS search starting at the given state.
	/// Returns the root of the search tree, you must call NodePool.Value.ReturnAll() afterwards.
	/// </summary>
	public T PerformMCTS(SimulationState state, bool useMinusOne, double progressiveBiasWeight)
	{
		var root = NodePool.Value.Get();
		state.CloneTo(root.State);

		for (var i = 0; i < Iterations; i++)
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
				Expand(leaf, progressiveBiasWeight);

				//Randomly choose one of the newly expanded nodes
				leaf = Select(leaf);

				//Simulation
				winningPlayer = SimulateRollout(leaf.State);
			}

			//Backpropagation
			do
			{
				leaf.ReceiveBackpropagation(winningPlayer, useMinusOne);
				leaf = leaf.Parent;
			} while (leaf != null);
		}

		return root;
	}

	private int SimulateRollout(SimulationState baseState)
	{
		if (baseState.GameHasEnded)
			return baseState.WinningPlayer;

		baseState.CloneTo(_rolloutState);
		_rolloutState.Fidelity = SimulationFidelity.NoPiecePlacing;
		if (_rolloutState.PieceToPlace != null)
		{
			//Place it somewhere
			Helpers.GetFirstPlacement(_rolloutState.PlayerBoardState[_rolloutState.PieceToPlacePlayer], _rolloutState.PieceToPlace, out var bitmap, out var x, out var y);
			_rolloutState.PerformPlacePiece(bitmap, x, y);
		}

		//Run the game
		while (!_rolloutState.GameHasEnded)
		{
			RolloutMoveMaker.MakeMove(_rolloutState);
		}

		return _rolloutState.WinningPlayer;
	}

	private T Select(T root)
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

	protected virtual void Expand(T root, double progressiveBiasWeight)
	{
		root.Expand(progressiveBiasWeight);
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
	private static readonly double explorationParameter = 1.5;

	public int VisitCount;
	public int Value;

	protected double ProgressiveBias;

	public double CalculateUCT(Random random, MCTSNode parent)
	{
		//https://en.wikipedia.org/wiki/Monte_Carlo_tree_search#Exploration_and_exploitation
		var res = Value / (VisitCount + epsilon) +
		       explorationParameter * Math.Sqrt(Math.Log(parent.VisitCount + 1) / (VisitCount + epsilon)) +
		       random.NextDouble() * epsilon; // small random number to break ties randomly in unexpanded nodes

		res += ProgressiveBias / (VisitCount + 1);
		return res;
	}
}

public abstract class MCTSNode<T> : MCTSNode where T : MCTSNode<T>
{
	public readonly List<T> Children;

	public readonly SimulationState State = new SimulationState();
	public T Parent;
	public bool IsGameEnd => State.GameHasEnded;

	protected MCTSNode(int initialChildrenSize)
	{
		Children = new List<T>(initialChildrenSize);
	}

	public void ReceiveBackpropagation(int winningPlayer, bool useMinusOne)
	{
		VisitCount++;
		if (Parent != null && winningPlayer == Parent.State.ActivePlayer)
			Value++;
		else if (useMinusOne)
			Value--;
	}

	public abstract void Expand(double progressiveBiasWeight);

	public virtual void Reset()
	{
		Children.Clear();
		Value = 0;
		VisitCount = 0;
		Parent = null;
	}
}