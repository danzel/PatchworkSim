using System;
using System.Collections.Generic;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers
{
	/// <summary>
	/// Search the best n future boards, favoring being able to go deeper
	/// </summary>
	public class EvaluatorTreeSearchPreplacer : IPreplacer
	{
		public string Name => $"EvalTree({_boardEvaluator.Name}@{_depth}/{_branching}/{_scoreFinalState})";

		private readonly IBoardEvaluator _boardEvaluator;
		private readonly int _depth;
		private readonly int _branching;
		private readonly bool _scoreFinalState;

		public EvaluatorTreeSearchPreplacer(IBoardEvaluator boardEvaluator, int depth, int branching, bool scoreFinalState)
		{
			_boardEvaluator = boardEvaluator;
			_depth = depth;
			_branching = branching;
			_scoreFinalState = scoreFinalState;
		}

		public Preplacement Preplace(BoardState board, List<PieceDefinition> plannedFuturePieces)
		{
			var root = FindBestPlacements(in board, plannedFuturePieces[0]);

			long bestScore = long.MinValue;
			Preplacement? bestPreplacement = null;

			for (var i = 0; i < root.Length; i++)
			{
				if (root[i] == null)
					break;
				var move = root[i].Value;

				var clone = board;
				clone.Place(move.Bitmap, move.X, move.Y);
				var score = PreplaceRecursive(in clone, plannedFuturePieces, 1);

				if (score > bestScore)
				{
					bestScore = score;
					bestPreplacement = move;
				}
			}

			return bestPreplacement.Value;
		}

		private Preplacement?[] FindBestPlacements(in BoardState board, PieceDefinition piece)
		{
			var bestScores = new int[_branching];
			var bestPlacements = new Preplacement?[_branching];
			int size = 0;

			_boardEvaluator.BeginEvaluation(board);

			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						if (board.CanPlace(bitmap, x, y))
						{
							var clone = board;
							clone.Place(bitmap, x, y);

							var score = _boardEvaluator.Evaluate(in clone, x, x + bitmap.Width, y, y + bitmap.Height);


							//Insertion sort
							bool inserted = false;
							for (var i = 0; i < size; i++)
							{
								if (bestScores[i] < score)
								{
									//Insert here

									//Move existing ones back
									for (var j = i; j < size - 1; j++)
									{
										bestScores[j + 1] = bestScores[j];
										bestPlacements[j + 1] = bestPlacements[j];
									}

									//TODO: Preplacement garbage
									bestScores[i] = score;
									bestPlacements[i] = new Preplacement(bitmap, x, y);

									inserted = true;
								}
							}

							if (!inserted && size < _branching)
							{
								//Insert at end
								bestScores[size] = score;
								bestPlacements[size] = new Preplacement(bitmap, x, y);
								size++;
							}
						}
					}
				}
			}

			return bestPlacements;
		}

		private long PreplaceRecursive(in BoardState board, List<PieceDefinition> pieces, int depth)
		{
			if (depth == _depth || depth == pieces.Count)
			{
				var eval = 1;
				if (_scoreFinalState)
				{
					_boardEvaluator.BeginEvaluation(new BoardState());
					eval = _boardEvaluator.Evaluate(in board, 0, BoardState.Width, 0, BoardState.Height);
				}

				return PowCache[depth] * eval; //TODO ??????
			}

			var nextPiece = pieces[depth];

			if (!Helpers.CanPlace(board, nextPiece))
			{
				var eval = 1;
				if (_scoreFinalState)
				{
					_boardEvaluator.BeginEvaluation(new BoardState());
					eval = _boardEvaluator.Evaluate(in board, 0, BoardState.Width, 0, BoardState.Height);
				}

				return PowCache[depth] * eval; //TODO ??????
			}

			long score = 0;

			var placements = FindBestPlacements(in board, nextPiece);

			for (var i = 0; i < placements.Length; i++)
			{
				if (placements[i] == null)
					break;
				var move = placements[i].Value;

				var clone = board;
				clone.Place(move.Bitmap, move.X, move.Y);
				score += PreplaceRecursive(in clone, pieces, depth + 1);
			}

			return score;
		}

		private static readonly long[] PowCache =
		{
			(long)Math.Pow(10, 0),
			(long)Math.Pow(10, 1),
			(long)Math.Pow(10, 2),
			(long)Math.Pow(10, 3),
			(long)Math.Pow(10, 4),
			(long)Math.Pow(10, 5),
			(long)Math.Pow(10, 6),
		};
	}
}