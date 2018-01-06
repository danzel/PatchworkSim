namespace PatchworkSim.AI.MoveMakers
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

				if (Helpers.ActivePlayerCanPurchasePiece(state, piece))
				{
					state.PerformPurchasePiece(state.NextPieceIndex + i);
					return;
				}
			}

		    state.PerformAdvanceMove();
	    }
    }
}
