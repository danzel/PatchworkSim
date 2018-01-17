using System;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead
{
	/// <summary>
	/// Places the piece to minimise the bounding box of used spaces
	/// </summary>
	public class SmallestBoundingBoxPlacementStrategy : NoLookaheadStrategy
	{
		public static readonly SmallestBoundingBoxPlacementStrategy Instance = new SmallestBoundingBoxPlacementStrategy();

		public override string Name => "SmallestBoundingBox";

		private SmallestBoundingBoxPlacementStrategy()
		{
		}

		protected override bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int resultX, out int resultY)
		{
			int smallestSize = BoardState.Height * BoardState.Width + 1;

			resultBitmap = null;
			resultX = 0;
			resultY = 0;

			//TODO: If we weren't a singleton we could track our used Width/Height
			int currentHeight = CalcUsedHeight(ref board);
			int currentWidth = CalcUsedWidth(ref board);
			Console.WriteLine($"Current {currentWidth}x{currentHeight}");

			for (var x = 0; x < BoardState.Width; x++)
			{
				for (var y = 0; y < BoardState.Height; y++)
				{
					foreach (var bitmap in piece.PossibleOrientations)
					{
						var size = Math.Max(currentWidth, x + bitmap.Width) * Math.Max(currentHeight, y + bitmap.Height);

						if (size < smallestSize && board.CanPlace(bitmap, x, y))
						{
							smallestSize = size;
							resultBitmap = bitmap;
							resultX = x;
							resultY = y;
						}
					}
				}
			}

			return resultBitmap != null;
		}

		private static int CalcUsedHeight(ref BoardState board)
		{
			for (var y = BoardState.Height - 1; y >= 0; y--)
			{
				for (var x = 0; x < BoardState.Width; x++)
				{
					if (board[x, y])
						return y + 1;
				}
			}

			return 0;
		}

		private static int CalcUsedWidth(ref BoardState board)
		{
			for (var x = BoardState.Width - 1; x >= 0; x--)
			{
				for (var y = 0; y < BoardState.Width; y++)
				{
					if (board[x, y])
						return x + 1;
				}
			}

			return 0;
		}
	}
}