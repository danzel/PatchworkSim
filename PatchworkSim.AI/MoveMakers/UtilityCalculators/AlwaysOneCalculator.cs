namespace PatchworkSim.AI.MoveMakers.UtilityCalculators
{
	public class AlwaysOneCalculator : IUtilityCalculator
	{
		public static AlwaysOneCalculator Instance = new AlwaysOneCalculator();

		public string Name => "one";
		public double CalculateValueOfAdvancing(SimulationState state)
		{
			return 1;
		}

		public double CalculateValueOfPurchasing(SimulationState state, int pieceIndex, PieceDefinition piece)
		{
			return 1;
		}
	}
}
