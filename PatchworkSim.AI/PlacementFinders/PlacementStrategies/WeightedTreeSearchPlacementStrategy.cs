using System;
using System.Collections.Generic;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	/// <summary>
	/// Performs a tree search of future possible boards guided by the utility function.
	/// The placement that leads to the deepest known tree (tie breaked by the amount of possible ways to reach it) is selected
	/// </summary>
	public class WeightedTreeSearchPlacementStrategy : IPlacementStrategy
	{
		public string Name => $"WeightedTreeSearch({_iterations}@{_maxBranching}+{_utilityFunction.Name})";
		public bool ImplementsLookahead => true;

		private readonly WTSUtilityFunction _utilityFunction;
		private readonly int _iterations;
		private readonly int _maxBranching;

		private readonly Random _random = new Random(0);

		public WeightedTreeSearchPlacementStrategy(WTSUtilityFunction utilityFunction, int iterations, int maxBranching)
		{
			_utilityFunction = utilityFunction;
			_iterations = iterations;
			_maxBranching = maxBranching;
		}

		public bool TryPlacePiece(BoardState board, PieceDefinition piece, List<int> possibleFuturePieces, int possibleFuturePiecesOffset, out PieceBitmap bitmap, out int x, out int y)
		{
			//TODO: When board is empty we should remove rotations/mirrors of the placements (not worth it on future ones)
			//TODO: How should we select which piece we are likely to place after the first one? For now we will always assume we get the next one in the list

			var root = new SearchNode(board, null, -1, -1, -1);

			Expand(root, piece);

			//Perform a MCTS style thingy, but weighting going down a branch by the utility function
			int stuckCount = 0;
			for (var iteration = 0; iteration < _iterations; iteration++)
			{
				var leaf = Select(root);

				if (leaf == null)
				{
					stuckCount++;
					continue;
				}

				Expand(leaf, PieceDefinition.AllPieceDefinitions[possibleFuturePieces[(possibleFuturePiecesOffset + leaf.Depth) % possibleFuturePieces.Count]]);
			}

			if (root.Children.Count > 0)
			{
				//TODO: Tie Break
				//TODO: Could backpropagate max depth instead of calculating it at the end. Then could weight searches to go explore deeper places?
				var bestChild = root.Children[0];
				var bestChildDepth = root.Children[0].CalculateMaxChildDepth();
				var ties = 0;

				for (var i = 1; i < root.Children.Count; i++)
				{
					var c = root.Children[i];
					var depth = c.CalculateMaxChildDepth();
					if (depth > bestChildDepth)
					{
						bestChild = c;
						bestChildDepth = depth;
						ties = 0;
					}
					else if (depth == bestChildDepth)
					{
						ties++;
					}
				}

				//Console.WriteLine($"Best Depth:{bestChildDepth},  ties: {ties},  stuck: {stuckCount}");

				bitmap = bestChild.Bitmap;
				x = bestChild.X;
				y = bestChild.Y;
				return true;
			}
			else
			{
				bitmap = null;
				x = 0;
				y = 0;
				return false;
			}
		}

		private SearchNode Select(SearchNode currentRoot)
		{
			while (currentRoot.Children != null) //Look for a leaf node (one we haven't expanded yet)
			{
				//Hit a leaf that has no future possible placements
				if (currentRoot.Children.Count == 0)
					return null;

				var selectedUtility = _random.NextDouble() * currentRoot.ChildrenUtilitySum;

				bool foundOne = false;
				for (var i = 0; i < currentRoot.Children.Count; i++)
				{
					var c = currentRoot.Children[i];
					selectedUtility -= c.Utility;

					if (selectedUtility < 0) //TODO: <= ?
					{
						//Console.Write(" -> " + i);
						currentRoot = c;
						foundOne = true;
						break;
					}
				}

				if (!foundOne)
					throw new Exception();
			}
			//Console.WriteLine();

			return currentRoot;
		}

		private void Expand(SearchNode node, PieceDefinition piece)
		{
			var children = new List<SearchNode>();

			//Exhaustively place it and make new child nodes
			for (var index = 0; index < piece.PossibleOrientations.Length; index++)
			{
				var bitmap = piece.PossibleOrientations[index];
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						if (node.Board.CanPlace(bitmap, x, y))
						{
							var copy = node.Board;
							copy.Place(bitmap, x, y);
							var child = new SearchNode(copy, bitmap, x, y, node.Depth + 1);
							//evaluate child nodes
							child.Utility = _utilityFunction.Evaluate(copy);
							children.Add(child);
						}
					}
				}
			}

			//TODO: If this is the first piece, remove mirrors/rotations from the children

			//Sort child nodes
			children.Sort();

			//Remove excess child nodes
			if (children.Count > _maxBranching)
				children.RemoveRange(_maxBranching, children.Count - _maxBranching);

			//Set children
			node.Children = children;

			for (var i = 0; i < children.Count; i++)
			{
				var c = children[i];
				node.ChildrenUtilitySum += c.Utility;
			}
		}


		class SearchNode : IComparable<SearchNode>
		{
			public readonly BoardState Board;
			public readonly PieceBitmap Bitmap;
			public readonly int X, Y;
			public readonly int Depth;

			public double Utility;
			public double ChildrenUtilitySum;
			public List<SearchNode> Children = null;

			public SearchNode(BoardState board, PieceBitmap bitmap, int x, int y, int depth)
			{
				Board = board;
				Bitmap = bitmap;
				X = x;
				Y = y;
				Depth = depth;
			}

			public int CompareTo(SearchNode other)
			{
				return -Utility.CompareTo(other.Utility);
			}

			public int CalculateMaxChildDepth()
			{
				var deepest = Depth;
				if (Children != null)
				{
					for (var i = 0; i < Children.Count; i++)
					{
						var c = Children[i];
						deepest = Math.Max(deepest, c.CalculateMaxChildDepth());
					}
				}

				return deepest;
			}
		}


		public interface WTSUtilityFunction
		{
			string Name { get; }

			/// <summary>
			/// Evaluate the given board state and return a value from 0-1, where 0 is bad and 1 is good
			/// </summary>
			double Evaluate(BoardState board);
		}

		public class TightPlacementWTSUF : WTSUtilityFunction
		{
			public string Name => $"Tight({_doubler}@{_focusPower})";

			private readonly bool _doubler;
			private readonly double _focusPower;

			public TightPlacementWTSUF(bool doubler, double focusPower)
			{
				_doubler = doubler;
				_focusPower = focusPower;
			}

			public double Evaluate(BoardState board)
			{
				TightPlacementStrategy.CalculateScore(board, _doubler, out var score);

				double result;
				if (_doubler)
					result = 1 - score / (Math.Pow(2, 8) * (BoardState.Width + BoardState.Height));
				else
					result = 1 - score / (double)(BoardState.Width * BoardState.Height * 2);

				if (result > 1 || result < 0)
					throw new Exception();

				return Math.Pow(result, _focusPower);
			}
		}
	}
}