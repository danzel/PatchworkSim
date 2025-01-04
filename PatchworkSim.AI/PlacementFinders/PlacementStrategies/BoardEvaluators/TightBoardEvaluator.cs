using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators;

/// <summary>
/// A BoardEvaluator that uses the TightPlacementStrategy scoring mechanism to calculate the score
/// </summary>
public class TightBoardEvaluator : IBoardEvaluator
{
	public string Name => $"Tight({_doubler})";

	private readonly bool _doubler;
	private readonly int[] _boardEvalX = new int[BoardState.Width];
	private readonly int[] _boardEvalY = new int[BoardState.Height];

	public TightBoardEvaluator(bool doubler)
	{
		_doubler = doubler;
	}

	public void BeginEvaluation(BoardState currentBoard)
	{
		for (var x = 0; x < BoardState.Width; x++)
			_boardEvalX[x] = EvaluateInternal(currentBoard, x, x + 1, 0, 0);
		for (var y = 0; y < BoardState.Height; y++)
			_boardEvalY[y] = EvaluateInternal(currentBoard, 0, 0, y, y + 1);
	}

	public int Evaluate(in BoardState board, int minX, int maxX, int minY, int maxY)
	{
		var utilityBefore = 0;
		for (var x = minX; x < maxX; x++)
			utilityBefore += _boardEvalX[x];
		for (var y = minY; y < maxY; y++)
			utilityBefore += _boardEvalY[y];

		return EvaluateInternal(board, minX, maxX, minY, maxY) - utilityBefore;
	}

	private int EvaluateInternal(BoardState board, int minX, int maxX, int minY, int maxY)
	{
		//Tight returns a larger score to mean worse, so reverse it
		return -TightPlacementStrategy.CalculateScore(board, _doubler, minX, maxX, minY, maxY);
	}
}
