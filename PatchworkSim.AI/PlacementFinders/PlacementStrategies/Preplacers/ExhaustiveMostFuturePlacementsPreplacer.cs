using System;
using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;

public class ExhaustiveMostFuturePlacementsPreplacer : IPreplacer
{
	public string Name => $"Exhaustive({_depth})";

	private readonly int _depth;

	public ExhaustiveMostFuturePlacementsPreplacer(int depth)
	{
		_depth = depth;
	}

	public Preplacement Preplace(BoardState board, List<PieceDefinition> plannedFuturePieces)
	{
		PieceBitmap resultBitmap = null;
		int resultX = -1;
		int resultY = -1;

		//How many placements the next future piece has for our best found placement (more is better)
		long bestPlacementCount = -1;
		//Tie break when there is a draw, based on distance to 0,0 (less is better)
		int bestTieBreaker = -1;

		foreach (var bitmap in plannedFuturePieces[0].PossibleOrientations)
		{
			for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
			{
				for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
				{
					if (board.CanPlace(bitmap, x, y))
					{
						long placementCount = CalculatePlacementCount(board, bitmap, x, y, plannedFuturePieces, 1);

						var tieBreaker = x + y;
						if (placementCount > bestPlacementCount || (placementCount == bestPlacementCount && tieBreaker < bestTieBreaker))
						{
							bestPlacementCount = placementCount;
							bestTieBreaker = tieBreaker;

							resultBitmap = bitmap;
							resultX = x;
							resultY = y;
						}
					}
				}
			}
		}

		if (resultBitmap == null)
			throw new Exception("Cannot place the first piece");

		return new Preplacement(resultBitmap, resultX, resultY);
	}

	private static readonly long[] PowCache = {
		(long)Math.Pow(10, 0),
		(long)Math.Pow(10, 1),
		(long)Math.Pow(10, 2),
		(long)Math.Pow(10, 3),
		(long)Math.Pow(10, 4),
		(long)Math.Pow(10, 5),
		(long)Math.Pow(10, 6),
	};

	private long CalculatePlacementCount(BoardState board, PieceBitmap justPlacedBitmap, int justPlacedX, int justPlacedY, List<PieceDefinition> plannedFuturePieces, int depth)
	{
		long placementCount = PowCache[depth];

		//We are as deep as we are allowed to go, or we are out of pieces to place
		if (_depth == depth || plannedFuturePieces.Count == depth)
			return placementCount;

		board.Place(justPlacedBitmap, justPlacedX, justPlacedY);


		//Now calculate how many placements the next piece can have
		var nextPiece = plannedFuturePieces[depth];

		for (var index = 0; index < nextPiece.PossibleOrientations.Length; index++)
		{
			var bitmap = nextPiece.PossibleOrientations[index];
			for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
			{
				for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
				{
					if (board.CanPlace(bitmap, x, y))
					{
						placementCount += CalculatePlacementCount(board, bitmap, x, y, plannedFuturePieces, depth + 1);
					}
				}
			}
		}

		return placementCount;
	}
}
