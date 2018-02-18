using System;
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

		private static readonly double MaxDoublerScore = Math.Pow(2, 8) * (BoardState.Width + BoardState.Height);
		private const double MaxIncrementScore = BoardState.Width * BoardState.Height * 2;

		public double Evaluate(BoardState board)
		{
			TightPlacementStrategy.CalculateScore(board, _doubler, out var score);

			double result;
			if (_doubler)
				result = 1 - score / MaxDoublerScore;
			else
				result = 1 - score / MaxIncrementScore;

			if (result > 1 || result < 0)
				throw new Exception();

			return result;
		}
	}
}
