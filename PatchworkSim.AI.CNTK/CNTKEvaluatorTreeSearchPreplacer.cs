using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PatchworkSim.AI.CNTK
{
	/// <summary>
	/// Performs similar to a preplacer, but keeps multiple possible boards for each placement.  Evaluates possible boards in parallel using the NN.
	/// </summary>
	internal class CNTKEvaluatorTreeSearch
	{
		private readonly BulkBoardEvaluator _evaluator;
		private readonly int _branching;
		private readonly int _keepRandom;

		private readonly ListPool<BoardWithParent> _pool = new ListPool<BoardWithParent>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="evaluator"></param>
		/// <param name="branching">How many to keep at each level</param>
		/// <param name="keepRandom">How many at each level to keep that are not the 'best'</param>
		public CNTKEvaluatorTreeSearch(BulkBoardEvaluator evaluator, int branching, int keepRandom)
		{
			_evaluator = evaluator;
			_branching = branching;
			_keepRandom = keepRandom;
		}

		/// <summary>
		/// Do a BFS of the best placements, keeping the best {branching} worth from each level
		/// </summary>
		public HashSet<BoardState> PreplaceAll(List<PieceDefinition> pieces, out int totalPlaceAreaCovered)
		{
			//TODO: We can keep the HashSet and just clear it each time (GC)
			/*
			 * Get {branching} worth of the best next placements
			 * while (pieces left to place) {
			 *	place the next piece in all of the current placements and keep the best {branching}
			 * if we couldnt place, just keep the last placements
			 * return all of the (unique) boards that lead to the current best placements
			 * }
			 */

			var initialBoard = new BoardState();
			var currentBoards = GetAllPossibleInitialPlacements(in initialBoard, pieces[0]);
			CleanPlacements(currentBoards);
			int areaCovered = pieces[0].TotalUsedLocations;

			for (var depth = 1; depth < pieces.Count; depth++)
			{
				var nextBoards = _pool.Get();

				//Breadth first search of future boards
				for (var i = 0; i < currentBoards.Count; i++)
				{
					var boardWithPlacement = currentBoards[i];

					GetAllPossiblePlacements(boardWithPlacement, pieces[depth], nextBoards);
				}

				if (nextBoards.Count == 0)
				{
					_pool.Return(nextBoards);
					break;
				}

				areaCovered += pieces[depth].TotalUsedLocations;

				CleanPlacements(nextBoards);

				_pool.Return(currentBoards);
				currentBoards = nextBoards;
			}

			//Unroll all of the (unique) boards that led to here in to the result, and calculate
			var result = new HashSet<BoardState>();
			for (var i = 0; i < currentBoards.Count; i++)
			{
				var board = currentBoards[i];
				while (board != null)
				{
					result.Add(board.Board);
					board = board.Parent;
				}
			}

			totalPlaceAreaCovered = areaCovered;

			_pool.Return(currentBoards);
			return result;
		}

		private void CleanPlacements(List<BoardWithParent> nextBoards)
		{
			//Sort by score and only keep _branching worth of them
			_evaluator.Evaluate(nextBoards);
			//nextBoards.Sort();
			if (nextBoards.Count > _branching)
			{
				//Shuffle _keepRandom worth, so we don't always keep the best, we keep some that might not be good, so that we can learn from them
				if (_keepRandom > 0)
				{
					ShuffleKeepRandom(nextBoards);
				}
				nextBoards.RemoveRange(_branching, nextBoards.Count - _branching);
			}
		}

		private readonly Random _rng = new Random(0);

		/// <summary>
		/// Shuffle from (_branching - keepRandom) to _branching, so the last few that we keep are random ones
		/// </summary>
		public void ShuffleKeepRandom(List<BoardWithParent> list)
		{
			for (var n = _branching - _keepRandom; n < _branching; n++)
			{
				int k = n + _rng.Next(list.Count - n);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}



		private List<BoardWithParent> GetAllPossibleInitialPlacements(in BoardState board, PieceDefinition piece)
		{
			//TODO: Maybe optimise empty board placement?

			var result = _pool.Get();

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
							result.Add(new BoardWithParent(null, clone));
						}
					}
				}
			}

			return result;
		}

		private void GetAllPossiblePlacements(BoardWithParent boardWithParent, PieceDefinition piece, List<BoardWithParent> result)
		{
			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						if (boardWithParent.Board.CanPlace(bitmap, x, y))
						{
							var clone = boardWithParent.Board;
							clone.Place(bitmap, x, y);
							result.Add(new BoardWithParent(boardWithParent, clone));
						}
					}
				}
			}
		}
	}
}