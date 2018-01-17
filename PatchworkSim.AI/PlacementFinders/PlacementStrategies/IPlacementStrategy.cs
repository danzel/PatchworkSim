using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	public interface IPlacementStrategy
	{
		string Name { get; }

		/// <summary>
		/// True if TryPlacePieceAdvanced should be used
		/// </summary>
		bool ImplementsAdvanced { get; }

		/// <summary>
		/// Places the given piece on the board. Returns true if successful.
		/// As a rule of thumb strategies should start placing near 0,0 (as Helpers.CanPlace starts at 8,8 - making CanPlace more efficient)
		/// </summary>
		bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap bitmap, out int x, out int y);

		/// <summary>
		/// Places the given piece on the board. Returns true if successful.
		/// As a rule of thumb strategies should start placing near 0,0 (as Helpers.CanPlace starts at 8,8 - making CanPlace more efficient)
		/// </summary>
		bool TryPlacePieceAdvanced(BoardState board, PieceDefinition piece, List<int> possibleFuturePieces, int possibleFuturePiecesOffset, out PieceBitmap bitmap, out int x, out int y);
	}
}