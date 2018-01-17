using System.Collections.Generic;
using System.Linq;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	/// <summary>
	/// Tries to place the piece the closests to the 0,0 corner as possible.
	/// When there are multiple placements at the same distance, uses the least amount of holes as a tie breaker, then uses the largest hole as a tie breaker (a bigger largest hole is better)
	/// </summary>
	public class ClosestToCornerLeastHolesTieBreakerPlacementStrategy : IPlacementStrategy
	{
		public string Name => "ClosestToCornerLeastHolesTieBreaker";
		public bool ImplementsAdvanced => false;

		public static ClosestToCornerLeastHolesTieBreakerPlacementStrategy Instance = new ClosestToCornerLeastHolesTieBreakerPlacementStrategy();

		private ClosestToCornerLeastHolesTieBreakerPlacementStrategy()
		{
		}

		public bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int resultX, out int resultY)
		{
			var holes = new List<int>();

			int leastHoles = BoardState.Width * BoardState.Height;
			int largestHoleSize = 0;

			resultBitmap = null;
			resultX = -1;
			resultY = -1;


			for (var distance = 0; distance < BoardState.Width + BoardState.Height; distance++)
			{

				for (var x = 0; x <= distance ; x++)
				{
					var y = distance - x;

					foreach (var bitmap in piece.PossibleOrientations)
					{
						if (board.CanPlace(bitmap, x, y))
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

		public bool TryPlacePieceAdvanced(BoardState board, PieceDefinition piece, List<int> possibleFuturePieces, int possibleFuturePiecesOffset, out PieceBitmap bitmap, out int x, out int y)
		{
			throw new System.NotImplementedException();
		}
	}
}