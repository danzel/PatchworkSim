using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead
{
	/// <summary>
	/// Starting at 0,0 move out along the walls attempting to place the piece.
	/// The first time it succeeds, place the piece there.
	/// If it fails, move out from the other wall by one and try again
	/// </summary>
	/// <remarks>
	/// Possible Improvements:
	///  - Minimize the amount of holes left. We probably leave lots of 1x1 and larger holes. These are bad
	///  - Consider other close to the origin placements - often there are tidier placements than just the next one along that fit closer and leave less ugly gaps
	/// </remarks>
	public class SimpleClosestToWallAndCornerStrategy : NoLookaheadStrategy
	{
		public override string Name => "SimpleClosestToWallAndCorner";

		public static SimpleClosestToWallAndCornerStrategy Instance = new SimpleClosestToWallAndCornerStrategy();

		private SimpleClosestToWallAndCornerStrategy()
		{
		}

		protected override bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int x, out int y)
		{
			for (var oppositeWallGap = 0; oppositeWallGap < BoardState.Width; oppositeWallGap++)
			{
				//TODO: Could use the minimum of the size of the sides of the bitmap in this gap < part to skip some wasteful loops
				for (var gap = 0; gap < BoardState.Height; gap++)
				{
					foreach (var bitmap in piece.PossibleOrientations)
					{
						//Try place in x direction
						if (gap + bitmap.Width <= BoardState.Width && oppositeWallGap + bitmap.Height <= BoardState.Height && board.CanPlace(bitmap, gap, oppositeWallGap))
						{
							resultBitmap = bitmap;
							x = gap;
							y = oppositeWallGap;
							return true;
						}

						//Try place in y direction
						if (oppositeWallGap + bitmap.Width <= BoardState.Width && gap + bitmap.Height <= BoardState.Height && board.CanPlace(bitmap, oppositeWallGap, gap))
						{
							resultBitmap = bitmap;
							x = oppositeWallGap;
							y = gap;
							return true;
						}
					}
				}
			}

			resultBitmap = null;
			x = -1;
			y = -1;
			return false;
		}
	}
}
