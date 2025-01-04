using PatchworkSim;
using Xunit;

namespace PathworkSim.Test;

public class AiHelpersTests
{
	[Fact]
	public void ButtonIncomeAfter()
	{
		Assert.Equal(SimulationState.ButtonIncomeMarkers.Length, SimulationHelpers.ButtonIncomeAmountAfterPosition(0));
		Assert.Equal(SimulationState.ButtonIncomeMarkers.Length - 1, SimulationHelpers.ButtonIncomeAmountAfterPosition(SimulationState.ButtonIncomeMarkers[0]));
		Assert.Equal(0, SimulationHelpers.ButtonIncomeAmountAfterPosition(SimulationState.EndLocation));
	}
}
