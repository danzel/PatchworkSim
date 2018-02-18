using System;
using System.Threading;

namespace PatchworkSim.AI.MoveMakers
{
	public class MoveOnlyMinimaxWithAlphaBetaPruningMoveMaker : BaseUtilityMoveMaker
	{
		public override string Name => $"MinimaxAlphaBeta({_maxSearchDepth})";

		private readonly int _maxSearchDepth;

		private readonly ThreadLocal<SingleThreadedStackPool<SimulationState>> _pool = new ThreadLocal<SingleThreadedStackPool<SimulationState>>(() => new SingleThreadedStackPool<SimulationState>(), false);

		public MoveOnlyMinimaxWithAlphaBetaPruningMoveMaker(int maxSearchDepth)
		{
			_maxSearchDepth = maxSearchDepth;
		}

		protected override double CalculateValueOfAdvancing(SimulationState baseState)
		{
			var state = _pool.Value.Get();
			state.Pieces.Clear();
			baseState.CloneTo(state);
			state.Fidelity = SimulationFidelity.NoPiecePlacing;
			state.PerformAdvanceMove();

			var res = AlphaBeta(state, _maxSearchDepth, double.MinValue, double.MaxValue, baseState.ActivePlayer);
			_pool.Value.Return(state);
			return res;
		}

		protected override double CalculateValue(SimulationState baseState, int pieceIndex, PieceDefinition piece)
		{
			var state = _pool.Value.Get();
			state.Pieces.Clear();
			baseState.CloneTo(state);
			state.Fidelity = SimulationFidelity.NoPiecePlacing;
			state.PerformPurchasePiece(pieceIndex);

			var res =  AlphaBeta(state, _maxSearchDepth, double.MinValue, double.MaxValue, baseState.ActivePlayer);
			_pool.Value.Return(state);
			return res;
		}

		//https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning#Pseudocode
		private double AlphaBeta(SimulationState parentState, int depth, double alpha, double beta, int maximizingPlayer)
		{
			//We deviate from strict depth limited minimax here.
			//We want to ensure that both players get the same amount(ish) of turns, otherwise when we get one more turn than our opponent we will think that is better.
			//So to ensure fairness, we don't terminate a search until it is the maximizing players turn again, this ensures the opponent has had time to respond to our move
			//TODO: This isn't totally perfect, if the last move we made gave us an extra move, we've got a turn advantage and it is our turn, so we'll terminate
			//TODO: I roughly tested making this continue until the last player was the non-maximizing player also, this made the search 2.5x as long and it seemed slightly weaker
			//TODO: Adding the ActivePlayer check vs not having it improves strength a lot, but makes the search runtime 2x
			if ((depth <= 0 && parentState.ActivePlayer == maximizingPlayer) || parentState.GameHasEnded)
				return Evaluate(parentState, maximizingPlayer);

			var shouldMaximize = maximizingPlayer == parentState.ActivePlayer;
			double bestValue = shouldMaximize ? double.MinValue : double.MaxValue;

			//foreach child
			//Advance
			{
				var state = _pool.Value.Get();
				state.Pieces.Clear();
				parentState.CloneTo(state);
				state.PerformAdvanceMove();
				var v = AlphaBeta(state, depth - 1, alpha, beta, maximizingPlayer);
				_pool.Value.Return(state);
				if (shouldMaximize)
				{
					bestValue = Math.Max(bestValue, v);
					alpha = Math.Max(alpha, bestValue);
					if (beta <= alpha)
						return bestValue;
				}
				else
				{
					bestValue = Math.Min(bestValue, v);
					beta = Math.Min(beta, bestValue);
					if (beta <= alpha)
						return bestValue;
				}
			}

			//Try buy all possible pieces
			for (var i = 0; i < 3; i++)
			{
				var piece = Helpers.GetNextPiece(parentState, i);
				if (Helpers.ActivePlayerCanPurchasePiece(parentState, piece))
				{
					var state = _pool.Value.Get();
					state.Pieces.Clear();
					parentState.CloneTo(state);
					state.PerformPurchasePiece(state.NextPieceIndex + i);
					var v = AlphaBeta(state, depth -1, alpha, beta, maximizingPlayer);
					_pool.Value.Return(state);
					if (shouldMaximize)
					{
						bestValue = Math.Max(bestValue, v);
						alpha = Math.Max(alpha, bestValue);
						if (beta <= alpha)
							break;
					}
					else
					{
						bestValue = Math.Min(bestValue, v);
						beta = Math.Min(beta, bestValue);
						if (beta <= alpha)
							break;
					}
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
