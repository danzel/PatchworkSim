namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators
{
	public interface IBoardEvaluator
	{
		string Name { get; }

		/// <summary>
		/// Evaluate the given board state and return a value. Larger values are better than smaller ones
		/// </summary>
		int Evaluate(BoardState board, int minX, int maxX, int minY, int maxY);
	}
}
