namespace PatchworkSim.AI.MoveMakers
{
	/// <summary>
	/// The is a Greedy (no look-ahead) utility AI designed to always pick the move that gives the most payoff based on pure card value
	/// </summary>
	public class GreedyCardValueUtilityMoveMaker : IMoveDecisionMaker
	{
		private readonly int _timeCostValue;

		/// </summary>
		/// <param name="timeCostValue">The value that each time cost on a card costs us (pass a positive value, it is subtracted away)</param>
		public GreedyCardValueUtilityMoveMaker(int timeCostValue)
		{
			_timeCostValue = timeCostValue;
		}

		public void MakeMove(SimulationState state)
		{
			int bestPiece = -1;
			int bestPieceValue = 0;

			for (var i = 0; i < 3; i++)
			{
				var piece = Helpers.GetNextPiece(state, i);
				if (Helpers.ActivePlayerCanPurchasePiece(state, piece))
				{
					var value = CalculateValue(state, piece);

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

		private int CalculateValue(SimulationState state, PieceDefinition piece)
		{
			var value = piece.TotalUsedLocations * 2 - piece.ButtonCost - piece.TimeCost * _timeCostValue;
			value += Helpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[state.ActivePlayer]) * piece.ButtonsIncome;
			//TODO: We should value gaining buttons more near the start of the game because they let us buy more, not sure if we need to put some extra stuff in for that

			return value;
		}

		public string Name => $"GreedyCardValue({_timeCostValue})";
	}
}
