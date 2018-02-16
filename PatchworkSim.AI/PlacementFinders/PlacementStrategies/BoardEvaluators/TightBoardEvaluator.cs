using System;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators
{
	/// <summary>
	/// A BoardEvaluator that uses the TightPlacementStrategy scoring mechanism to calculate the score
	/// </summary>
	public class TightBoardEvaluator : IBoardEvaluator
	{
		public string Name => $"Tight({_doubler}@{_focusPower})";

		private readonly bool _doubler;
		private readonly double _focusPower;

		public TightBoardEvaluator(bool doubler, double focusPower)
		{
			_doubler = doubler;
			_focusPower = focusPower;
		}

		public double Evaluate(BoardState board)
		{
			TightPlacementStrategy.CalculateScore(board, _doubler, out var score);

			double result;
			if (_doubler)
				result = 1 - score / (Math.Pow(2, 8) * (BoardState.Width + BoardState.Height));
			else
				result = 1 - score / (double)(BoardState.Width * BoardState.Height * 2);

			if (result > 1 || result < 0)
				throw new Exception();

			return Math.Pow(result, _focusPower);
		}
	}
}
