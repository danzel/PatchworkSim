//#define PERF_PARALLEL

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PatchworkSim;
using PatchworkSim.AI.MoveMakers;
using PatchworkSim.AI.PlacementFinders;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;
using PatchworkSim.Loggers;

namespace PatchworkRunner
{
	class Program
	{
		static void Main(string[] args)
		{
			//RunMoveMakerForPerformance();

			//RunPlacementForPerformance();

			//ComparePlacementStrategies();
			//WatchAGame();

			//new GeneticTuneableUtilityEvolver().Run();

			CompareTwoAi();
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

		private static void RunPlacementForPerformance()
		{
			var sw = Stopwatch.StartNew();
			int totalPlaced = 0;

#if PERF_PARALLEL
			Parallel.For(0, 16, (run) => //5900
#else
			for (var run = 0; run < 10; run++) //18200
#endif
			{
				var strategy = ExhaustiveMostFuturePlacementsPlacementStrategy.Instance1_6;
				//var strategy = TightPlacementStrategy.InstanceIncrement;

				var board = new BoardState();

				var pieces = SimulationHelpers.GetRandomPieces(run);
				int myPlaced = 0;
				for (var i = 0; i < pieces.Count; i++)
				{
					if (strategy.TryPlacePiece(board, PieceDefinition.AllPieceDefinitions[pieces[i]], pieces, i + 1, out var bitmap, out var x, out var y))
					{
						board.Place(bitmap, x, y);
						myPlaced++;
					}
					else
					{
						break;
					}
				}
				Console.WriteLine(myPlaced);
				Interlocked.Add(ref totalPlaced, myPlaced);
			}
#if PERF_PARALLEL
			);
#endif
			Console.WriteLine($"TotalPlaced: {totalPlaced}");
			Console.WriteLine($"Millisecods: {sw.ElapsedMilliseconds}");
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
				//ClosestToCornerLeastHolesTieBreakerPlacementStrategy.Instance,
				//NextToPieceEdgeLeastHolesTieBreakerPlacementStrategy.Instance,
				//TightPlacementStrategy.InstanceDoubler,
				//TightPlacementStrategy.InstanceIncrement,
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
			var p = new PreplacerStrategy(new ExhaustiveMostFuturePlacementsPreplacer(3));
			//var p = new PreplacerStrategy(new WeightedTreeSearchPreplacer(new WeightedTreeSearchPreplacer.TightPlacementWTSUF(true, 1), 10000, 2));
			var m = new MoveOnlyMonteCarloTreeSearchWithPreplacerMoveMaker(10000, TuneableUtilityMoveMaker.Tuning1, p);
			var pdm = new PlayerDecisionMaker(m, new PlacementMaker(p));

			var state = new SimulationState(SimulationHelpers.GetRandomPieces(4), 0);
			var runner = new SimulationRunner(state
				, pdm
				, new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(10000, TuneableUtilityMoveMaker.Tuning1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6)
			);
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

		private static void CompareTwoAi()
		{
			PlayerDecisionMaker AiA() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(10000, TuneableUtilityMoveMaker.Tuning1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6);

			PlayerDecisionMaker AiB()
			{
				var p = new PreplacerStrategy(new ExhaustiveMostFuturePlacementsPreplacer(3));
				//var p = new PreplacerStrategy(new WeightedTreeSearchPreplacer(new WeightedTreeSearchPreplacer.TightPlacementWTSUF(true, 1), 10000, 2));
				var m = new MoveOnlyMonteCarloTreeSearchWithPreplacerMoveMaker(10000, TuneableUtilityMoveMaker.Tuning1, p);
				return new PlayerDecisionMaker(m, new PlacementMaker(p));
			}

			const int TotalRuns = 100;

			//TODO: Play each AI against each other AI 100 times and print a table of results

			long aTicks = 0;
			long bTicks = 0;

			for (var run = 0; run < TotalRuns / 2; run++)
			{
				Console.WriteLine($"Run {run}");
				int aWins = 0;
				int bWins = 0;

				Parallel.For(0, 2, starter =>
						//for (var starter = 0; starter < 2; starter++)
					{
						var state = new SimulationState(SimulationHelpers.GetRandomPieces(run), 0);

						//Let each Ai have half of the goes first and half second
						var runner = new SimulationRunner(state, starter == 0 ? AiA() : AiB(), starter == 1 ? AiA() : AiB());

						while (!state.GameHasEnded)
							runner.PerformNextStep();

						var aWin = starter % 2 == state.WinningPlayer;

						if (aWin)
							Interlocked.Increment(ref aWins);
						else
							Interlocked.Increment(ref bWins);

						Interlocked.Add(ref aTicks, runner.Stopwatches[starter == 0 ? 0 : 1].ElapsedTicks);
						Interlocked.Add(ref bTicks, runner.Stopwatches[starter == 0 ? 1 : 0].ElapsedTicks);

						Console.WriteLine($"Starter {starter}, winner {(state.WinningPlayer + starter) % 2}");
					}
				);

				if (aWins != bWins)
				{
					Console.WriteLine($"^^^ Not Equal ^^^ A Winner: {aWins > bWins}");
				}
				else
				{
					Console.WriteLine("Draw");
				}
			}

			Console.WriteLine($"Total Time {aTicks * 1000 / Stopwatch.Frequency} / {bTicks * 1000 / Stopwatch.Frequency}");

			Console.ReadLine();
		}
	}
}