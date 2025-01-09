using PatchworkSim.AI.MoveMakers.UtilityCalculators;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace PatchworkSim.AI.MoveMakers;

public class MoveOnlyAlphaBetaWithPreplacerMoveMaker : IMoveDecisionMaker
{
	public string Name => $"MO-AlphaBetaPreplacer({_maxSearchDepth})";

	private readonly int _maxSearchDepth;
	private readonly IUtilityCalculator _calculator;
	private readonly PreplacerStrategy _preplacer;

	private readonly ThreadLocal<SingleThreadedStackPool<SimulationState>> _pool = new ThreadLocal<SingleThreadedStackPool<SimulationState>>(() => new SingleThreadedStackPool<SimulationState>(), false);
	private readonly SingleThreadedStackPool<SimulationState> _thisThreadPool;

	private readonly SimulationState _replaySimulation = new SimulationState();
	private readonly List<PieceDefinition> _lookahead = new List<PieceDefinition>(PieceDefinition.AllPieceDefinitions.Length + SimulationState.LeatherPatches.Length);


	public MoveOnlyAlphaBetaWithPreplacerMoveMaker(int maxSearchDepth, IUtilityCalculator calculator, PreplacerStrategy preplacer)
	{
		_thisThreadPool = _pool.Value!;
		_maxSearchDepth = maxSearchDepth;
		_calculator = calculator;
		_preplacer = preplacer;
	}

	public void MakeMove(SimulationState state)
	{
		var bestMoves = AlphaBeta(state);
		var bestMove = bestMoves[bestMoves.Count - 1];

		//Now we can preplace our pieces, we have to resimulate the simulation to do so however, as we only know the best moves to make, not who makes them
		int preplaceAmount = 0;
		_lookahead.Clear();
		state.CloneTo(_replaySimulation);
		_replaySimulation.Fidelity = SimulationFidelity.NoPiecePlacing;
		for (var i = bestMoves.Count - 1; i >= 0; i--)
		{
			var move = bestMoves[i];
			var player = _replaySimulation.ActivePlayer;
			var patchIndex = _replaySimulation.LeatherPatchesIndex;

			//We purchased a piece
			if (player == state.ActivePlayer && move != -1)
			{
				_lookahead.Add(PieceDefinition.AllPieceDefinitions[_replaySimulation.Pieces[(_replaySimulation.NextPieceIndex + move) % _replaySimulation.Pieces.Count]]);
			}

			if (move == -1)
				_replaySimulation.PerformAdvanceMove();
			else
				_replaySimulation.PerformPurchasePiece(_replaySimulation.NextPieceIndex + move);

			//If we moved past a leather patch, we must have got it
			if (player == state.ActivePlayer && _replaySimulation.LeatherPatchesIndex > patchIndex)
			{
				_lookahead.Add(PieceDefinition.LeatherTile);
			}

			if (i == bestMoves.Count - 1)
				preplaceAmount = _lookahead.Count;
		}

		//Tell the Preplacer what we are planning on getting
		if (preplaceAmount > 0)
		{
			_preplacer.PreparePlacePiece(state.PlayerBoardState[state.ActivePlayer], _lookahead, preplaceAmount);
		}

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
	private List<int> AlphaBeta(SimulationState parentState)
	{
		var maximizingPlayer = parentState.ActivePlayer;
		int alpha = int.MinValue;
		int beta = int.MaxValue;

		int bestMove = -1;
		List<int> bestMoves = null!;
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

			if (beta <= alpha && move != -1)
				continue;

			parentState.CloneTo(state);
			state.Fidelity = SimulationFidelity.NoPiecePlacing;

			int v;
			if (move == -1)
			{
				state.PerformAdvanceMove();

				//Decrease alpha by 1 (if possible) so we can identify draws. We favor doing an advance in a draw, but we evaluate purchases first
				//This gives us the same results as if we were evaluating advance first, but performance is better
				//This makes us stronger than favoring buying pieces in a draw (Which probably implies our Evaluate method is incorrect?), but costs slight runtime performance
				v = AlphaBeta(state, _maxSearchDepth - 1, (alpha == int.MinValue ? int.MinValue : alpha - 1), beta, maximizingPlayer, out var moves);

				if (v >= bestValue)
				{
					bestMove = move;

					bestMoves = moves;
					bestMoves.Add(move);
				}
			}
			else
			{
				state.PerformPurchasePiece(state.NextPieceIndex + move);
				v = AlphaBeta(state, _maxSearchDepth - 1, alpha, beta, maximizingPlayer, out var moves);
				if (v > bestValue)
				{
					bestMove = move;

					bestMoves = moves;
					bestMoves.Add(move);
				}
			}

			//Console.WriteLine($"Considering {m.ToString().PadLeft(2)} = {v}");


			bestValue = Math.Max(bestValue, v);
			alpha = Math.Max(alpha, bestValue);
		}

		_thisThreadPool.Return(state);
		return bestMoves;
	}


	//https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning#Pseudocode
	private int AlphaBeta(SimulationState parentState, int depth, int alpha, int beta, int maximizingPlayer, [NotNull] out List<int>? moves)
	{
		//Console.WriteLine($"{depth} ({alpha}/{beta})");
		//We deviate from strict depth limited minimax here.
		//We want to ensure that both players get the same amount(ish) of turns, otherwise when we get one more turn than our opponent we will think that is better.
		//So to ensure fairness, we don't terminate a search until it is the maximizing players turn again, this ensures the opponent has had time to respond to our move
		//TODO: This isn't totally perfect, if the last move we made gave us an extra move, we've got a turn advantage and it is our turn, so we'll terminate
		//TODO: I roughly tested making this continue until the last player was the non-maximizing player also, this made the search 2.5x as long and it seemed slightly weaker
		//TODO: Adding the ActivePlayer check vs not having it improves strength a lot, but makes the search runtime 2x
		if ((depth <= 0 && parentState.ActivePlayer == maximizingPlayer) || parentState.GameHasEnded)
		{
			moves = new List<int>();
			return Evaluate(parentState, maximizingPlayer);
		}

		var shouldMaximize = maximizingPlayer == parentState.ActivePlayer;
		int bestValue = shouldMaximize ? int.MinValue : int.MaxValue;
		moves = null;


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

			var v = AlphaBeta(state, depth - 1, alpha, beta, maximizingPlayer, out var childMoves);

			if (shouldMaximize)
			{
				if (v > bestValue)
				{
					moves = childMoves;
					moves.Add(move);
				}

				bestValue = Math.Max(bestValue, v);
				alpha = Math.Max(alpha, bestValue);
			}
			else
			{
				if (v < bestValue)
				{
					moves = childMoves;
					moves.Add(move);
				}

				bestValue = Math.Min(bestValue, v);
				beta = Math.Min(beta, bestValue);
			}

			if (beta <= alpha)
			{
				break;
			}
		}

		//All of our child moves result in a loss
		if (moves == null)
			moves = new List<int>();

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