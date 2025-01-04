using System.Collections.Generic;
using System.Linq;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;

/// <summary>
/// Tries to place the piece the closests to the 0,0 corner as possible.
/// When there are multiple placements at the same distance, uses the least amount of holes as a tie breaker, then uses the largest hole as a tie breaker (a bigger largest hole is better)
/// </summary>
public class ClosestToCornerLeastHolesTieBreakerPlacementStrategy : NoLookaheadStrategy
{
	public override string Name => "ClosestToCornerLeastHolesTieBreaker";

	public static readonly ClosestToCornerLeastHolesTieBreakerPlacementStrategy Instance = new ClosestToCornerLeastHolesTieBreakerPlacementStrategy();

	private ClosestToCornerLeastHolesTieBreakerPlacementStrategy()
	{
	}

	protected override bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int resultX, out int resultY)
	{
		var holes = new List<int>();

		int leastHoles = BoardState.Width * BoardState.Height;
		int largestHoleSize = 0;

		resultBitmap = null;
		resultX = -1;
		resultY = -1;


		//TODO: Distance could be < the piece size too
		for (var distance = 0; distance < BoardState.Width + BoardState.Height; distance++)
		{
			//TODO: Min(distance, Width - bitmap.MinSideSize) ?
			for (var x = 0; x <= distance ; x++)
			{
				var y = distance - x;

				foreach (var bitmap in piece.PossibleOrientations)
				{
					if (x + bitmap.Width <= BoardState.Width && y + bitmap.Height <= BoardState.Height && board.CanPlace(bitmap, x, y))
					{
						var clone = board;
						clone.Place(bitmap, x, y);

						holes.Clear();
						PlacementHelper.HoleCount(clone, ref holes);

						//TODO: No Linq
						if (holes.Count < leastHoles || (holes.Count == leastHoles && holes.Max() > largestHoleSize))
						{
							leastHoles = holes.Count;
							largestHoleSize = holes.Max();

							resultBitmap = bitmap;
							resultX = x;
							resultY = y;
						}
					}
				}
			}

			if (resultBitmap != null)
				return true;
		}

		return false;
	}
}