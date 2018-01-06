using System;
using PatchworkSim;
using PatchworkSim.AI;
using PatchworkSim.AI.MoveMakers;
using PatchworkSim.AI.PlacementFinders;
using PatchworkSim.Loggers;

namespace PatchworkRunner
{
	class Program
	{
		static void Main(string[] args)
		{
			var state = new SimulationState(SimulationHelpers.GetRandomPieces(1), 0);
			var runner = new SimulationRunner(state, new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, FirstPossiblePlacementMaker.Instance), new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, FirstPossiblePlacementMaker.Instance));
			var logger = new ConsoleLogger(state);
			logger.PrintBoardsAfterPlacement = true;
			state.Logger = logger;

			while (!state.GameHasEnded)
			{
				runner.PerformNextStep();
				Console.ReadLine();
			}

			Console.WriteLine($"Player {state.WinningPlayer} Won");

			//logger.PrintBoards();

			Console.WriteLine("Press any key to exit");
			Console.ReadLine();
		}
	}
}