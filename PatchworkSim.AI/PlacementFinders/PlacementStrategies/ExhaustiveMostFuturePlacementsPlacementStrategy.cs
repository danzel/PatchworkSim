using System;
using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	public class ExhaustiveMostFuturePlacementsPlacementStrategy : IPlacementStrategy
	{
		public static readonly ExhaustiveMostFuturePlacementsPlacementStrategy Instance1 = new ExhaustiveMostFuturePlacementsPlacementStrategy(1);

		public string Name => $"ExhaustiveMostFuturePlacements({_lookAheadAmount})";
		public bool ImplementsAdvanced => true;

		private readonly int _lookAheadAmount;

		public ExhaustiveMostFuturePlacementsPlacementStrategy(int lookAheadAmount)
		{
			_lookAheadAmount = lookAheadAmount;
		}

		public bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap bitmap, out int x, out int y)
		{
			throw new NotImplementedException("Use the advanced version");
		}

		public bool TryPlacePieceAdvanced(BoardState board, PieceDefinition piece, List<int> possibleFuturePieces, int possibleFuturePiecesOffset, out PieceBitmap resultBitmap, out int resultX, out int resultY)
		{
			resultBitmap = null;
			resultX = -1;
			resultY = -1;

			//How many placements the next future piece has for our best found placement
			int bestPlacementCount = -1;

			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						if (board.CanPlace(bitmap, x, y))
						{
							int placementCount = CalculatePlacementCount(board, bitmap, x, y, possibleFuturePieces, possibleFuturePiecesOffset, _lookAheadAmount);

							if (placementCount > bestPlacementCount)
							{
								bestPlacementCount = placementCount;
								resultBitmap = bitmap;
								resultX = x;
								resultY = y;
							}
						}
					}
				}
			}

			return resultBitmap != null;
		}

		private int CalculatePlacementCount(BoardState board, PieceBitmap justPlacedBitmap, int justPlacedX, int justPlacedY, List<int> possibleFuturePieces, int possibleFuturePiecesOffset, int lookAheadAmount)
		{
			if (lookAheadAmount == 0)
				return (_lookAheadAmount - lookAheadAmount);

			board.Place(justPlacedBitmap, justPlacedX, justPlacedY);

			int placementCount = (_lookAheadAmount - lookAheadAmount);

			//Now calculate how many placements the next piece can have
			var nextPiece = PieceDefinition.AllPieceDefinitions[possibleFuturePieces[possibleFuturePiecesOffset]];

			foreach (var bitmap in nextPiece.PossibleOrientations)
			{
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						if (board.CanPlace(bitmap, x, y))
						{
							placementCount += CalculatePlacementCount(board, bitmap, x, y, possibleFuturePieces, (possibleFuturePiecesOffset + 1) % possibleFuturePieces.Count, lookAheadAmount - 1);
						}
					}
				}
			}

			return placementCount;
		}
	}
}