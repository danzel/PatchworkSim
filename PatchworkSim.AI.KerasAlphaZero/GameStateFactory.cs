namespace PatchworkSim.AI.KerasAlphaZero;

static class GameStateFactory
{
	public const int PlayerObservations = 10;
	public const int LookAheadPieceAmount = 24;
	public const int PieceFields = 3;

	public const float PlayerPositionScale = SimulationState.EndLocation;
	public static readonly float IncomesRemainingScale = SimulationState.ButtonIncomeMarkers.Length;
	public const float ButtonIncomeScale = 20;
	public const float ButtonAmountScale = 20;

	public const float PieceButtonCostScale = 10;
	public const float PieceButtonIncomeScale = 4;
	public const float PieceUsedLocationsScale = 8;

	public const float UsedLocationsScale = BoardState.Width * BoardState.Height;

	public static GameState CreateGameState(SimulationState sim)
	{
		var res = new GameState();

		var ap = sim.ActivePlayer;
		var na = sim.NonActivePlayer;

		res.Observation.Add(sim.PlayerButtonIncome[ap] / ButtonIncomeScale);
		res.Observation.Add(sim.PlayerButtonIncome[na] / ButtonIncomeScale);
		res.Observation.Add(sim.PlayerButtonAmount[ap] / ButtonAmountScale);
		res.Observation.Add(sim.PlayerButtonAmount[na] / ButtonAmountScale);
		res.Observation.Add(sim.PlayerBoardUsedLocationsCount[ap] / UsedLocationsScale);
		res.Observation.Add(sim.PlayerBoardUsedLocationsCount[na] / UsedLocationsScale);
		res.Observation.Add(sim.PlayerPosition[ap] / PlayerPositionScale);
		res.Observation.Add(sim.PlayerPosition[na] / PlayerPositionScale);
		res.Observation.Add(SimulationHelpers.ButtonIncomeAmountAfterPosition(sim.PlayerPosition[ap]) / IncomesRemainingScale);
		res.Observation.Add(SimulationHelpers.ButtonIncomeAmountAfterPosition(sim.PlayerPosition[na]) / IncomesRemainingScale);

		for (var i = 0; i < LookAheadPieceAmount; i++)
		{
			var piece = Helpers.GetNextPiece(sim, i);

			res.Observation.Add(piece.ButtonCost / PieceButtonCostScale);
			res.Observation.Add(piece.ButtonsIncome / PieceButtonIncomeScale);
			res.Observation.Add(piece.TotalUsedLocations / PieceUsedLocationsScale);
		}

		return res;
	}
}
