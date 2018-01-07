namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	public interface IPlacementStrategy
	{
		string Name { get; }

		/// <summary>
		/// Places the given piece on the board. Returns true if successful.
		/// As a rule of thumb strategies should start placing near 0,0 (as Helpers.CanPlace starts at 8,8 - making CanPlace more efficient)
		/// </summary>
		bool TryPlacePiece(bool[,] board, PieceDefinition piece, out bool[,] bitmap, out int x, out int y);
	}
}