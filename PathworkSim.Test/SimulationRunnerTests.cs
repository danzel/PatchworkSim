using PatchworkSim;
using PatchworkSim.AI.MoveMakers;
using PatchworkSim.AI.PlacementFinders;
using Xunit;

namespace PathworkSim.Test;

public class SimulationRunnerTests
{
	/// <summary>
	/// If both players use AlwaysAdvanceMoveMaker, they should both advance all the way to the end and the game end
	/// </summary>
	[Fact]
	public void AlwaysAdvanceToEndNoPiecePlacing()
	{
		var state = new SimulationState(SimulationHelpers.GetRandomPieces(), 0);
		state.Fidelity = SimulationFidelity.NoPiecePlacing;
		var runner = new SimulationRunner(state, new PlayerDecisionMaker(AlwaysAdvanceMoveMaker.Instance, null), new PlayerDecisionMaker(AlwaysAdvanceMoveMaker.Instance, null));

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

		//non-starting player should have won by collecting all of the leather patches (https://boardgamegeek.com/thread/1703957/how-break-stalemate)
		Assert.Equal(1, state.WinningPlayer);

		//Check the ending points are correct
		Assert.Equal(SimulationState.EndLocation - BoardState.Width * BoardState.Height * 2 + SimulationState.PlayerStartingButtons, state.CalculatePlayerEndGameWorth(0));
		Assert.Equal(SimulationState.EndLocation - BoardState.Width * BoardState.Height * 2 + SimulationState.PlayerStartingButtons + 2 * SimulationState.LeatherPatches.Length, state.CalculatePlayerEndGameWorth(1));
	}

	[Fact]
	public void BuyFirstPossibleMoveMakerNoPiecePlacing()
	{
		var state = new SimulationState(SimulationHelpers.GetRandomPieces(3), 0);
		state.Fidelity = SimulationFidelity.NoPiecePlacing;
		var runner = new SimulationRunner(state, new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, null), new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, null));

		while (!state.GameHasEnded)
		{
			runner.PerformNextStep();
		}

		//Check someone bought something
		Assert.True(state.Pieces.Count < PieceDefinition.AllPieceDefinitions.Length);
	}

	[Fact]
	public void AdvanceToEndPlacingSinglePatches()
	{
		var state = new SimulationState(SimulationHelpers.GetRandomPieces(), 0);
		state.Fidelity = SimulationFidelity.FullSimulation;
		var runner = new SimulationRunner(state, new PlayerDecisionMaker(AlwaysAdvanceMoveMaker.Instance, PlacementMaker.FirstPossibleInstance), new PlayerDecisionMaker(AlwaysAdvanceMoveMaker.Instance, PlacementMaker.FirstPossibleInstance));

		while (!state.GameHasEnded)
		{
			runner.PerformNextStep();
		}

		//Both players should be at the end
		Assert.Equal(SimulationState.EndLocation, state.PlayerPosition[0]);
		Assert.Equal(SimulationState.EndLocation, state.PlayerPosition[1]);

		//Game should have ended
		Assert.True(state.GameHasEnded);

		//Players should have one button for each place they moved
		Assert.Equal(SimulationState.EndLocation + SimulationState.PlayerStartingButtons, state.PlayerButtonAmount[0]);
		Assert.Equal(SimulationState.EndLocation + SimulationState.PlayerStartingButtons, state.PlayerButtonAmount[1]);

		//non-starting player should have won by collecting all of the leather patches (https://boardgamegeek.com/thread/1703957/how-break-stalemate)
		Assert.Equal(1, state.WinningPlayer);

		//Check the ending points are correct
		Assert.Equal(SimulationState.EndLocation - BoardState.Width * BoardState.Height * 2 + SimulationState.PlayerStartingButtons, state.CalculatePlayerEndGameWorth(0));
		Assert.Equal(SimulationState.EndLocation - BoardState.Width * BoardState.Height * 2 + SimulationState.PlayerStartingButtons + 2 * SimulationState.LeatherPatches.Length, state.CalculatePlayerEndGameWorth(1));

		//Check the pieces are on their board
		int sum = 0;
		for (var x = 0; x < BoardState.Width; x++)
		{
			for (var y = 0; y < BoardState.Height; y++)
			{
				if (state.PlayerBoardState[1][x, y])
					sum++;
			}
		}
		Assert.Equal(SimulationState.LeatherPatches.Length, sum);
	}

	[Fact]
	public void BuyFirstPossibleMoveMakerPlacing()
	{
		var state = new SimulationState(SimulationHelpers.GetRandomPieces(1), 0);
		var runner = new SimulationRunner(state, new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, PlacementMaker.FirstPossibleInstance), new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, PlacementMaker.FirstPossibleInstance));

		while (!state.GameHasEnded)
		{
			runner.PerformNextStep();
		}

		//Check someone bought something
		Assert.True(state.Pieces.Count < PieceDefinition.AllPieceDefinitions.Length);

		Assert.Equal(11, state.PlayerButtonIncome[0]);
		Assert.Equal(37, state.PlayerButtonAmount[0]);
	}
}