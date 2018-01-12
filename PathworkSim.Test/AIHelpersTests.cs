using PatchworkSim;
using PatchworkSim.AI;
using Xunit;

namespace PathworkSim.Test
{
	public class AiHelpersTests
	{
		[Fact]
		public void ButtonIncomeAfter()
		{
			Assert.Equal(SimulationState.ButtonIncomeMarkers.Length, Helpers.ButtonIncomeAmountAfterPosition(0));
			Assert.Equal(SimulationState.ButtonIncomeMarkers.Length - 1, Helpers.ButtonIncomeAmountAfterPosition(SimulationState.ButtonIncomeMarkers[0]));
			Assert.Equal(0, Helpers.ButtonIncomeAmountAfterPosition(SimulationState.EndLocation));
		}
	}
}
