using System;
using System.Threading;

namespace PatchworkSim.AI.MoveMakers
{
	public class MoveOnlyMinimaxWithAlphaBetaPruningMoveMaker : IMoveDecisionMaker
	{
		public string Name => $"MinimaxAlphaBeta({_maxSearchDepth})";

		private readonly int _maxSearchDepth;

		private readonly ThreadLocal<SingleThreadedStackPool<SimulationState>> _pool = new ThreadLocal<SingleThreadedStackPool<SimulationState>>(() => new SingleThreadedStackPool<SimulationState>(), false);

		public MoveOnlyMinimaxWithAlphaBetaPruningMoveMaker(int maxSearchDepth)
		{
			_maxSearchDepth = maxSearchDepth;
		}

		public void MakeMove(SimulationState state)
		{
			AlphaBeta(state, _maxSearchDepth, int.MinValue, int.MaxValue, state.ActivePlayer, out var bestMove);

			if (!bestMove.HasValue)
				state.PerformAdvanceMove();
			else
				state.PerformPurchasePiece(state.NextPieceIndex + bestMove.Value);
		}

		//https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning#Pseudocode
		private int AlphaBeta(SimulationState parentState, int depth, int alpha, int beta, int maximizingPlayer, out int? bestMove)
		{
			bestMove = null;

			//We deviate from strict depth limited minimax here.
			//We want to ensure that both players get the same amount(ish) of turns, otherwise when we get one more turn than our opponent we will think that is better.
			//So to ensure fairness, we don't terminate a search until it is the maximizing players turn again, this ensures the opponent has had time to respond to our move
			//TODO: This isn't totally perfect, if the last move we made gave us an extra move, we've got a turn advantage and it is our turn, so we'll terminate
			//TODO: I roughly tested making this continue until the last player was the non-maximizing player also, this made the search 2.5x as long and it seemed slightly weaker
			//TODO: Adding the ActivePlayer check vs not having it improves strength a lot, but makes the search runtime 2x
			if ((depth <= 0 && parentState.ActivePlayer == maximizingPlayer) || parentState.GameHasEnded)
				return Evaluate(parentState, maximizingPlayer);

			var shouldMaximize = maximizingPlayer == parentState.ActivePlayer;
			int bestValue = shouldMaximize ? int.MinValue : int.MaxValue;

			//TODO: Can run faster if we try the best moves first
			//foreach child

			//Try buy all possible pieces
			for (var i = 0; i < 3; i++)
			{
				var piece = Helpers.GetNextPiece(parentState, i);
				if (Helpers.ActivePlayerCanPurchasePiece(parentState, piece))
				{
					var state = _pool.Value.Get();
					state.Pieces.Clear();
					parentState.CloneTo(state);
					state.Fidelity = SimulationFidelity.NoPiecePlacing;
					state.PerformPurchasePiece(state.NextPieceIndex + i);
					var v = AlphaBeta(state, depth -1, alpha, beta, maximizingPlayer);
					_pool.Value.Return(state);
					if (shouldMaximize)
					{
						if (v > bestValue)
							bestMove = i;
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

			//Advance
			if (bestMove != null) // Don't need to consider it if we can't purchase any pieces, we are definitely going to advance
			{
				var state = _pool.Value.Get();
				state.Pieces.Clear();
				parentState.CloneTo(state);
				state.Fidelity = SimulationFidelity.NoPiecePlacing;
				state.PerformAdvanceMove();
				//Decrease alpha by 1 (if possible) so we can identify draws. We favor doing an advance in a draw, but we evaluate purchases first
				//This gives us the same results as if we were evaluating advance first, but performance is better
				//This makes us stronger than favoring buying pieces in a draw (Which probably implies our Evaluate method is incorrect?), but costs slight runtime performance
				var v = AlphaBeta(state, depth - 1, (alpha == int.MinValue ? int.MinValue : alpha - 1), beta, maximizingPlayer);
				_pool.Value.Return(state);
				if (shouldMaximize)
				{
					if (v >= bestValue)
						bestMove = null;
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

			return bestValue;
		}


		//https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning#Pseudocode
		private int AlphaBeta(SimulationState parentState, int depth, int alpha, int beta, int maximizingPlayer)
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
			int bestValue = shouldMaximize ? int.MinValue : int.MaxValue;

			//TODO: Can run faster if we try the best moves first
			//foreach child

			//Try buy all possible pieces
			for (var i = 0; i < 3; i++)
			{
				var piece = Helpers.GetNextPiece(parentState, i);
				if (Helpers.ActivePlayerCanPurchasePiece(parentState, piece))
				{
					var state = _pool.Value.Get();
					state.Pieces.Clear();
					parentState.CloneTo(state);
					state.Fidelity = SimulationFidelity.NoPiecePlacing;
					state.PerformPurchasePiece(state.NextPieceIndex + i);
					var v = AlphaBeta(state, depth - 1, alpha, beta, maximizingPlayer);
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

			//Advance
			{
				var state = _pool.Value.Get();
				state.Pieces.Clear();
				parentState.CloneTo(state);
				state.Fidelity = SimulationFidelity.NoPiecePlacing;
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

			return bestValue;
		}

		/// <summary>
		/// Score the simulation for the given player
		/// </summary>
		private int Evaluate(SimulationState state, int maximizingPlayer)
		{
			if (state.GameHasEnded)
			{
				if (maximizingPlayer == state.WinningPlayer)
					return int.MaxValue;
				else
					return int.MinValue;
			}

			return Helpers.EstimateEndgameValue(state, maximizingPlayer) - Helpers.EstimateEndgameValue(state, maximizingPlayer == 0 ? 1 : 0);
		}
	}
}
