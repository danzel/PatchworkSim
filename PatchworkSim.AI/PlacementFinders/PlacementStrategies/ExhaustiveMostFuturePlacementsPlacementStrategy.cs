using System;
using System.Diagnostics.CodeAnalysis;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies;

/// <summary>
/// Strategy that exhaustively tries all possible placements of future pieces and chooses the best placement for the given piece based on how many possible future placements will be possible.
/// This strategy assumes that the next piece in possibleFuturePieces is guaranteed to be the next piece, which is untrue in real game play.
/// </summary>
public class ExhaustiveMostFuturePlacementsPlacementStrategy : IPlacementStrategy
{
	public static readonly ExhaustiveMostFuturePlacementsPlacementStrategy Instance1_1 = new ExhaustiveMostFuturePlacementsPlacementStrategy(1, 1);
	public static readonly ExhaustiveMostFuturePlacementsPlacementStrategy Instance1_6 = new ExhaustiveMostFuturePlacementsPlacementStrategy(1, 6);

	public string Name => $"ExhaustiveMostFuturePlacements({_lookAheadAmount}-{_lookAheadSpread})";
	public bool ImplementsLookahead => true;

	private readonly int _lookAheadAmount;
	private readonly int _lookAheadSpread;

	public ExhaustiveMostFuturePlacementsPlacementStrategy(int lookAheadAmount, int lookAheadSpread)
	{
		_lookAheadAmount = lookAheadAmount;
		_lookAheadSpread = lookAheadSpread;
	}

	public bool TryPlacePiece(BoardState board, PieceDefinition piece, in PieceCollection possibleFuturePieces, int possibleFuturePiecesOffset, [NotNullWhen(true)] out PieceBitmap? resultBitmap, out int resultX, out int resultY)
	{
		resultBitmap = null;
		resultX = -1;
		resultY = -1;

		//How many placements the next future piece has for our best found placement (more is better)
		long bestPlacementCount = -1;
		//Tie break when there is a draw, based on distance to 0,0 (less is better)
		int bestTieBreaker = -1;

		foreach (var bitmap in piece.PossibleOrientations)
		{
			for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
			{
				for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
				{
					if (board.CanPlace(bitmap, x, y))
					{
						long placementCount = 0;
						for (var i = 0; i < _lookAheadSpread; i++)
							placementCount += CalculatePlacementCount(board, bitmap, x, y, in possibleFuturePieces, (possibleFuturePiecesOffset + i) % possibleFuturePieces.Count, _lookAheadAmount);

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

		return resultBitmap != null;
	}

	private static readonly long[] powCache = new long[] {
		(long)Math.Pow(10, 0),
		(long)Math.Pow(10, 1),
		(long)Math.Pow(10, 2),
		(long)Math.Pow(10, 3),
		(long)Math.Pow(10, 4),
		(long)Math.Pow(10, 5),
		(long)Math.Pow(10, 6),
	};

	private long CalculatePlacementCount(BoardState board, PieceBitmap justPlacedBitmap, int justPlacedX, int justPlacedY, in PieceCollection possibleFuturePieces, int possibleFuturePiecesOffset, int lookAheadAmount)
	{
		long placementCount = powCache[_lookAheadAmount - lookAheadAmount];//(int)Math.Pow(10, _lookAheadAmount - lookAheadAmount);

		if (lookAheadAmount == 0)
			return placementCount;

		board.Place(justPlacedBitmap, justPlacedX, justPlacedY);


		//Now calculate how many placements the next piece can have
		var nextPiece = PieceDefinition.AllPieceDefinitions[possibleFuturePieces[possibleFuturePiecesOffset]];

		for (var index = 0; index < nextPiece.PossibleOrientations.Length; index++)
		{
			var bitmap = nextPiece.PossibleOrientations[index];
			for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
			{
				for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
				{
					if (board.CanPlace(bitmap, x, y))
					{
						placementCount += CalculatePlacementCount(board, bitmap, x, y, in possibleFuturePieces, (possibleFuturePiecesOffset + 1) % possibleFuturePieces.Count, lookAheadAmount - 1);
					}
				}
			}
		}

		return placementCount;
	}
}