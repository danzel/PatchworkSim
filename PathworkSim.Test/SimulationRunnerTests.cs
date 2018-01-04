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
			var state = new SimulationState();
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
			Assert.Equal(SimulationState.EndLocation, state.PlayerButtonAmount[0]);
			Assert.Equal(SimulationState.EndLocation, state.PlayerButtonAmount[1]);

			//Starting player should have won by arriving at the end first
			Assert.Equal(0, state.WinningPlayer);
		}
    }
}
