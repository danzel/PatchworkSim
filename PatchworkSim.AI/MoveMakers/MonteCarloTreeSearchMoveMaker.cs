using System;
using System.Collections.Generic;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;

namespace PatchworkSim.AI.MoveMakers
{
	/// <summary>
	/// A MCTS move maker that places the pieces as it goes, which should stop it thinking it can purchase a piece that it cannot.
	/// Rollouts are still ran without piece placing, so it may not perform ideally
	/// </summary>
	public class MonteCarloTreeSearchMoveMaker : IMoveDecisionMaker
	{
		public string Name => $"MCTS({_mcts.Iterations}+{_mcts.RolloutMoveMaker.Name}+{_mcts.BoardEvaluator.Name}@{_mcts.MaxChildrenPerPiece})";

		public readonly StoredPlacementStrategy PlacementStrategy = new StoredPlacementStrategy();

		private readonly MonteCarloTreeSearchPlacementExpander _mcts;

		public MonteCarloTreeSearchMoveMaker(int iterations, IMoveDecisionMaker rolloutMoveMaker, IBoardEvaluator boardEvaluator, int maxChildrenPerPiece)
		{
			_mcts = new MonteCarloTreeSearchPlacementExpander(iterations, rolloutMoveMaker, boardEvaluator, maxChildrenPerPiece);
		}

		public void MakeMove(SimulationState state)
		{
			var root = _mcts.PerformMCTS(state);

			var bestChild = _mcts.FindBestChild(root);
			//DumpChildren(root);

			//First move is guaranteed to be a move, future ones may be placements
			if (bestChild.PieceToPurchase.HasValue)
				state.PerformPurchasePiece(bestChild.PieceToPurchase.Value);
			else
				state.PerformAdvanceMove();

			if (bestChild.Children.Count > 0)
			{
				//While the move is a placement, queue it up to be done
				bestChild = _mcts.FindBestChild(bestChild);
				while (bestChild.PlaceBitmap != null)
				{
					PlacementStrategy.EnqueuePlacement(new Preplacement(bestChild.PlaceBitmap, bestChild.PlaceX, bestChild.PlaceY));

					if (bestChild.Children.Count > 0)
						bestChild = _mcts.FindBestChild(bestChild);
					else
						break;
				}
			}

			MonteCarloTreeSearch<SearchNode>.NodePool.Value.ReturnAll();
		}

		protected class MonteCarloTreeSearchPlacementExpander : MonteCarloTreeSearch<SearchNode>
		{
			public readonly IBoardEvaluator BoardEvaluator;
			public readonly int MaxChildrenPerPiece;

			public MonteCarloTreeSearchPlacementExpander(int iterations, IMoveDecisionMaker rolloutMoveMaker, IBoardEvaluator boardEvaluator, int maxChildrenPerPiece) : base(iterations, rolloutMoveMaker)
			{
				MaxChildrenPerPiece = maxChildrenPerPiece;
				BoardEvaluator = boardEvaluator;
			}

			protected override void Expand(SearchNode root)
			{
				if (root.State.PieceToPlace != null)
				{
					ExpandByPlacing(root);
				}
				else
				{
					ExpandByMove(root);
				}
			}

			private readonly List<double> _expandPlacingUtility = new List<double>(4);

			private void ExpandByPlacing(SearchNode root)
			{
				// This code mostly taken from WeightedTreeSearchPreplacer

				_expandPlacingUtility.Clear();

				var piece = root.State.PieceToPlace;
				var isFirstPiece = root.State.PlayerBoardState[root.State.ActivePlayer].IsEmpty;
				var board = root.State.PlayerBoardState[root.State.PieceToPlacePlayer];
				var children = root.Children;


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
							if (board.CanPlace(bitmap, x, y))
							{
								//evaluate child nodes
								var copy = board;
								copy.Place(bitmap, x, y);
								var utility = BoardEvaluator.Evaluate(copy);

								//Insertion sort us in to the children list
								SearchNode child = null;
								for (var i = 0; i < children.Count; i++)
								{
									//We should be here
									if (utility > _expandPlacingUtility[i])
									{
										if (children.Count == MaxChildrenPerPiece)
										{
											//Children is already full, grab the last one and reuse it
											child = children[MaxChildrenPerPiece - 1];
											children.RemoveAt(MaxChildrenPerPiece - 1);
											children.Insert(i, child);
											_expandPlacingUtility.Insert(i, utility);
										}
										else
										{
											//Not full yet, just insert us here
											child = NodePool.Value.Get();
											children.Insert(i, child);
											_expandPlacingUtility.Insert(i, utility);
										}
										break;
									}
								}

								//Didn't find a place to put us, but children isn't full yet
								if (child == null && children.Count < MaxChildrenPerPiece)
								{
									//Insert us last
									child = NodePool.Value.Get();
									children.Add(child);
									_expandPlacingUtility.Add(utility);
								}

								//Populate the child
								if (child != null)
								{
									child.PlaceBitmap = bitmap;
									child.PlaceX = x;
									child.PlaceY = y;
								}
							}
						}
					}
				}

				//Properly apply the placement in the children
				for (var i = 0; i < children.Count; i++)
				{
					var c = children[i];
					root.State.CloneTo(c.State);
					c.State.PerformPlacePiece(c.PlaceBitmap, c.PlaceX, c.PlaceY);
					c.Parent = root;
				}
			}

			private void ExpandByMove(SearchNode root)
			{
				//Advance
				{
					var node = NodePool.Value.Get();

					root.State.CloneTo(node.State);
					node.State.PerformAdvanceMove();

					node.Parent = root;
					root.Children.Add(node);
				}

				//Purchase
				for (var i = 0; i < 3; i++)
				{
					if (Helpers.ActivePlayerCanPurchasePiece(root.State, Helpers.GetNextPiece(root.State, i)))
					{
						var node = NodePool.Value.Get();

						root.State.CloneTo(node.State);
						var pieceIndex = node.State.NextPieceIndex + i;
						node.State.PerformPurchasePiece(pieceIndex);

						node.Parent = root;
						node.PieceToPurchase = pieceIndex;
						root.Children.Add(node);
					}
				}
			}
		}

		protected class SearchNode : MCTSNode<SearchNode>, IPoolableItem
		{
			/// <summary>
			/// The piece that was purchased to arrive at this node, or null for advance
			/// </summary>
			public int? PieceToPurchase;

			//TODO: Do we want to use these placements or re-calculate the placement at the end of it?
			public PieceBitmap PlaceBitmap;

			public int PlaceX;
			public int PlaceY;

			public SearchNode() : base(4) //TODO: Number will need to be bigger maybe
			{
			}

			public override void Expand()
			{
				throw new Exception("This is implemented in MonteCarloTreeSearchPlacementExpander.Expand");
			}

			public override void Reset()
			{
				base.Reset();

				PieceToPurchase = null;
				PlaceBitmap = null;
				PlaceX = -1;
				PlaceY = -1;
			}
		}
	}
}