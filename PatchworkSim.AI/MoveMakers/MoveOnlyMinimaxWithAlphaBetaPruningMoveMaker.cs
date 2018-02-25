using System;
using System.Runtime.CompilerServices;
using System.Threading;
using PatchworkSim.AI.MoveMakers.UtilityCalculators;

namespace PatchworkSim.AI.MoveMakers
{
	public class MoveOnlyMinimaxWithAlphaBetaPruningMoveMaker : IMoveDecisionMaker
	{
		public string Name => $"MinimaxAlphaBeta({_maxSearchDepth})";

		private readonly int _maxSearchDepth;
		private readonly IUtilityCalculator _calculator;

		private readonly ThreadLocal<SingleThreadedStackPool<SimulationState>> _pool = new ThreadLocal<SingleThreadedStackPool<SimulationState>>(() => new SingleThreadedStackPool<SimulationState>(), false);

		private readonly SingleThreadedStackPool<SimulationState> _thisThreadPool;

		public MoveOnlyMinimaxWithAlphaBetaPruningMoveMaker(int maxSearchDepth, IUtilityCalculator calculator)
		{
			_thisThreadPool = _pool.Value;
			_maxSearchDepth = maxSearchDepth;
			_calculator = calculator;
		}

		public void MakeMove(SimulationState state)
		{
			var bestMove = AlphaBeta(state);

			if (bestMove == -1)
				state.PerformAdvanceMove();
			else
				state.PerformPurchasePiece(state.NextPieceIndex + bestMove);
		}

		private static void InsertionSort(ref FixedArray4Int possibleMoves, ref FixedArray4Double possibleMoveValues, ref int possibleMovesAmount, int move, double value)
		{
			bool inserted = false;
			for (var j = 0; j < possibleMovesAmount; j++)
			{
				if (value > possibleMoveValues[j])
				{
					//Move existing ones back
					for (var k = possibleMovesAmount; k > j; k--)
					{
						possibleMoves[k] = possibleMoves[k - 1];
						possibleMoveValues[k] = possibleMoveValues[k - 1];
					}

					possibleMoves[j] = move;
					possibleMoveValues[j] = value;
					inserted = true;
					break;
				}
			}

			if (!inserted)
			{
				possibleMoves[possibleMovesAmount] = move;
				possibleMoveValues[possibleMovesAmount] = value;
			}

			possibleMovesAmount++;
		}

		//https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning#Pseudocode
		private int AlphaBeta(SimulationState parentState)
		{
			var maximizingPlayer = parentState.ActivePlayer;
			int alpha = int.MinValue;
			int beta = int.MaxValue;

			int bestMove = -1;
			int bestValue = Int32.MinValue;

			//0-2 + -1 for advance
			var possibleMoves = new FixedArray4Int();
			var possibleMoveValues = new FixedArray4Double();
			int possibleMovesAmount = 0;

			//Insertion sort the possible moves in to possibleMoves(Weights)
			//Buying stuff
			for (var i = 0; i < 3; i++)
			{
				var piece = Helpers.GetNextPiece(parentState, i);
				if (Helpers.ActivePlayerCanPurchasePiece(parentState, piece))
				{
					var value = _calculator.CalculateValueOfPurchasing(parentState, parentState.NextPieceIndex + i, piece);
					InsertionSort(ref possibleMoves, ref possibleMoveValues, ref possibleMovesAmount, i, value);
				}
			}
			//Advance
			{
				var value = _calculator.CalculateValueOfAdvancing(parentState);
				InsertionSort(ref possibleMoves, ref possibleMoveValues, ref possibleMovesAmount, -1, value);
			}

			//foreach child
			var state = _thisThreadPool.Get();
			for (var m = 0; m < possibleMovesAmount; m++)
			{
				var move = possibleMoves[m];
				parentState.CloneTo(state);
				state.Fidelity = SimulationFidelity.NoPiecePlacing;

				if (move == -1)
					state.PerformAdvanceMove();
				else
					state.PerformPurchasePiece(state.NextPieceIndex + move);

				var v = AlphaBeta(state, _maxSearchDepth - 1, alpha, beta, maximizingPlayer);

				if (v > bestValue)
					bestMove = move;
				bestValue = Math.Max(bestValue, v);
				alpha = Math.Max(alpha, bestValue);
				if (beta <= alpha)
					break;
			}

			_thisThreadPool.Return(state);
			return bestMove;
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



			//0-2 + -1 for advance
			var possibleMoves = new FixedArray4Int();
			var possibleMoveValues = new FixedArray4Double();
			int possibleMovesAmount = 0;

			//Insertion sort the possible moves in to possibleMoves(Weights)
			//Buying stuff
			for (var i = 0; i < 3; i++)
			{
				var piece = Helpers.GetNextPiece(parentState, i);
				if (Helpers.ActivePlayerCanPurchasePiece(parentState, piece))
				{
					var value = _calculator.CalculateValueOfPurchasing(parentState, parentState.NextPieceIndex + i, piece);
					InsertionSort(ref possibleMoves, ref possibleMoveValues, ref possibleMovesAmount, i, value);
				}
			}
			//Advance
			{
				var value = _calculator.CalculateValueOfAdvancing(parentState);
				InsertionSort(ref possibleMoves, ref possibleMoveValues, ref possibleMovesAmount, -1, value);
			}

			//foreach child
			var state = _thisThreadPool.Get();
			for (var m = 0; m < possibleMovesAmount; m++)
			{
				var move = possibleMoves[m];
				parentState.CloneTo(state);

				if (move == -1)
					state.PerformAdvanceMove();
				else
					state.PerformPurchasePiece(state.NextPieceIndex + move);

				var v = AlphaBeta(state, depth - 1, alpha, beta, maximizingPlayer);

				if (shouldMaximize)
				{
					bestValue = Math.Max(bestValue, v);
					alpha = Math.Max(alpha, bestValue);
					if (beta <= alpha)
					{
						_thisThreadPool.Return(state);
						return bestValue;
					}
				}
				else
				{
					bestValue = Math.Min(bestValue, v);
					beta = Math.Min(beta, bestValue);
					if (beta <= alpha)
					{
						_thisThreadPool.Return(state);
						return bestValue;
					}
				}
			}

			_thisThreadPool.Return(state);
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