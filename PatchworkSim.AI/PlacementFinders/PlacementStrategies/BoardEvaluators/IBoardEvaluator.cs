namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators
{
	public interface IBoardEvaluator
	{
		string Name { get; }

		/// <summary>
		/// Evaluate the given board state and return a value from 0-1, where 0 is bad and 1 is good
		/// </summary>
		double Evaluate(BoardState board);
	}
}
