﻿using System;
using System.Linq;
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
			//var state = new SimulationState(SimulationHelpers.GetRandomPieces(2), 0);
			//var logger = new ConsoleLogger(state);

			var strategies = new IPlacementStrategy[]
			{
				//FirstPossiblePlacementStrategy.Instance,
				//SimpleClosestToWallAndCornerStrategy.Instance,
				//SmallestBoundingBoxPlacementStrategy.Instance
				ClosestToCornerLeastHolesTieBreakerPlacementStrategy.Instance,
				NextToPieceEdgeLeastHolesTieBreakerPlacementStrategy.Instance,
				TightPlacementStrategy.InstanceDoubler,
				TightPlacementStrategy.InstanceIncrement
				//new NextToPieceEdgeAllowedHolesTieBreakerPlacementStrategy(15)
			};


			var pieces = SimulationHelpers.GetRandomPieces(2);

			var oldBoards = new BoardState[strategies.Length];
			var boards = new BoardState[strategies.Length];
			var placed = new int[strategies.Length];
			var stillPlacing = strategies.Select(s => true).ToArray();
			int stillPlacingCount = strategies.Length;

			for (var index = 0; index < pieces.Count; index++)
			{
				var pieceIndex = pieces[index];
				var piece = PieceDefinition.AllPieceDefinitions[pieceIndex];
				Console.WriteLine($"Placing piece {index} - {piece.Name}");

				for (var i = 0; i < strategies.Length; i++)
				{
					if (!stillPlacing[i])
						continue;
					if (strategies[i].TryPlacePiece(boards[i], piece, out var bitmap, out var x, out var y))
					{
						placed[i]++;
						boards[i].Place(bitmap, x, y);
					}
					else
					{
						stillPlacing[i] = false;
						stillPlacingCount--;
					}
				}

				ConsoleLogger.PrintBoardsDiff(oldBoards, boards);
				if (stillPlacingCount == 0)
					break;

				for (var i = 0; i < strategies.Length; i++)
					oldBoards[i] = boards[i];
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