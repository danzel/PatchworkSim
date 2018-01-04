namespace PatchworkSim
{
	public class SimulationRunner
	{
		private readonly SimulationState _state;
		private readonly PlayerDecisionMaker[] _decisionMakers;

		public SimulationRunner(SimulationState state, PlayerDecisionMaker player1, PlayerDecisionMaker player2)
		{
			_state = state;
			_decisionMakers = new[] { player1, player2 };
		}

		/// <summary>
		/// Does the next step required - player makes a decision and does a thing
		/// </summary>
		public void PerformNextStep()
		{
			if (_state.PieceToPlace == null)
			{
				//Player will decide to buy a piece or move in front of opponent
				//If they buy a piece, they need to place it (if simulation is running at high fidelity)
				_decisionMakers[_state.ActivePlayer].MoveDecisionMaker.MakeMove(_state);
			}
			else
			{
				//Player must place the piece
				_decisionMakers[_state.PieceToPlacePlayer].PlacementDecisionMaker.PlacePiece(_state);
			}
		}
	}
}
