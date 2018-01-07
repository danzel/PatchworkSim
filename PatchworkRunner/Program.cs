using System;
using PatchworkSim;
using PatchworkSim.AI.MoveMakers;
using PatchworkSim.AI.PlacementFinders;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;
using PatchworkSim.Loggers;

namespace PatchworkRunner
{
	class Program
	{
		static void Main(string[] args)
		{
			ComparePlacementStrategies();
			//WatchAGame();
		}

		private static void ComparePlacementStrategies()
		{
			//State only exists so we can use the logger
			var state = new SimulationState(SimulationHelpers.GetRandomPieces(1), 0);
			var logger = new ConsoleLogger(state);

			var strategies = new IPlacementStrategy[]
			{
				FirstPossiblePlacementStrategy.Instance,
				SimpleClosestToWallAndCornerStrategy.Instance
			};

			var pieces = SimulationHelpers.GetRandomPieces(1);

			var placed = new[] { 0, 0 };
			var stillPlacing = new[] { true, true };

			for (var index = 0; index < pieces.Length; index++)
			{
				var pieceIndex = pieces[index];
				var piece = PieceDefinition.AllPieceDefinitions[pieceIndex];
				Console.WriteLine($"Placing piece {index} - {piece.Name}");

				for (var i = 0; i < 2; i++)
				{
					if (!stillPlacing[i])
						continue;
					if (strategies[i].TryPlacePiece(state.PlayerBoardState[i], piece, out var bitmap, out var x, out var y))
					{
						placed[i]++;
						BitmapOps.Place(state.PlayerBoardState[i], bitmap, x, y);
					}
					else
					{
						stillPlacing[i] = false;
					}
				}
				logger.PrintBoards(true);
				if (!stillPlacing[0] && !stillPlacing[1])
					break;
			}

			Console.ReadLine();
		}

		private static void WatchAGame()
		{
			var state = new SimulationState(SimulationHelpers.GetRandomPieces(1), 0);
			var runner = new SimulationRunner(state, new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, PlacementMaker.FirstPossibleInstance), new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, PlacementMaker.FirstPossibleInstance));
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