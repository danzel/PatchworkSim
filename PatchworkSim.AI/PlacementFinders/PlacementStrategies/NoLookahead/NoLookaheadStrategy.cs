using System.Diagnostics.CodeAnalysis;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;

/// <summary>
/// An IPlacementStrategy that does not make use of the possibleFuturePieces when deciding where to place a piece
/// </summary>
public abstract class NoLookaheadStrategy : IPlacementStrategy
{
	public abstract string Name { get; }

	public bool ImplementsLookahead => false;

	public bool TryPlacePiece(BoardState board, PieceDefinition piece, in PieceCollection possibleFuturePieces, int possibleFuturePiecesOffset, [NotNullWhen(true)] out PieceBitmap? resultBitmap, out int x, out int y)
	{
		return TryPlacePiece(board, piece, out resultBitmap, out x, out y);
	}

	protected abstract bool TryPlacePiece(BoardState board, PieceDefinition piece, [NotNullWhen(true)] out PieceBitmap? resultBitmap, out int x, out int y);
}
