using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;

namespace PatchworkSim.AI.CNTK
{
	/// <summary>
	/// Preplacer primarily to be used for training the neural network.
	/// Performs like EvaluatorTreeSearchPreplacer, but evaluates possible boards in parallel using the NN.
	/// </summary>
	internal class CNTKEvaluatorTreeSearchPreplacer : IPreplacer
	{
		public string Name => $"CNTKEvaluatorTreeSearchPreplacer({_depth}/{_branching})";

		private readonly BulkBoardEvaluator _evaluator;
		private readonly int _depth;
		private readonly int _branching;

		private readonly ListPool<BoardWithPlacement> _pool = new ListPool<BoardWithPlacement>();

		public CNTKEvaluatorTreeSearchPreplacer(BulkBoardEvaluator evaluator, int depth, int branching)
		{
			_evaluator = evaluator;
			_depth = depth;
			_branching = branching;
		}

		public Preplacement Preplace(BoardState initialBoard, List<PieceDefinition> plannedFuturePieces)
		{
			var currentBoards = GetAllPossibleInitialPlacements(in initialBoard, plannedFuturePieces[0]);
			CleanPlacements(currentBoards);

			for (var depth = 1; depth < _depth; depth++)
			{
				var nextBoards = _pool.Get();

				//Breadth first search of future boards
				for (var i = 0; i < currentBoards.Count; i++)
				{
					var boardWithPlacement = currentBoards[i];

					GetAllPossiblePlacements(in boardWithPlacement.Board, plannedFuturePieces[depth], boardWithPlacement.FirstPiecePlacement, nextBoards);
				}

				if (nextBoards.Count == 0)
					break;

				CleanPlacements(nextBoards);

				_pool.Return(currentBoards);
				currentBoards = nextBoards;
			}

			var res =  currentBoards[0].FirstPiecePlacement;
			_pool.Return(currentBoards);
			return res;
		}

		private void CleanPlacements(List<BoardWithPlacement> nextBoards)
		{
//Sort by score and only keep _branching worth of them
			_evaluator.Evaluate(nextBoards);
			nextBoards.Sort();
			//TODO: Check high scores are at the top
			if (nextBoards.Count > _branching)
				nextBoards.RemoveRange(_branching, nextBoards.Count - _branching);
		}

		private List<BoardWithPlacement> GetAllPossibleInitialPlacements(in BoardState board, PieceDefinition piece)
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
							result.Add(new BoardWithPlacement(clone, new Preplacement(bitmap, x, y)));
						}
					}
				}
			}

			return result;
		}

		private void GetAllPossiblePlacements(in BoardState board, PieceDefinition piece, Preplacement firstPiecePlacement, List<BoardWithPlacement> result)
		{
			//TODO: Maybe optimise empty board placement?

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
							result.Add(new BoardWithPlacement(clone, firstPiecePlacement));
						}
					}
				}
			}
		}
	}
}