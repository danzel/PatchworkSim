using System;

namespace PatchworkSim.AI.MoveMakers.UtilityCalculators;

/// <summary>
/// Generally TuneableByBoardPosition is better
/// </summary>
public class TuneableUtilityCalculator : IUtilityCalculator
{
	public static readonly TuneableUtilityCalculator Tuning1 = new TuneableUtilityCalculator(-0.03892573, 1, -0.593965087, -0.984706754, 0.554044008, -0.052553921, 0.609957017, -0.015045333, "Tuning1");

	public string Name => $"TU-{_name}";

	public readonly double AdvancingPerButtonUtility;
	public readonly double UsedLocationUtility;
	public readonly double ButtonCostUtility;
	public readonly double TimeCostUtility;
	public readonly double IncomeUtility;
	public readonly double IncomeSquaredUtility;
	public readonly double GetAnotherTurnUtility;
	public readonly double ReceiveIncomeUtility;

	private readonly string? _name;

	public TuneableUtilityCalculator(double advancingPerButtonUtility, double usedLocationUtility, double buttonCostUtility, double timeCostUtility, double incomeUtility, double incomeSquaredUtility, double getAnotherTurnUtility, double receiveIncomeUtility, string? name = null)
	{
		AdvancingPerButtonUtility = advancingPerButtonUtility;
		UsedLocationUtility = usedLocationUtility;
		ButtonCostUtility = buttonCostUtility;
		TimeCostUtility = timeCostUtility;
		IncomeUtility = incomeUtility;
		IncomeSquaredUtility = incomeSquaredUtility;
		GetAnotherTurnUtility = getAnotherTurnUtility;
		ReceiveIncomeUtility = receiveIncomeUtility;

		_name = name;
	}

	public double CalculateValueOfAdvancing(SimulationState state)
	{
		var distance = state.PlayerPosition[state.NonActivePlayer] - state.PlayerPosition[state.ActivePlayer] + 1;

		return AdvancingPerButtonUtility * distance; //TODO Clamp? Divide by total utilities?

	}

	public double CalculateValueOfPurchasing(SimulationState state, int pieceIndex, PieceDefinition piece)
	{
		var value = piece.TotalUsedLocations * UsedLocationUtility;

		value += piece.ButtonCost * ButtonCostUtility;

		value += piece.TimeCost * TimeCostUtility;

		//TODO: Should we have piece income and total income utilities?
		value += SimulationHelpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[state.ActivePlayer]) * piece.ButtonsIncome * IncomeUtility;

		value += SimulationHelpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[state.ActivePlayer]) * piece.ButtonsIncome * piece.ButtonsIncome * IncomeSquaredUtility;

		//TODO: Should this be boolean or vary by difference in location?
		if (state.PlayerPosition[state.NonActivePlayer] >= (state.PlayerPosition[state.ActivePlayer] + piece.TimeCost))
			value += GetAnotherTurnUtility;

		//TODO: Should this be boolean or vary by income amount?
		if (SimulationHelpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[state.ActivePlayer]) != SimulationHelpers.ButtonIncomeAmountAfterPosition(Math.Min(SimulationState.EndLocation, state.PlayerPosition[state.ActivePlayer] + piece.TimeCost)))
			value += ReceiveIncomeUtility;

		return value; //TODO Clamp? Divide by total utilities?
	}
}
