using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PatchworkSim;
using PatchworkSim.AI.MoveMakers;
using PatchworkSim.AI.PlacementFinders;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;
using PatchworkSim.Loggers;

namespace PatchworkAIComparer
{
	class Program
	{
		static void Main(string[] args)
		{
			//TestFullAi();

			TestPlacementOnly();
		}

		static void TestFullAi()
		{
			/*var aiToTest = new Func<PlayerDecisionMaker>[]
			{
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-3), PlacementMaker.FirstPossibleInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-2), PlacementMaker.FirstPossibleInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), PlacementMaker.FirstPossibleInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.FirstPossibleInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.FirstPossibleInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.FirstPossibleInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(3), PlacementMaker.FirstPossibleInstance),

				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-3), PlacementMaker.SimpleClosestToWallAndCornerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-2), PlacementMaker.SimpleClosestToWallAndCornerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), PlacementMaker.SimpleClosestToWallAndCornerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.SimpleClosestToWallAndCornerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.SimpleClosestToWallAndCornerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.SimpleClosestToWallAndCornerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(3), PlacementMaker.SimpleClosestToWallAndCornerInstance),

				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-3), PlacementMaker.ClosestToCornerLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-2), PlacementMaker.ClosestToCornerLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), PlacementMaker.ClosestToCornerLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.ClosestToCornerLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.ClosestToCornerLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.ClosestToCornerLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(3), PlacementMaker.ClosestToCornerLeastHolesTieBreakerInstance),

				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-3), PlacementMaker.NextToPieceEdgeLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-2), PlacementMaker.NextToPieceEdgeLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), PlacementMaker.NextToPieceEdgeLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.NextToPieceEdgeLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.NextToPieceEdgeLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.NextToPieceEdgeLeastHolesTieBreakerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(3), PlacementMaker.NextToPieceEdgeLeastHolesTieBreakerInstance),

				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-3), PlacementMaker.TightDoublerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-2), PlacementMaker.TightDoublerInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), PlacementMaker.TightDoublerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.TightDoublerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.TightDoublerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.TightDoublerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(3), PlacementMaker.TightDoublerInstance),

				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-3), PlacementMaker.TightIncrementInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-2), PlacementMaker.TightIncrementInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), PlacementMaker.TightIncrementInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.TightIncrementInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.TightIncrementInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.TightIncrementInstance),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(3), PlacementMaker.TightIncrementInstance),

				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-3), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_1),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-2), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_1),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_1),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_1),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_1),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_1),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(3), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_1),

				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-3), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-2), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				//() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(3), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),

				//() => new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, PlacementMaker.FirstPossibleInstance),
				//() => new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, PlacementMaker.SimpleClosestToWallAndCornerInstance),

				//() => new PlayerDecisionMaker(new RandomMoveMaker(), PlacementMaker.FirstPossibleInstance),
				//() => new PlayerDecisionMaker(new RandomMoveMaker(), PlacementMaker.SimpleClosestToWallAndCornerInstance),

				//() => new PlayerDecisionMaker(AlwaysAdvanceMoveMaker.Instance, PlacementMaker.FirstPossibleInstance)
			};*/

			var aiToTest = new Func<PlayerDecisionMaker>[]
			{
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(3), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),

				() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(1000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(5000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(10000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),

				() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(1000, TuneableUtilityMoveMaker.Tuning1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(5000, TuneableUtilityMoveMaker.Tuning1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(10000, TuneableUtilityMoveMaker.Tuning1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),

				() => new PlayerDecisionMaker(new QuickRandomSearchMoveMaker(6, 1000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new QuickRandomSearchMoveMaker(10, 2000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new QuickRandomSearchMoveMaker(20, 5000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new QuickRandomSearchMoveMaker(30, 10000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				
				() => new PlayerDecisionMaker(TuneableUtilityMoveMaker.Tuning1, PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
			};

			const int TotalRuns = 100;
			const bool enableConsoleLogging = false;

			//TODO: Play each AI against each other AI 100 times and print a table of results

			var totalWins = new int[aiToTest.Length, aiToTest.Length];
			Console.WriteLine($"Running {aiToTest.Length * (aiToTest.Length - 1) / 2} * {TotalRuns} Games");
			int gameNumber = 0;

			for (var a = 0; a < aiToTest.Length; a++)
			{
				for (var b = a; b < aiToTest.Length; b++)
				{
					if (a == b)
						continue;

					var aiA = aiToTest[a];
					var aiB = aiToTest[b];
					Console.WriteLine($"{++gameNumber} {aiA().Name} vs {aiB().Name}");

					Parallel.For(0, TotalRuns, (run) =>
							//for (var run = 0; run < TotalRuns; run++)
						{
							var state = new SimulationState(SimulationHelpers.GetRandomPieces(run / 2), 0);
							ConsoleLogger logger = null;
							if (enableConsoleLogging)
								state.Logger = logger = new ConsoleLogger(state);
							//state.Fidelity = SimulationFidelity.NoPiecePlacing;
							//Let each Ai have half of the goes first and half second
							var runner = new SimulationRunner(state, run % 2 == 0 ? aiA() : aiB(), run % 2 == 1 ? aiA() : aiB());

							while (!state.GameHasEnded)
							{
								runner.PerformNextStep();
								if (logger != null)
									logger.PrintBoards(true);
							}

							var aWin = run % 2 == state.WinningPlayer;

							//lock (totalWins)
							{
								if (aWin)
									totalWins[a, b]++;
								else
									totalWins[b, a]++;
							}
						}
					);
					Console.WriteLine(totalWins[a, b]);
				}
			}

			//Calculate the total points for each
			var total = Enumerable.Range(0, aiToTest.Length).Select(ai => Enumerable.Range(0, aiToTest.Length).Sum(opponent => totalWins[ai, opponent])).ToArray();

			//Dump a CSV with the results
			var filename = "result_" + DateTimeOffset.Now.Ticks + ".csv";
			var res = new List<string>();

			res.Add("," + string.Join(", ", aiToTest.Select(ai => ai().Name)) + ",Win%,Rank");
			for (var a = 0; a < aiToTest.Length; a++)
			{
				var line = aiToTest[a]().Name;
				for (var b = 0; b < aiToTest.Length; b++)
				{
					if (a == b)
						line += ",";
					else
						line += "," + totalWins[a, b];
				}

				//Win% and rank
				line += "," + (total[a] / (float)(aiToTest.Length - 1)).ToString("0.0");
				line += "," + (aiToTest.Length - total.Count(c => c < total[a]));

				res.Add(line);
			}

			Console.WriteLine("Saving results as " + filename);
			File.WriteAllLines(filename, res);

			Process.Start(filename);
		}

		static void TestPlacementOnly()
		{
			var stopwatch = Stopwatch.StartNew();

			var strategies = new IPlacementStrategy[]
			{
				FirstPossiblePlacementStrategy.Instance,
				SimpleClosestToWallAndCornerStrategy.Instance,
				ClosestToCornerLeastHolesTieBreakerPlacementStrategy.Instance,
				NextToPieceEdgeLeastHolesTieBreakerPlacementStrategy.Instance,
				TightPlacementStrategy.InstanceDoubler,
				TightPlacementStrategy.InstanceIncrement,
				ExhaustiveMostFuturePlacementsPlacementStrategy.Instance1_1,
				ExhaustiveMostFuturePlacementsPlacementStrategy.Instance1_6,

				new WeightedTreeSearchPlacementStrategy(new WeightedTreeSearchPlacementStrategy.TightPlacementWTSUF(true, 1), 1000, 4),
				new WeightedTreeSearchPlacementStrategy(new WeightedTreeSearchPlacementStrategy.TightPlacementWTSUF(true, 1), 1000, 8),
				new WeightedTreeSearchPlacementStrategy(new WeightedTreeSearchPlacementStrategy.TightPlacementWTSUF(true, 1), 10000, 4),
				new WeightedTreeSearchPlacementStrategy(new WeightedTreeSearchPlacementStrategy.TightPlacementWTSUF(true, 1), 10000, 8),

				new WeightedTreeSearchPlacementStrategy(new WeightedTreeSearchPlacementStrategy.TightPlacementWTSUF(false, 1), 1000, 4),
				new WeightedTreeSearchPlacementStrategy(new WeightedTreeSearchPlacementStrategy.TightPlacementWTSUF(false, 1), 1000, 8),
				new WeightedTreeSearchPlacementStrategy(new WeightedTreeSearchPlacementStrategy.TightPlacementWTSUF(false, 1), 10000, 4),
				new WeightedTreeSearchPlacementStrategy(new WeightedTreeSearchPlacementStrategy.TightPlacementWTSUF(false, 1), 10000, 8),

			};

			foreach (var strategy in strategies)
			{
				var rand = new Random(0);
				int totalPlaced = 0;

				for (var i = 0; i < 100; i++)
				{
					var pieces = SimulationHelpers.GetRandomPieces(i);
					int index = 0;
					int placed = 0;

					var board = new BoardState();

					while (true)
					{
						var piece = pieces[index];
						pieces.RemoveAt(index);
						index = index % pieces.Count;

						if (strategy.TryPlacePiece(board, PieceDefinition.AllPieceDefinitions[piece], pieces, index, out var bitmap, out var x, out var y))
						{
							placed++;
							board.Place(bitmap, x, y);

							//Advance to a random one in the next 6 pieces (TODO: Would be good to bias this towards 1-3 as these are more likely)
							//index = (index + rand.Next(0, 6)) % pieces.Count;
						}
						else
						{
							break;
						}
					}

					totalPlaced += placed;
				}

				Console.WriteLine($"{strategy.Name}    {totalPlaced}");
			}

			Console.WriteLine($"Time taken {stopwatch.ElapsedMilliseconds}");

			Console.ReadLine();
		}
	}
}