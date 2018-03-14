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

		private readonly ListPool<BoardWithParent> _pool = new ListPool<BoardWithParent>();

		public CNTKEvaluatorTreeSearch(BulkBoardEvaluator evaluator, int branching)
		{
			_evaluator = evaluator;
			_branching = branching;
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
			nextBoards.Sort();
			if (nextBoards.Count > _branching)
				nextBoards.RemoveRange(_branching, nextBoards.Count - _branching);
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