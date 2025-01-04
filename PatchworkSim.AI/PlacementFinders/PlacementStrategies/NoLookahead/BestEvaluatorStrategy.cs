using PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;

public class BestEvaluatorStrategy : NoLookaheadStrategy
{
	public override string Name => $"Best({_evaluator.Name})";

	private readonly IBoardEvaluator _evaluator;

	public BestEvaluatorStrategy(IBoardEvaluator evaluator)
	{
		_evaluator = evaluator;
	}


	protected override bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int resultX, out int resultY)
	{
		_evaluator.BeginEvaluation(board);

		resultBitmap = null;
		resultX = -1;
		resultY = -1;

		int bestScore = int.MinValue;

		//Exhaustively place it and make new child nodes
		for (var index = 0; index < piece.PossibleOrientations.Length; index++)
		{
			var bitmap = piece.PossibleOrientations[index];
			var searchWidth = BoardState.Width - bitmap.Width + 1;
			var searchHeight = BoardState.Height - bitmap.Height + 1;

			//TODO? If this is the first piece, remove mirrors/rotations from the children
			//if (isFirstPiece)
			//{
			//	//TODO: This doesn't stop diagonal mirrors
			//	searchWidth = (BoardState.Width - bitmap.Width) / 2 + 1;
			//	searchHeight = (BoardState.Height - bitmap.Height) / 2 + 1;
			//}

			for (int x = 0; x < searchWidth; x++)
			{
				for (int y = 0; y < searchHeight; y++)
				{
					if (board.CanPlace(bitmap, x, y))
					{
						var clone = board;
						clone.Place(bitmap, x, y);

						var score = _evaluator.Evaluate(in clone, x, x + bitmap.Width, y, y + bitmap.Height);
						if (score > bestScore)
						{
							resultBitmap = bitmap;
							resultX = x;
							resultY = y;

							bestScore = score;
						}
					}
				}
			}
		}

		return resultBitmap != null;
	}
}
