using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead
{
	public class FirstPossiblePlacementStrategy : NoLookaheadStrategy
	{
		public static readonly FirstPossiblePlacementStrategy Instance = new FirstPossiblePlacementStrategy();

		public override string Name => "FirstPossible";

		private FirstPossiblePlacementStrategy()
		{
		}

		protected override bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int resultX, out int resultY)
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
	}
}