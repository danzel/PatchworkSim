namespace PatchworkSim.AI.MoveMakers
{
	/// <summary>
	/// The is a Greedy (no look-ahead) utility AI designed to always pick the move that gives the most payoff based on pure card value
	/// </summary>
	public class GreedyCardValueUtilityMoveMaker : BaseUtilityMoveMaker
	{
		public override string Name => $"GreedyCardValue({_timeCostValue})";

		private readonly int _timeCostValue;

		/// <param name="timeCostValue">The value that each time cost on a card costs us (pass a positive value, it is subtracted away)</param>
		public GreedyCardValueUtilityMoveMaker(int timeCostValue)
		{
			_timeCostValue = timeCostValue;
		}

		protected override double CalculateValueOfAdvancing(SimulationState state)
		{
			return 0;
		}

		protected override double CalculateValue(SimulationState state, int pieceIndex, PieceDefinition piece)
		{
			var value = piece.TotalUsedLocations * 2 - piece.ButtonCost - piece.TimeCost * _timeCostValue;
			value += SimulationHelpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[state.ActivePlayer]) * piece.ButtonsIncome;
			//TODO: We should value gaining buttons more near the start of the game because they let us buy more, not sure if we need to put some extra stuff in for that

			return value;
		}
	}
}
