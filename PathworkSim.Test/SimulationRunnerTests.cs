using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			Assert.True(state.GameHasEnded);

			//TODO: Button Amount checks
			Assert.Equal(SimulationState.EndLocation, state.PlayerButtonAmount[0]);
			Assert.Equal(SimulationState.EndLocation, state.PlayerButtonAmount[1]);
		}
    }
}
