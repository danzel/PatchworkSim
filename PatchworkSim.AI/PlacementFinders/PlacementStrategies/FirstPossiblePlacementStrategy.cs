using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	public class FirstPossiblePlacementStrategy : IPlacementStrategy
	{
		public static FirstPossiblePlacementStrategy Instance = new FirstPossiblePlacementStrategy();

		public string Name => "FirstPossible";
		public bool ImplementsAdvanced => false;

		private FirstPossiblePlacementStrategy()
		{
		}

		public bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int resultX, out int resultY)
		{
			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (var y = 0; y <= BoardState.Height - bitmap.Height; y++)
				{
					for (var x = 0; x <= BoardState.Width - bitmap.Width; x++)
					{
						if (board.CanPlace(bitmap, x, y))
						{
							resultBitmap = bitmap;
							resultX = x;
							resultY = y;
							return true;
						}
					}
				}
			}

			resultBitmap = null;
			resultX = -1;
			resultY = -1;
			return false;
		}

		public bool TryPlacePieceAdvanced(BoardState board, PieceDefinition piece, List<int> possibleFuturePieces, int possibleFuturePiecesOffset, out PieceBitmap bitmap, out int x, out int y)
		{
			throw new System.NotImplementedException();
		}
	}
}