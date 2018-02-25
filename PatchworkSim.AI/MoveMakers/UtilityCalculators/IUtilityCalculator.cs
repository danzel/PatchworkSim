namespace PatchworkSim.AI.MoveMakers.UtilityCalculators
{
	public interface IUtilityCalculator
	{
		string Name { get; }

		double CalculateValueOfAdvancing(SimulationState state);
		double CalculateValueOfPurchasing(SimulationState state, int pieceIndex, PieceDefinition piece);
	}
}
