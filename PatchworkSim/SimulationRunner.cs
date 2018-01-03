using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			//Player will decide to buy a piece or move in front of opponent
			//If they buy a piece, they need to place it (if simulation is running at high fidelity)
			_decisionMakers[_state.ActivePlayer].MoveDecisionMaker.MakeMove(_state);

			//Move the player
			//Remove buttons
			//Get them to place the piece

			//Check if the player gets buttons
			//Check if the player gets a leather patch
			//Check if the active player changes
		}
	}
}
