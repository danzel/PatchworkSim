using System;
using System.Linq;
using System.Threading;
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
			//ComparePlacementStrategies();
			//WatchAGame();

			GenerateTuneableUtilityMoveMaker();
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

					if (!strategies[i].ImplementsLookahead && strategies[i].TryPlacePiece(boards[i], piece, pieces, i + 1, out var bitmap, out var x, out var y))
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
				new PlayerDecisionMaker(new GreedyCardValueUtilityMoveMaker(2), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6),
				new PlayerDecisionMaker(new QuickRandomSearchMoveMaker(10, 10000), PlacementMaker.ExhaustiveMostFuturePlacementsInstance1_6));
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

		private static void GenerateTuneableUtilityMoveMaker()
		{
			var boss = new GreedyCardValueUtilityMoveMaker(2);
			var rand = new Random();

			//TuneableUtilityMoveMaker best = null; //CreateRandomTuneable(rand);
			var mostWinsVsBoss = 0;

			int generation = 0;
			while (true)
			{
				generation++;

				var challenger = CreateRandomTuneable(rand);

				int wins = CalculateChallengerWinsFrom100(boss, challenger);
				if (wins > mostWinsVsBoss)
				{
					Console.WriteLine($"Challenger Gen {generation} won {wins} games");
					Console.WriteLine(challenger.Name);
					mostWinsVsBoss = wins;
					//best = challenger;

					//Compare against something else
					//int winsVsBoss = CalculateChallengerWinsFrom100(boss, challenger);
					//Console.WriteLine($"Challenger Gen {generation} won {winsVsBoss} games vs Boss");
				}
			}
		}

		private static TuneableUtilityMoveMaker CreateRandomTuneable(Random rand)
		{
			return new TuneableUtilityMoveMaker(
				rand.NextDouble() * 2 - 1,
				rand.NextDouble() * 2 - 1,
				rand.NextDouble() * 2 - 1,
				rand.NextDouble() * 2 - 1,
				rand.NextDouble() * 2 - 1,
				rand.NextDouble() * 2 - 1,
				rand.NextDouble() * 2 - 1,
				rand.NextDouble() * 2 - 1
			);
		}

		/// <summary>
		/// Returns the amount of wins the challenger got (from 100)
		/// </summary>
		private static int CalculateChallengerWinsFrom100(IMoveDecisionMaker best, IMoveDecisionMaker challenger)
		{
			int challengerWins = 0;

			Parallel.For(0, 100, (i) =>
			{
				var state = new SimulationState(SimulationHelpers.GetRandomPieces(i / 2), i % 2);
				state.Fidelity = SimulationFidelity.NoPiecePlacing;
				//TODO: May need a cheaper placement engine
				var runner = new SimulationRunner(state,
					new PlayerDecisionMaker(best, null),
					new PlayerDecisionMaker(challenger, null));

				while (!state.GameHasEnded)
					runner.PerformNextStep();

				if (state.WinningPlayer == 1)
					Interlocked.Increment(ref challengerWins);
			});

			return challengerWins;
		}
	}
}