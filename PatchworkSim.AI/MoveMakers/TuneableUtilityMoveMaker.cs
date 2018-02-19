using System;

namespace PatchworkSim.AI.MoveMakers
{
	/// <summary>
	/// A UtilityMoveMaker where the weights of each of the utility functions can be tuned.
	/// </summary>
	public class TuneableUtilityMoveMaker : BaseUtilityMoveMaker
	{
		public static readonly TuneableUtilityMoveMaker Tuning1 = new TuneableUtilityMoveMaker(-0.03892573, 1, -0.593965087, -0.984706754, 0.554044008, -0.052553921, 0.609957017, -0.015045333, "Tuning1");

		public readonly double AdvancingPerButtonUtility;
		public readonly double UsedLocationUtility;
		public readonly double ButtonCostUtility;
		public readonly double TimeCostUtility;
		public readonly double IncomeUtility;
		public readonly double IncomeSquaredUtility;
		public readonly double GetAnotherTurnUtility;
		public readonly double ReceiveIncomeUtility;
		private readonly string _name;

		public override string Name
		{
			get
			{
				if (_name != null)
					return _name;

				var max =
					Math.Max(
						Math.Max(
							Math.Max(Math.Abs(AdvancingPerButtonUtility), Math.Abs(UsedLocationUtility)),
							Math.Max(Math.Abs(ButtonCostUtility), Math.Abs(TimeCostUtility))),
						Math.Max(Math.Max(Math.Abs(IncomeUtility), Math.Abs(IncomeSquaredUtility)),
							Math.Max(Math.Abs(GetAnotherTurnUtility), Math.Abs(ReceiveIncomeUtility))));

				return $"TuneableUtility({AdvancingPerButtonUtility / max}|{UsedLocationUtility / max}|{ButtonCostUtility / max}|{TimeCostUtility / max}|{IncomeUtility / max}|{IncomeSquaredUtility / max}|{GetAnotherTurnUtility / max}|{ReceiveIncomeUtility / max})";
			}
		}

		/// <summary>
		/// Utility values should range from -1 to 1. They will be individually capped at that range after calculating them.
		/// </summary>
		public TuneableUtilityMoveMaker(double advancingPerButtonUtility, double usedLocationUtility, double buttonCostUtility, double timeCostUtility, double incomeUtility, double incomeSquaredUtility, double getAnotherTurnUtility, double receiveIncomeUtility, string name = null)
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


		protected override double CalculateValueOfAdvancing(SimulationState state)
		{
			var distance = state.PlayerPosition[state.NonActivePlayer] - state.PlayerPosition[state.ActivePlayer] + 1;

			return AdvancingPerButtonUtility * distance; //TODO Clamp? Divide by total utilities?
		}

		protected override double CalculateValue(SimulationState state, int pieceIndex, PieceDefinition piece)
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
}