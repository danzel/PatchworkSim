using System;

namespace PatchworkSim.AI.MoveMakers
{
	/// <summary>
	/// A UtilityMoveMaker where the weights of each of the utility functions can be tuned.
	/// </summary>
	public class TuneableUtilityMoveMaker : BaseUtilityMoveMaker
	{
		public readonly double AdvancingPerButtonUtility;
		public readonly double UsedLocationUtility;
		public readonly double ButtonCostUtility;
		public readonly double TimeCostUtility;
		public readonly double IncomeUtility;
		public readonly double IncomeSquaredUtility;
		public readonly double GetAnotherTurnUtility;
		public readonly double ReceiveIncomeUtility;

		public override string Name => $"TuneableUtility({AdvancingPerButtonUtility},{UsedLocationUtility},{ButtonCostUtility},{TimeCostUtility},{IncomeUtility},{IncomeSquaredUtility}, {GetAnotherTurnUtility},{ReceiveIncomeUtility})";

		/// <summary>
		/// Utility values should range from -1 to 1. They will be individually capped at that range after calculating them.
		/// </summary>
		public TuneableUtilityMoveMaker(double advancingPerButtonUtility, double usedLocationUtility, double buttonCostUtility, double timeCostUtility, double incomeUtility, double incomeSquaredUtility, double getAnotherTurnUtility, double receiveIncomeUtility)
		{
			AdvancingPerButtonUtility = advancingPerButtonUtility;
			UsedLocationUtility = usedLocationUtility;
			ButtonCostUtility = buttonCostUtility;
			TimeCostUtility = timeCostUtility;
			IncomeUtility = incomeUtility;
			IncomeSquaredUtility = incomeSquaredUtility;
			GetAnotherTurnUtility = getAnotherTurnUtility;
			ReceiveIncomeUtility = receiveIncomeUtility;
		}


		protected override double CalculateValueOfAdvancing(SimulationState state)
		{
			var distance = state.PlayerPosition[state.NonActivePlayer] - state.PlayerPosition[state.ActivePlayer] + 1;

			return AdvancingPerButtonUtility * distance; //TODO Clamp? Divide by total utilities?
		}

		protected override double CalculateValue(SimulationState state, PieceDefinition piece)
		{
			var value = Clamp(piece.TotalUsedLocations * UsedLocationUtility);

			value += Clamp(piece.ButtonCost * ButtonCostUtility);

			value += Clamp(piece.TimeCost * TimeCostUtility);

			//TODO: Should we have piece income and total income utilities?
			value += Clamp(Helpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[state.ActivePlayer]) * IncomeUtility);

			value += Clamp(Helpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[state.ActivePlayer]) * Helpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[state.ActivePlayer]) * IncomeSquaredUtility);

			//TODO: Should this be boolean or vary by difference in location?
			if (state.PlayerPosition[state.NonActivePlayer] > (state.PlayerPosition[state.ActivePlayer] + piece.TimeCost))
				value += GetAnotherTurnUtility;

			//TODO: Should this be boolean or vary by income amount?
			if (Helpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[state.ActivePlayer]) != Helpers.ButtonIncomeAmountAfterPosition(Math.Min(SimulationState.EndLocation, state.PlayerPosition[state.ActivePlayer] + piece.TimeCost)))
				value += ReceiveIncomeUtility;

			return value; //TODO Clamp? Divide by total utilities?
		}

		private double Clamp(double value)
		{
			return Math.Min(1, Math.Max(-1, value));
		}
	}
}