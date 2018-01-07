using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PatchworkSim;
using PatchworkSim.AI.MoveMakers;
using PatchworkSim.AI.PlacementFinders;

namespace PatchworkAIComparer
{
	class Program
	{
		static void Main(string[] args)
		{
			var aiToTest = new[]
			{
				new PlayerDecisionMaker(new RandomMoveMaker(), FirstPossiblePlacementMaker.Instance), //TODO: Really we should make a new RandomMoveMaker every test(?)
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-5), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-4), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-3), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-2), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(-1), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(0), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(1), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(3), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(4), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(5), FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(BuyFirstPossibleMoveMaker.Instance, FirstPossiblePlacementMaker.Instance),
				new PlayerDecisionMaker(AlwaysAdvanceMoveMaker.Instance, FirstPossiblePlacementMaker.Instance)
			};

			const int TotalRuns = 100;

			//TODO: Play each AI against each other AI 100 times and print a table of results

			var totalWins = new int[aiToTest.Length, aiToTest.Length];

			for (var a = 0; a < aiToTest.Length; a++)
			{
				for (var b = a; b < aiToTest.Length; b++)
				{
					var aiA = aiToTest[a];
					var aiB = aiToTest[b];
					Console.WriteLine($"Running {aiA.Name} vs {aiB.Name}");

					for (var run = 0; run < TotalRuns; run++)
					{
						var state = new SimulationState(SimulationHelpers.GetRandomPieces(run), 0);
						//state.Fidelity = SimulationFidelity.NoPiecePlacing;
						//Let each Ai have half of the goes first and half second
						var runner = new SimulationRunner(state, run % 2 == 0 ? aiA : aiB, run % 2 == 1 ? aiA : aiB);

						while (!state.GameHasEnded)
							runner.PerformNextStep();

						var aWin = run % 2 == state.WinningPlayer;

						if (aWin)
							totalWins[a, b]++;
						else
							totalWins[b, a]++;
					}
				}
			}

			//Calculate the total points for each
			var total = Enumerable.Range(0, aiToTest.Length).Select(ai => Enumerable.Range(0, aiToTest.Length).Sum(opponent => totalWins[ai, opponent])).ToArray();

			//Dump a CSV with the results
			var filename = "result_" + DateTimeOffset.Now.Ticks + ".csv";
			var res = new List<string>();

			res.Add("," + string.Join(", ", aiToTest.Select(ai => ai.Name)) + ",Total,Rank");
			for (var a = 0; a < aiToTest.Length; a++)
			{
				var line = aiToTest[a].Name;
				for (var b = 0; b < aiToTest.Length; b++)
				{
					line += "," + totalWins[a, b];
				}

				//Total and rank
				line += "," + total[a];
				line += "," + (aiToTest.Length - total.Count(c => c < total[a]));

				res.Add(line);
			}

			Console.WriteLine("Saving results as " + filename);
			File.WriteAllLines(filename, res);

			Process.Start(filename);
		}
	}
}