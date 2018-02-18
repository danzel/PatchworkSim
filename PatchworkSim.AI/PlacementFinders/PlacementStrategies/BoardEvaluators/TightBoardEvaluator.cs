using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators
{
	/// <summary>
	/// A BoardEvaluator that uses the TightPlacementStrategy scoring mechanism to calculate the score
	/// </summary>
	public class TightBoardEvaluator : IBoardEvaluator
	{
		public string Name => $"Tight({_doubler})";

		private readonly bool _doubler;

		public TightBoardEvaluator(bool doubler)
		{
			_doubler = doubler;
		}

		public int Evaluate(BoardState board, int minX, int maxX, int minY, int maxY)
		{
			//Tight returns a larger score to mean worse, so reverse it
			return -TightPlacementStrategy.CalculateScore(board, _doubler, minX, maxX, minY, maxY);
		}
	}
}
