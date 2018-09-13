using System;
using System.Linq;

namespace PatchworkSim.AI.MoveMakers
{
	/// <summary>
	/// A MCTS based move maker that runs the simulation in NoPiecePlacing.
	/// </summary>
	public abstract class BaseMoveOnlyMonteCarloTreeSearchMoveMaker : IMoveDecisionMaker
	{
		public abstract string Name { get; }

		protected readonly MonteCarloTreeSearch<SearchNode> Mcts;

		protected BaseMoveOnlyMonteCarloTreeSearchMoveMaker(int iterations, IMoveDecisionMaker rolloutMoveMaker = null)
		{
			Mcts = new MonteCarloTreeSearch<SearchNode>(iterations, rolloutMoveMaker);
		}

		public abstract void MakeMove(SimulationState state);

		protected void DumpChildren(SearchNode root)
		{
			Console.WriteLine(string.Join(", ", root.Children.Select(s => s.GetDebugText(root.State))));
		}

		public class SearchNode : MCTSNode<SearchNode>, IPoolableItem
		{
			/// <summary>
			/// The piece that was purchased to arrive at this node, or null for advance
			/// </summary>
			public int? PieceToPurchase;


			public SearchNode() : base(4)
			{
			}

			public override void Expand(double progressiveBiasWeight)
			{
#if DEBUG
				if (Children.Count != 0)
					throw new Exception("Cannot expand an already expanded node");
				if (IsGameEnd)
					throw new Exception("Cannot expand a GameEnd node");
#endif
				//Advance
				{
					var node = MonteCarloTreeSearch<SearchNode>.NodePool.Value.Get();

					State.CloneTo(node.State);
					node.State.Fidelity = SimulationFidelity.NoPiecePlacing;
					node.State.PerformAdvanceMove();

					node.ProgressiveBias = progressiveBiasWeight == 0 ? 0 : progressiveBiasWeight * UtilityCalculators.TuneableByBoardPositionUtilityCalculator.Tuning1.CalculateValueOfAdvancing(State);

					node.Parent = this;
					Children.Add(node);
				}

				//Purchase
				for (var i = 0; i < 3; i++)
				{
					//This cares if they can actually place the piece only when expanding the root node
					if (Helpers.ActivePlayerCanPurchasePiece(State, Helpers.GetNextPiece(State, i)))
					{
						var node = MonteCarloTreeSearch<SearchNode>.NodePool.Value.Get();

						State.CloneTo(node.State);
						node.State.Fidelity = SimulationFidelity.NoPiecePlacing;
						var pieceIndex = node.State.NextPieceIndex + i;
						node.State.PerformPurchasePiece(pieceIndex);

						node.ProgressiveBias = progressiveBiasWeight == 0 ? 0 : progressiveBiasWeight * UtilityCalculators.TuneableByBoardPositionUtilityCalculator.Tuning1.CalculateValueOfPurchasing(State, pieceIndex, Helpers.GetNextPiece(State, i));

						node.Parent = this;
						node.PieceToPurchase = pieceIndex;
						Children.Add(node);
					}
				}
			}

			public string GetDebugText(SimulationState parentState)
			{
				if (PieceToPurchase.HasValue)
				{
					return $"Purchase ({PieceDefinition.AllPieceDefinitions[parentState.Pieces[PieceToPurchase.Value % parentState.Pieces.Count]].Name}) {Value}/{VisitCount}";
				}
				else
				{
					return $"Advance {Value}/{VisitCount}";
				}
			}

			public override void Reset()
			{
				base.Reset();

				PieceToPurchase = null;
			}
		}
	}
}
