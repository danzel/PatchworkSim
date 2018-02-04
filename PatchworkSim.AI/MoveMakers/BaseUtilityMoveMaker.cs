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

		protected abstract double CalculateValueOfAdvancing(SimulationState state);

		protected abstract double CalculateValue(SimulationState state, PieceDefinition piece);
	}
}
