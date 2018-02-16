using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers
{
	public class WeightedTreeSearchPreplacer : IPreplacer
	{
		public string Name => $"WeightedTreeSearch({_iterations}@{_maxBranching}+{_utilityFunction.Name})";

		private readonly WTSUtilityFunction _utilityFunction;
		private readonly int _iterations;
		private readonly int _maxBranching;

		private readonly Random _random = new Random(0);

		private static readonly ThreadLocal<SearchNodePool> NodePool = new ThreadLocal<SearchNodePool>(() => new SearchNodePool(), false);

		public WeightedTreeSearchPreplacer(WTSUtilityFunction utilityFunction, int iterations, int maxBranching)
		{
			_utilityFunction = utilityFunction;
			_iterations = iterations;
			_maxBranching = maxBranching;
		}

		public Preplacement Preplace(BoardState board, List<PieceDefinition> plannedFuturePieces)
		{
			//TODO: When board is empty we should remove rotations/mirrors of the placements (not worth it on future ones)

			var root = NodePool.Value.Get(_maxBranching);
			root.Board = board;
			root.Bitmap = null;
			root.X = -1;
			root.Y = -1;
			root.Depth = -1;

			Expand(root, plannedFuturePieces[0], board.IsEmpty);

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

				//TODO: What to do when we hit the bottom?
				if (leaf.Depth + 1 < plannedFuturePieces.Count)
					Expand(leaf, plannedFuturePieces[leaf.Depth + 1], false);
			}

			if (root.Children.Count == 0)
				throw new Exception("Couldnt find any placement for the first piece");

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

			var result = new Preplacement(bestChild.Bitmap, bestChild.X, bestChild.Y);

			NodePool.Value.ReturnAll();

			return result;
			//NodePool.Dump();
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

		private void Expand(SearchNode node, PieceDefinition piece, bool isFirstPiece)
		{
			node.HasBeenExpanded = true;
			var children = node.Children;

			//Exhaustively place it and make new child nodes
			for (var index = 0; index < piece.PossibleOrientations.Length; index++)
			{
				var bitmap = piece.PossibleOrientations[index];
				var searchWidth = BoardState.Width - bitmap.Width + 1;
				var searchHeight = BoardState.Height - bitmap.Height + 1;

				//If this is the first piece, remove mirrors/rotations from the children
				if (isFirstPiece)
				{
					//TODO: This doesn't stop diagonal mirrors
					searchWidth = (BoardState.Width - bitmap.Width) / 2 + 1;
					searchHeight = (BoardState.Height - bitmap.Height) / 2 + 1;
				}

				for (int x = 0; x < searchWidth; x++)
				{
					for (int y = 0; y < searchHeight; y++)
					{
						if (node.Board.CanPlace(bitmap, x, y))
						{
							//evaluate child nodes
							var copy = node.Board;
							copy.Place(bitmap, x, y);
							var utility = _utilityFunction.Evaluate(copy);

							//Insertion sort us in to the children list
							SearchNode child = null;
							for (var i = 0; i < children.Count; i++)
							{
								//We should be here
								if (utility > children[i].Utility)
								{
									if (children.Count == _maxBranching)
									{
										//Children is already full, grab the last one and reuse it
										child = children[_maxBranching - 1];
										children.RemoveAt(_maxBranching - 1);
										children.Insert(i, child);
									}
									else
									{
										//Not full yet, just insert us here
										child = NodePool.Value.Get(_maxBranching);
										children.Insert(i, child);
									}
									break;
								}
							}

							//Didn't find a place to put us, but children isn't full yet
							if (child == null && children.Count < _maxBranching)
							{
								//Insert us last
								child = NodePool.Value.Get(_maxBranching);
								children.Add(child);
							}

							//Populate the child
							if (child != null)
							{
								child.Board = copy;
								child.Bitmap = bitmap;
								child.X = x;
								child.Y = y;
								child.Depth = node.Depth + 1;
								child.Utility = utility;
							}
						}
					}
				}
			}

			node.ChildrenUtilitySum = 0;
			for (var i = 0; i < node.Children.Count; i++)
			{
				var c = node.Children[i];
				node.ChildrenUtilitySum += c.Utility;
			}
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
				Children = new List<SearchNode>(maxChildCount);
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
		}

		class SearchNodePool
		{
			private readonly List<SearchNode> _searchNodePool = new List<SearchNode>();
			private int _getIndex;

			//public int Allocations = 0;
			//public int Returns = 0;
			//public int FetchedFromPool = 0;

			internal SearchNode Get(int maxChildCount)
			{
				if (_getIndex < _searchNodePool.Count)
				{
					var index = _getIndex;
					_getIndex++;
					//FetchedFromPool++;
					return _searchNodePool[index];
				}

				//Allocations++;
				var res = new SearchNode(maxChildCount);
				_searchNodePool.Add(res);
				_getIndex++;
				return res;
			}

			internal void ReturnAll()
			{
				for (var i = 0; i < _getIndex; i++)
				{
					//Returns++;
					var child = _searchNodePool[i];
					child.Children.Clear();
					child.HasBeenExpanded = false;
				}

				_getIndex = 0;
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
