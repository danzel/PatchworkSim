using System;
using System.Collections.Generic;
using System.Threading;
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

		private static readonly SearchNodePool NodePool = new SearchNodePool();

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

			var root = NodePool.Get(_maxBranching);
			root.Board = board;
			root.Bitmap = null;
			root.X = -1;
			root.Y = -1;
			root.Depth = -1;

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

				root.RecursiveReturnToPool();
				//NodePool.Dump();
				return true;
			}
			else
			{
				bitmap = null;
				x = 0;
				y = 0;

				root.RecursiveReturnToPool();
				//NodePool.Dump();
				return false;
			}
		}

		private SearchNode Select(SearchNode currentRoot)
		{
			while (currentRoot.HasBeenExpanded) //Look for a leaf node (one we haven't expanded yet)
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

		private readonly List<SearchNode> _tempChildren = new List<SearchNode>(BoardState.Width * BoardState.Height);
		private void Expand(SearchNode node, PieceDefinition piece)
		{
			node.HasBeenExpanded = true;

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
							var child = NodePool.Get(_maxBranching);
							child.Board = copy;
							child.Bitmap = bitmap;
							child.X = x;
							child.Y = y;
							child.Depth = node.Depth + 1;

							//evaluate child nodes
							child.Utility = _utilityFunction.Evaluate(copy);
							_tempChildren.Add(child);
						}
					}
				}
			}

			//TODO: If this is the first piece, remove mirrors/rotations from the children

			//TODO: Could have smaller children lists in the nodes if we sorted and pruned here in our own list, then copied the elements over

			//Sort child nodes
			_tempChildren.Sort();

			//Remove excess child nodes
			if (_tempChildren.Count > _maxBranching)
			{
				for (var i = _maxBranching; i < _tempChildren.Count; i++)
				{
					NodePool.Return(_tempChildren[i]);
				}
				//node.Children.RemoveRange(_maxBranching, node.Children.Count - _maxBranching);
			}

			//Move them to the node
			for (var i = 0; i < Math.Min(_maxBranching, _tempChildren.Count); i++)
				node.Children.Add(_tempChildren[i]);

			node.ChildrenUtilitySum = 0;
			for (var i = 0; i < node.Children.Count; i++)
			{
				var c = node.Children[i];
				node.ChildrenUtilitySum += c.Utility;
			}

			_tempChildren.Clear();
		}


		class SearchNode : IComparable<SearchNode>
		{
			public BoardState Board;
			public PieceBitmap Bitmap;
			public int X, Y;
			public int Depth;

			public double Utility;
			public double ChildrenUtilitySum;
			public bool HasBeenExpanded = false;
			public readonly List<SearchNode> Children;

			public SearchNode(int maxChildCount)
			{
				Children = new List<SearchNode>(4);
			}

			public int CompareTo(SearchNode other)
			{
				return -Utility.CompareTo(other.Utility);
			}

			public int CalculateMaxChildDepth()
			{
				var deepest = Depth;
				for (var i = 0; i < Children.Count; i++)
				{
					var c = Children[i];
					deepest = Math.Max(deepest, c.CalculateMaxChildDepth());
				}

				return deepest;
			}

			public void RecursiveReturnToPool()
			{
				for (var i = 0; i < Children.Count; i++)
				{
					var c = Children[i];
					c.RecursiveReturnToPool();
				}
				NodePool.Return(this);
			}
		}

		class SearchNodePool
		{
			private readonly ThreadLocal<Stack<SearchNode>> _searchNodePool = new ThreadLocal<Stack<SearchNode>>(() => new Stack<SearchNode>(), false);

			//public int Allocations = 0;
			//public int Returns = 0;
			//public int FetchedFromPool = 0;

			internal SearchNode Get(int maxChildCount)
			{
				if (_searchNodePool.Value.Count > 0)
				{
					//FetchedFromPool++;
					return _searchNodePool.Value.Pop();
				}

				//Allocations++;
				return new SearchNode(maxChildCount);
			}

			internal void Return(SearchNode value)
			{
				//Returns++;
				value.Children.Clear();
				value.HasBeenExpanded = false;

				_searchNodePool.Value.Push(value);
			}
			/*
			public void Dump()
			{
				Console.WriteLine($"Allocations {Allocations}");
				Console.WriteLine($"Returns {Returns}");
				Console.WriteLine($"FetchedFromPool {FetchedFromPool}");
			}*/
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