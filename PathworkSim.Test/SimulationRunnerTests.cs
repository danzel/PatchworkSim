using PatchworkSim;
using Xunit;

namespace PathworkSim.Test
{
    public class SimulationRunnerTests
    {
		/// <summary>
		/// If both players use AlwaysAdvanceDecisionMaker, they should both advance all the way to the end and the game end
		/// </summary>
		[Fact]
		public void AlwaysAdvanceToEnd()
		{
			var state = new SimulationState(SimulationHelpers.GetRandomPieces(), 0);
			state.Fidelity = SimulationFidelity.NoPiecePlacing;
			var runner = new SimulationRunner(state, new PlayerDecisionMaker(AlwaysAdvanceDecisionMaker.Instance, null), new PlayerDecisionMaker(AlwaysAdvanceDecisionMaker.Instance, null));

			//The game definitely ends within 200 steps
			for (var i = 0; i < 200 && !state.GameHasEnded; i++)
			{
				runner.PerformNextStep();

				Assert.InRange(state.PlayerPosition[0], 0, SimulationState.EndLocation);
				Assert.InRange(state.PlayerPosition[1], 0, SimulationState.EndLocation);
			}

			//Both players should be at the end
			Assert.Equal(SimulationState.EndLocation, state.PlayerPosition[0]);
			Assert.Equal(SimulationState.EndLocation, state.PlayerPosition[1]);

			//Game should have ended
			Assert.True(state.GameHasEnded);

			//Players should have one button for each place they moved
			Assert.Equal(SimulationState.EndLocation + SimulationState.PlayerStartingButtons, state.PlayerButtonAmount[0]);
			Assert.Equal(SimulationState.EndLocation + SimulationState.PlayerStartingButtons, state.PlayerButtonAmount[1]);

			//non-starting player should have won by collecting all of the leather patches
			Assert.Equal(1, state.WinningPlayer);

			//Check the ending points are correct
			Assert.Equal(SimulationState.EndLocation - SimulationState.PlayerBoardSize * SimulationState.PlayerBoardSize * 2 + SimulationState.PlayerStartingButtons, state.CalculatePlayerEndGameWorth(0));
			Assert.Equal(SimulationState.EndLocation - SimulationState.PlayerBoardSize * SimulationState.PlayerBoardSize * 2 + SimulationState.PlayerStartingButtons + 2 * SimulationState.LeatherPatches.Length, state.CalculatePlayerEndGameWorth(1));
		}
	}
}
