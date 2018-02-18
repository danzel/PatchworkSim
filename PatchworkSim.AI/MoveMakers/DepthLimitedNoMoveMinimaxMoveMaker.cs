using System;

namespace PatchworkSim.AI.MoveMakers
{
	public class DepthLimitedNoMoveMinimaxMoveMaker : BaseUtilityMoveMaker
	{
		public override string Name => $"Minimax({_maxSearchDepth})";

		private readonly int _maxSearchDepth;

		public DepthLimitedNoMoveMinimaxMoveMaker(int maxSearchDepth)
		{
			_maxSearchDepth = maxSearchDepth;
		}

		protected override double CalculateValueOfAdvancing(SimulationState state)
		{
			var maximizingPlayer = state.ActivePlayer;

			state = state.Clone();
			state.Fidelity = SimulationFidelity.NoPiecePlacing;
			state.PerformAdvanceMove();

			return Minimax(state, maximizingPlayer, _maxSearchDepth);
		}

		protected override double CalculateValue(SimulationState state, int pieceIndex, PieceDefinition piece)
		{
			var maximizingPlayer = state.ActivePlayer;

			state = state.Clone();
			state.Fidelity = SimulationFidelity.NoPiecePlacing;
			state.PerformPurchasePiece(pieceIndex);

			return Minimax(state, maximizingPlayer, _maxSearchDepth);
		}

		//https://en.wikipedia.org/wiki/Minimax#Pseudocode
		private double Minimax(SimulationState parentState, int maximizingPlayer, int depth)
		{
			if (depth == 0 || parentState.GameHasEnded)
				return Evaluate(parentState, maximizingPlayer);

			var shouldMaximize = maximizingPlayer == parentState.ActivePlayer;
			double bestValue = shouldMaximize ? double.MinValue : double.MaxValue;

			//foreach child
			//Advance
			{
				var state = parentState.Clone();
				state.PerformAdvanceMove();
				var v = Minimax(state, maximizingPlayer, depth - 1);
				if (shouldMaximize)
					bestValue = Math.Max(bestValue, v);
				else
					bestValue = Math.Min(bestValue, v);
			}

			//Try buy all possible pieces
			for (var i = 0; i < 3; i++)
			{
				var piece = Helpers.GetNextPiece(parentState, i);
				if (Helpers.ActivePlayerCanPurchasePiece(parentState, piece))
				{
					var state = parentState.Clone();
					state.PerformPurchasePiece(state.NextPieceIndex + i);
					var v = Minimax(state, maximizingPlayer, depth - 1);
					if (shouldMaximize)
						bestValue = Math.Max(bestValue, v);
					else
						bestValue = Math.Min(bestValue, v);
				}
			}

			return bestValue;
		}

		/// <summary>
		/// Score the simulation for the given player
		/// </summary>
		private double Evaluate(SimulationState state, int maximizingPlayer)
		{
			if (state.GameHasEnded)
			{
				if (maximizingPlayer == state.WinningPlayer)
					return double.MaxValue;
				else
					return double.MinValue;
			}

			return Helpers.EstimateEndgameValue(state, maximizingPlayer) - Helpers.EstimateEndgameValue(state, maximizingPlayer == 0 ? 1 : 0);
		}
	}
}
