using PatchworkSim.AI.MoveMakers.UtilityCalculators;

namespace PatchworkSim.AI.MoveMakers
{
	public abstract class BaseUtilityMoveMaker : IMoveDecisionMaker
	{
		public abstract string Name { get; }

		public void MakeMove(SimulationState state)
		{
			int bestPiece = -1;
			double bestPieceValue = CalculateValueOfAdvancing(state);

			for (var i = 0; i < 3; i++)
			{
				var piece = Helpers.GetNextPiece(state, i);
				if (Helpers.ActivePlayerCanPurchasePiece(state, piece))
				{
					var value = CalculateValue(state, state.NextPieceIndex + i, piece);

					if (value > bestPieceValue)
					{
						bestPiece = i;
						bestPieceValue = value;
					}
				}
			}

			if (bestPiece == -1)
				state.PerformAdvanceMove();
			else
				state.PerformPurchasePiece(state.NextPieceIndex + bestPiece);
		}

		protected abstract double CalculateValueOfAdvancing(SimulationState state);

		protected abstract double CalculateValue(SimulationState state, int pieceIndex, PieceDefinition piece);
	}

	public class UtilityMoveMaker : BaseUtilityMoveMaker
	{
		public override string Name => $"Utility({_calculator.Name})";

		private readonly IUtilityCalculator _calculator;

		public UtilityMoveMaker(IUtilityCalculator calculator)
		{
			_calculator = calculator;
		}

		protected override double CalculateValueOfAdvancing(SimulationState state)
		{
			return _calculator.CalculateValueOfAdvancing(state);
		}

		protected override double CalculateValue(SimulationState state, int pieceIndex, PieceDefinition piece)
		{
			return _calculator.CalculateValueOfPurchasing(state, pieceIndex, piece);
		}
	}
}
