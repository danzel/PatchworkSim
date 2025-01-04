using PatchworkSim.AI.MoveMakers.UtilityCalculators;

namespace PatchworkSim.AI.MoveMakers;

/// <summary>
/// A UtilityMoveMaker where the weights of each of the utility functions can be tuned.
/// </summary>
public class TuneableUtilityMoveMaker : BaseUtilityMoveMaker
{
	public static readonly TuneableUtilityMoveMaker Tuning1 = new TuneableUtilityMoveMaker(-0.03892573, 1, -0.593965087, -0.984706754, 0.554044008, -0.052553921, 0.609957017, -0.015045333, "Tuning1");

	public override string Name => $"TuneableUtility({_calculator.Name})";

	private readonly TuneableUtilityCalculator _calculator;

	/// <summary>
	/// Utility values should range from -1 to 1. They will be individually capped at that range after calculating them.
	/// </summary>
	public TuneableUtilityMoveMaker(double advancingPerButtonUtility, double usedLocationUtility, double buttonCostUtility, double timeCostUtility, double incomeUtility, double incomeSquaredUtility, double getAnotherTurnUtility, double receiveIncomeUtility, string name = null)
	{
		_calculator = new TuneableUtilityCalculator(advancingPerButtonUtility, usedLocationUtility, buttonCostUtility, timeCostUtility, incomeUtility, incomeSquaredUtility, getAnotherTurnUtility, receiveIncomeUtility, name);
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