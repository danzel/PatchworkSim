using System.Collections.Generic;
using System.Linq;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead
{
	/// <summary>
	/// Can place pieces at 0,0 - or next to any other piece.
	/// Ties are broken by: Least Holes, Largest Largest Hole, (TODO: For final tiebreak try: Smallest Bounding box, closest to 0,0 (direct and straight lines)
	/// </summary>
	public class NextToPieceEdgeLeastHolesTieBreakerPlacementStrategy : NoLookaheadStrategy
	{
		public override string Name => "NextToPieceEdgeLeastHolesTieBreaker";

		public static readonly NextToPieceEdgeLeastHolesTieBreakerPlacementStrategy Instance = new NextToPieceEdgeLeastHolesTieBreakerPlacementStrategy();

		private NextToPieceEdgeLeastHolesTieBreakerPlacementStrategy()
		{
		}

		protected override bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int resultX, out int resultY)
		{
			resultBitmap = null;
			resultX = -1;
			resultY = -1;

			int bestLeastHoles = BoardState.Width * BoardState.Height + 1;
			int bestLargestHole = BoardState.Width * BoardState.Height + 1;
			int bestDistance = BoardState.Width * BoardState.Height + 1;
			var holes = new List<int>();

			foreach (var bitmap in piece.PossibleOrientations)
			{
				//TODO: Try at 0, 0
				if (board.CanPlace(bitmap, 0, 0))
				{
					TryPlacement(board, bitmap, 0, 0, ref resultBitmap, ref resultX, ref resultY, ref bestLeastHoles, ref bestLargestHole, ref bestDistance, holes);
				}

				for (var x = 0; x <= BoardState.Width - bitmap.Width; x++)
				{
					//You can only place in a position if you couldn't place in the previous position, or if you can't place in the next position
					bool couldPlaceBefore = true;

					for (var y = 0; y <= BoardState.Height - bitmap.Height; y++)
					{
						var canPlaceNow = board.CanPlace(bitmap, x, y);

						if (!couldPlaceBefore && canPlaceNow)
						{
							//Try here
							TryPlacement(board, bitmap, x, y, ref resultBitmap, ref resultX, ref resultY, ref bestLeastHoles, ref bestLargestHole, ref bestDistance, holes);
						}
						if (couldPlaceBefore && !canPlaceNow && y > 0)
						{
							//try at previous place
							// TODO May have already been tried (optimization, record the last tried place?)
							TryPlacement(board, bitmap, x, y - 1, ref resultBitmap, ref resultX, ref resultY, ref bestLeastHoles, ref bestLargestHole, ref bestDistance, holes);
						}

						couldPlaceBefore = canPlaceNow;
					}
				}

				//Do a y then x loop as well
				for (var y = 0; y <= BoardState.Height - bitmap.Height; y++)
				{
					//You can only place in a position if you couldn't place in the previous position, or if you can't place in the next position
					bool couldPlaceBefore = true;

					for (var x = 0; x <= BoardState.Width - bitmap.Width; x++)
					{
						var canPlaceNow = board.CanPlace(bitmap, x, y);

						if (!couldPlaceBefore && canPlaceNow)
						{
							//Try here
							//TODO May have already tried this in the x,y loop above
							TryPlacement(board, bitmap, x, y, ref resultBitmap, ref resultX, ref resultY, ref bestLeastHoles, ref bestLargestHole, ref bestDistance, holes);
						}
						if (couldPlaceBefore && !canPlaceNow && x > 0)
						{
							//try at previous place
							// TODO May have already been tried (optimization, record the last tried place?)
							//TODO May have already tried this in the x,y loop above
							TryPlacement(board, bitmap, x - 1, y, ref resultBitmap, ref resultX, ref resultY, ref bestLeastHoles, ref bestLargestHole, ref bestDistance, holes);
						}

						couldPlaceBefore = canPlaceNow;
					}
				}
			}

			return resultBitmap != null;
		}

		private void TryPlacement(BoardState board, PieceBitmap bitmap, int x, int y, ref PieceBitmap resultBitmap, ref int resultX, ref int resultY, ref int bestLeastHoles, ref int bestLargestHole, ref int bestDistance, List<int> holes)
		{
			board.Place(bitmap, x, y);

			holes.Clear();
			PlacementHelper.HoleCount(board, ref holes);

			//TODO: Remove LINQ
			var distance = x + y; //TODO: Try direct
			if (holes.Count < bestLeastHoles || (holes.Count == bestLeastHoles && holes.Max() > bestLargestHole) || (holes.Count == bestLeastHoles && holes.Max() == bestLargestHole && distance < bestDistance))
			{
				bestLeastHoles = holes.Count;
				bestLargestHole = holes.Max();
				bestDistance = distance;

				resultBitmap = bitmap;
				resultX = x;
				resultY = y;
			}
		}
	}
}