using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead
{
	/// <summary>
	/// An IPlacementStrategy that does not make use of the possibleFuturePieces when deciding where to place a piece
	/// </summary>
	public abstract class NoLookaheadStrategy : IPlacementStrategy
	{
		public abstract string Name { get; }

		public bool ImplementsLookahead => false;

		public bool TryPlacePiece(BoardState board, PieceDefinition piece, in PieceCollection possibleFuturePieces, int possibleFuturePiecesOffset, out PieceBitmap bitmap, out int x, out int y)
		{
			return TryPlacePiece(board, piece, out bitmap, out x, out y);
		}

		protected abstract bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap bitmap, out int x, out int y);
	}
}
