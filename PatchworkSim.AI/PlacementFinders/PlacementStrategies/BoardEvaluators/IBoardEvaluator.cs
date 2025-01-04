namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators;

public interface IBoardEvaluator
{
	string Name { get; }

	/// <summary>
	/// Begin evaluation starting from the given board state.
	/// </summary>
	void BeginEvaluation(BoardState currentBoard);

	/// <summary>
	/// Evaluate the given board state (compared to the one provided in BeginEvaluation) and return a value. Larger values imply a better resulting board state
	/// </summary>
	int Evaluate(in BoardState board, int minX, int maxX, int minY, int maxY);
}
