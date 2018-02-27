using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace PatchworkAIComparer
{
	class Program
	{
		static void Main(string[] args)
		{
			TestFullAi();

			//TestPlacementOnly();
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
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), PlacementMaker.TightDoublerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.TightDoublerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.TightDoublerInstance),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.TightDoublerInstance),

				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				//
				//() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(1000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				//() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(5000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				//() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(10000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),

				() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(1000, TuneableUtilityMoveMaker.Tuning1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				//() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(5000, TuneableUtilityMoveMaker.Tuning1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(10000, TuneableUtilityMoveMaker.Tuning1), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() =>
				{
					var p = new PreplacerStrategy(new WeightedTreeSearchPreplacer(new TightBoardEvaluator(true), 10000, 2));
					var m = new MoveOnlyMonteCarloTreeSearchWithPreplacerMoveMaker(10000, TuneableUtilityMoveMaker.Tuning1, p);
					return new PlayerDecisionMaker(m, new PlacementMaker(p));
				},
				() =>
				{
					var p = new PreplacerStrategy(new ExhaustiveMostFuturePlacementsPreplacer(3));
					var m = new MoveOnlyMonteCarloTreeSearchWithPreplacerMoveMaker(10000, TuneableUtilityMoveMaker.Tuning1, p);
					return new PlayerDecisionMaker(m, new PlacementMaker(p));
				},

				//() => new PlayerDecisionMaker(new MoveOnlyMonteCarloTreeSearchMoveMaker(10000, TuneableUtilityMoveMaker.Tuning1), new PlacementMaker(new WeightedTreeSearchPlacementStrategy(new WeightedTreeSearchPlacementStrategy.TightPlacementWTSUF(true, 1), 10000, 2)))

				() => new PlayerDecisionMaker(new QuickRandomSearchMoveMaker(6, 1000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new QuickRandomSearchMoveMaker(10, 2000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new QuickRandomSearchMoveMaker(20, 5000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new QuickRandomSearchMoveMaker(30, 10000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),

				() => new PlayerDecisionMaker(new MoveOnlyMinimaxMoveMaker(6), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				() => new PlayerDecisionMaker(new MoveOnlyMinimaxMoveMaker(8), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),

				() =>
				{
					var mcts = new MonteCarloTreeSearchMoveMaker(10000, TuneableUtilityMoveMaker.Tuning1, new TightBoardEvaluator(true), 2);
					return new PlayerDecisionMaker(mcts, new PlacementMaker(mcts.PlacementStrategy));
				},

				() =>
				{
					var mcts = new MonteCarloTreeSearchMoveMaker(20000, TuneableUtilityMoveMaker.Tuning1, new TightBoardEvaluator(true), 2);
					return new PlayerDecisionMaker(mcts, new PlacementMaker(mcts.PlacementStrategy));
				},
				//
				//() => new PlayerDecisionMaker(TuneableUtilityMoveMaker.Tuning1, PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
			};

			const int TotalRuns = 100;
			const bool enableConsoleLogging = false;

			//TODO: Play each AI against each other AI 100 times and print a table of results

			var totalWins = new int[aiToTest.Length, aiToTest.Length];
			var totalTimeTaken = new long[aiToTest.Length];
			Console.WriteLine($"Running {aiToTest.Length * (aiToTest.Length - 1) / 2} * {TotalRuns} Games");
			int gameNumber = 0;

			for (var a = 0; a < aiToTest.Length; a++)
			{
				for (var b = a; b < aiToTest.Length; b++)
				{
					if (a == b)
						continue;

					long aiATime = 0;
					long aiBTime = 0;
					var aiA = aiToTest[a];
					var aiB = aiToTest[b];
					Console.WriteLine($"{++gameNumber} {aiA().Name} vs {aiB().Name}");

					Parallel.For(0, TotalRuns, new ParallelOptions { MaxDegreeOfParallelism = 6 }, (run) =>
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

							lock (totalWins)
							{
								if (aWin)
									totalWins[a, b]++;
								else
									totalWins[b, a]++;
							}

							Interlocked.Add(ref aiATime, runner.Stopwatches[run % 2].ElapsedMilliseconds);
							Interlocked.Add(ref aiBTime, runner.Stopwatches[(run + 1) % 2].ElapsedMilliseconds);
							//Console.WriteLine(aWin);
						}
					);
					Console.WriteLine(totalWins[a, b]);

					totalTimeTaken[a] += aiATime;
					totalTimeTaken[b] += aiBTime;
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

			res.Add("");
			for (var a = 0; a < aiToTest.Length; a++)
				res.Add(aiToTest[a]().Name + "," + totalTimeTaken[a]);

			Console.WriteLine("Saving results as " + filename);
			File.WriteAllLines(filename, res);

			Process.Start(filename);
		}

		static void TestPlacementOnly()
		{
			var stopwatch = Stopwatch.StartNew();

			var strategies = new IPlacementStrategy[]
			{
				//FirstPossiblePlacementStrategy.Instance,
				//SimpleClosestToWallAndCornerStrategy.Instance,
				//ClosestToCornerLeastHolesTieBreakerPlacementStrategy.Instance,
				//NextToPieceEdgeLeastHolesTieBreakerPlacementStrategy.Instance,
				//TightPlacementStrategy.InstanceDoubler,
				//TightPlacementStrategy.InstanceIncrement,
				ExhaustiveMostFuturePlacementsPlacementStrategy.Instance1_1,
				ExhaustiveMostFuturePlacementsPlacementStrategy.Instance1_6,
				new BestEvaluatorStrategy(new Pattern2x2BoardEvaluator()),
				new BestEvaluatorStrategy(TuneablePattern2x2BoardEvaluator.Tuning1),
			};

			//foreach (var strategy in strategies)
			Parallel.ForEach(strategies, (strategy) =>
				{
					//var rand = new Random(0);
					int totalPlaced = 0;

					for (var i = 0; i < 100; i++)
					{
						var pieces = new PieceCollection();
						pieces.Populate(SimulationHelpers.GetRandomPieces(i));
						int index = 0;
						int placed = 0;

						var board = new BoardState();

						while (true)
						{
							var piece = pieces[index];
							pieces.RemoveAt(index);
							index = index % pieces.Count;

							if (strategy.TryPlacePiece(board, PieceDefinition.AllPieceDefinitions[piece], in pieces, index, out var bitmap, out var x, out var y))
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
			);
			Console.WriteLine($"Time taken {stopwatch.ElapsedMilliseconds}");

			Console.ReadLine();
		}

		static void TestPreplacers()
		{
			var preplacers = new IPreplacer[]
			{
				new ExhaustiveMostFuturePlacementsPreplacer(1), 
				new ExhaustiveMostFuturePlacementsPreplacer(2),
				//new ExhaustiveMostFuturePlacementsPreplacer(3),

				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 2, 2, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 3, 2, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 4, 2, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 5, 2, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 6, 2, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 2, 3, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 3, 3, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 4, 3, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 5, 3, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 2, 4, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 3, 4, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 4, 4, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 2, 5, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 3, 5, false),
				//new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 4, 5, false),
				//new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 5, 5, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 2, 6, false),
				new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 3, 6, false),
				//new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 4, 6, false),
				//new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 5, 6, false),
				//new EvaluatorTreeSearchPreplacer(new TightBoardEvaluator(true), 6, 6, false),

				
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 2, 2, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 3, 2, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 4, 2, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 5, 2, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 6, 2, false),

				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 2, 3, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 3, 3, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 4, 3, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 5, 3, false),

				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 2, 4, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 3, 4, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 4, 4, false),

				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 2, 5, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 3, 5, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 4, 5, false),
				//new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 5, 5, false),

				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 2, 6, false),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 3, 6, false),
				//new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 4, 6, false),
				//new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 5, 6, false),
				//new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 6, 6, false),


				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 2, 2, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 3, 2, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 4, 2, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 5, 2, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 6, 2, true),

				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 2, 3, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 3, 3, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 4, 3, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 5, 3, true),

				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 2, 4, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 3, 4, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 4, 4, true),

				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 2, 5, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 3, 5, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 4, 5, true),
				//new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 5, 5, true),

				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 2, 6, true),
				new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 3, 6, true),
				//new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 4, 6, true),
				//new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 5, 6, true),
				//new EvaluatorTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 6, 6, true),

				//new WeightedTreeSearchPreplacer(new TightBoardEvaluator(true), 1000, 3),
				//new WeightedTreeSearchPreplacer(new TightBoardEvaluator(true), 1000, 3),
				//new WeightedTreeSearchPreplacer(new Pattern2x2BoardEvaluator(), 1000, 3),
				//new WeightedTreeSearchPreplacer(TuneablePattern2x2BoardEvaluator.Tuning1, 1000, 3),
				//new WeightedTreeSearchPreplacer(TuneablePattern3x3BoardEvaluator.Tuning1, 1000, 3),
			};

			//foreach (var strategy in strategies)
			Parallel.ForEach(preplacers, new ParallelOptions { MaxDegreeOfParallelism = 4}, (preplacer) =>
				{
					var sw = Stopwatch.StartNew();

					int totalPlaced = 0;

					for (var i = 0; i < 1000; i++)
					{
						var pieces = SimulationHelpers.GetRandomPieces(i).Select(p => PieceDefinition.AllPieceDefinitions[p]).ToList();
						int placed = 0;

						var board = new BoardState();

						while (Helpers.CanPlace(board, pieces[0]))
						{
							var preplacement = preplacer.Preplace(board, pieces);
							board.Place(preplacement.Bitmap, preplacement.X, preplacement.Y);
							placed++;
							pieces.RemoveAt(0);
						}

						totalPlaced += placed;
					}

					Console.WriteLine($"{totalPlaced},{sw.ElapsedMilliseconds},{preplacer.Name}");
				}
			);

			Console.WriteLine("Done");
			Console.ReadLine();
		}

	}
}