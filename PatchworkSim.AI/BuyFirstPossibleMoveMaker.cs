namespace PatchworkSim.AI
{
	/// <summary>
	/// An IMoveDecisionMaker that always purchases the first possible piece it can afford (possibly not place), otherwise does the advance move
	/// </summary>
    public class BuyFirstPossibleMoveMaker : IMoveDecisionMaker
    {
	    public static readonly BuyFirstPossibleMoveMaker Instance = new BuyFirstPossibleMoveMaker();

		private BuyFirstPossibleMoveMaker()
		{
		}

	    public void MakeMove(SimulationState state)
	    {
			for (var i = 0; i < 3; i++)
			{
				//TODO: Refactor this out
				var piece = PieceDefinition.AllPieceDefinitions[state.Pieces[(state.NextPieceIndex + i) % state.Pieces.Count]];

				//TODO: Refactor this check out (can purchase lazy edition)
				if (piece.TotalUsedLocations < SimulationState.PlayerBoardSize * SimulationState.PlayerBoardSize - state.PlayerBoardUsedLocationsCount[state.ActivePlayer] && piece.ButtonCost < state.PlayerButtonAmount[state.ActivePlayer])
				{
					state.PerformPurchasePiece(state.NextPieceIndex + i);
					return;
				}
			}

		    state.PerformAdvanceMove();
	    }
    }
}
