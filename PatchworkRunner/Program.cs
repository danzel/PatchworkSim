//#define PERF_PARALLEL
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PatchworkSim;
using PatchworkSim.AI.MoveMakers;
using PatchworkSim.AI.PlacementFinders;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;
using PatchworkSim.Loggers;

namespace PatchworkRunner
{
	class Program
	{
		static void Main(string[] args)
		{
			RunMoveMakerForPerformance();

			//ComparePlacementStrategies();
			//WatchAGame();

			//new GeneticTuneableUtilityEvolver().Run();
		}

		private static void RunMoveMakerForPerformance()
		{
			var sw = Stopwatch.StartNew();

#if PERF_PARALLEL
			Parallel.For(0, 10, (run) => //5900
			#else
			for (var run = 0; run < 10; run++) //18200
#endif
			{
				var a = new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(10000), PlacementMaker.FirstPossibleInstance);
				var b = new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(5000), PlacementMaker.FirstPossibleInstance);

				var state = new SimulationState(SimulationHelpers.GetRandomPieces(run / 2), 0);
				var runner = new SimulationRunner(state, run % 2 == 0 ? a : b, run % 2 == 1 ? a : b);
				while (!state.GameHasEnded)
				{
					runner.PerformNextStep();
				}
				Console.WriteLine(state.WinningPlayer);
			}
#if PERF_PARALLEL
			);
#endif
			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.ReadLine();
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
				TightPlacementStrategy.InstanceIncrement,
				ExhaustiveMostFuturePlacementsPlacementStrategy.Instance1_1,
				ExhaustiveMostFuturePlacementsPlacementStrategy.Instance1_6,
			};

			//ExhaustiveMostFuturePlacementsPlacementStrategy
			//3,1 - 2.4 seconds | 12 Pieces
			//4,1 - 46 seconds  | 12 Pieces
			//5,1 - 29 minutes  | 11 Pieces

			var pieces = SimulationHelpers.GetRandomPieces(2);

			var oldBoards = new BoardState[strategies.Length];
			var boards = new BoardState[strategies.Length];
			var placed = new int[strategies.Length];
			var stillPlacing = strategies.Select(s => true).ToArray();
			int stillPlacingCount = strategies.Length;

			//TODO: Should change this to advance semi-randomly like a real game would
			for (var index = 0; index < pieces.Count; index++)
			{
				var pieceIndex = pieces[index];
				var piece = PieceDefinition.AllPieceDefinitions[pieceIndex];
				Console.WriteLine($"Placing piece {index} - {piece.Name}");

				for (var i = 0; i < strategies.Length; i++)
				{
					if (!stillPlacing[i])
						continue;

					if (strategies[i].TryPlacePiece(boards[i], piece, pieces, index + 1, out var bitmap, out var x, out var y))
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
			var runner = new SimulationRunner(state,
				new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(1000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_1),
				new PlayerDecisionMaker(new QuickRandomSearchMoveMaker(6, 1000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_1));
			var logger = new ConsoleLogger(state);
			logger.PrintBoardsAfterPlacement = true;
			state.Logger = logger;

			while (!state.GameHasEnded)
			{
				runner.PerformNextStep();
				//Console.ReadLine();
			}

			Console.WriteLine($"Player {state.WinningPlayer} Won [{state.CalculatePlayerEndGameWorth(0)} | {state.CalculatePlayerEndGameWorth(1)}]");

			//logger.PrintBoards();

			Console.WriteLine("Press any key to exit");
			Console.ReadLine();
		}
	}
}