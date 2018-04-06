using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CNTK;
using PatchworkSim.Loggers;

namespace PatchworkSim.AI.CNTK
{
	public class TrainingRunner
	{
		public void Run()
		{
			var device = DeviceDescriptor.GPUDevice(0);

			//https://www.cs.toronto.edu/~vmnih/docs/dqn.pdf
			//Roughly like algorithm on page 5, except:
			// Our reward is the the percentage full the resulting board is
			// We perform a full playout (until board full) before passing all of the states to the learner
			//  This is required so we know how full the board got

			const int batchSize = 32;
			const int totalMemorySize = 1_000_000;

			//We scale epsilon down from initial to final value linearly over {epsilonGamesToDecreaseToFinal} games, then it is fixed at the final value
			const double epsilonInitial = 1f;
			const double epsilonFinal = 0.1f;
			const int epsilonGamesToDecreaseToFinal = 100_000;


			var trainer = new ModelTrainer("../../../../PatchworkSim.AI.CNTK/keras/model.dnn", batchSize, totalMemorySize, device);
			var boardEvaluator = new BulkBoardEvaluator(trainer.ModelFunc, device);
			var trainingDataGenerator = new QLearningGenerator(boardEvaluator);

			var greedyPlacer = new CNTKNoLookaheadPlacementStrategy(boardEvaluator);

			int generation = 0;
			int rand = 0;

			Console.WriteLine($"Gen\tTotlArea\tArea%\tTestArea\tLossAvg\tEvalAvg");

			int bestTestCoverage = TestIt(greedyPlacer, true);

			while (true)
			{
				int totalAreaCovered = 0;
				int gamesPlayed = 0;

				if (generation >= epsilonGamesToDecreaseToFinal)
					trainingDataGenerator.Epsilon = epsilonFinal;
				else
				{
					var p = generation / (double)epsilonGamesToDecreaseToFinal;
					trainingDataGenerator.Epsilon = epsilonInitial * (1 - p) + epsilonFinal * p;
				}

				do
				{
					//Get the pieces we are going to place
					var pieces = SimulationHelpers.GetRandomPieces(rand).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList();

					var trainingSamples = trainingDataGenerator.GenerateTrainingData(pieces, out var areaCovered);

					//Pass those to the trainer
					//Console.WriteLine($"Managed to cover {areaCovered} resulting in {trainingSamples.Count} samples");

					totalAreaCovered += areaCovered;
					trainer.RecordPlacementsForTraining(trainingSamples);
					gamesPlayed++;
				} while (!trainer.ReadyToTrain); //This loop only runs the first time through when we don't have the minimum amount of samples required to do any training


				var areaPercent = totalAreaCovered * 100.0 / (gamesPlayed * BoardState.Width * BoardState.Height);

				var result = trainer.Train();

				int testCaseAreaCovered = TestIt(greedyPlacer, false);

				if (testCaseAreaCovered > bestTestCoverage)
				{
					bestTestCoverage = testCaseAreaCovered;
					TestIt(greedyPlacer, true);
				}

				Console.WriteLine($"{generation}\t{totalAreaCovered.ToString().PadRight(8)}\t{areaPercent:0.0}\t{testCaseAreaCovered.ToString().PadRight(8)}\t{result.LossAverage:0.0000}\t{result.EvaluationAverage:0.0000}");
				generation++;

				if (generation % 10000 == 0)
					trainer.Save($"./model-{generation.ToString().PadLeft(4, '0')}-{DateTimeOffset.UtcNow.Ticks}.dnn");
			}
		}

		private int TestIt(CNTKNoLookaheadPlacementStrategy placer, bool printOutput)
		{
			var pieces = new[]
			{
				SimulationHelpers.GetRandomPieces(0).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList(),
				///SimulationHelpers.GetRandomPieces(1).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList(),
				///SimulationHelpers.GetRandomPieces(2).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList(),
				///SimulationHelpers.GetRandomPieces(3).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList(),
			};

			int areaCovered = 0;
			var oldBoards = new BoardState[pieces.Length];
			var boards = new BoardState[pieces.Length];
			var stillPlacing = pieces.Select(p => true).ToArray();

			var noPieces = new PieceCollection();

			while (stillPlacing.Any(x => x))
			{
				for (var i = 0; i < pieces.Length; i++)
				{
					oldBoards[i] = boards[i];

					if (stillPlacing[i] && Helpers.CanPlace(boards[i], pieces[i][0]))
					{
						var board = boards[i];
						placer.TryPlacePiece(board, pieces[i][0], in noPieces, 0, out var bitmap, out var x, out var y);
						board.Place(bitmap, x, y);

						areaCovered += pieces[i][0].TotalUsedLocations;
						pieces[i].RemoveAt(0);

						boards[i] = board;
					}
					else
					{
						stillPlacing[i] = false;
					}
				}

				if (printOutput)
				{
					Console.WriteLine("----------------------------------------");
					ConsoleLogger.PrintBoardsDiff(oldBoards, boards);
				}
			}
			
			if (printOutput)
			{
				Console.WriteLine("Placed " + String.Join(", ", pieces.Select(p => PieceDefinition.AllPieceDefinitions.Length - p.Count)));
			}

			return areaCovered;
		}
	}

	class QLearningGenerator
	{
		private readonly BulkBoardEvaluator _boardEvaluator;

		/// <summary>
		/// Percent chance we'll do a random move instead of the 'best' move (controlled by TrainingRunner)
		/// </summary>
		public double Epsilon;

		private readonly Random _random = new Random(0);

		public QLearningGenerator(BulkBoardEvaluator boardEvaluator)
		{
			_boardEvaluator = boardEvaluator;
		}

		public List<TrainingSample> GenerateTrainingData(List<PieceDefinition> pieces, out int areaCovered)
		{
			var resultingBoards = new List<BoardState>();

			var board = new BoardState();
			int pieceIndex = 0;
			areaCovered = 0;

			while (Helpers.CanPlace(board, pieces[pieceIndex]))
			{
				var allPlacements = GetAllPossiblePlacements(in board, pieces[pieceIndex]);

				int chosenPlacementIndex;
				if (allPlacements.Count == 1)
				{
					chosenPlacementIndex = 0;
				}
				else if (_random.NextDouble() > Epsilon)
				{
					//Choose the best one according to the NN
					_boardEvaluator.Evaluate(allPlacements);

					chosenPlacementIndex = 0;

					var bestScore = allPlacements[0].Score;
					for (var i = 1; i < allPlacements.Count; i++)
					{
						if (allPlacements[i].Score > bestScore)
						{
							chosenPlacementIndex = i;
							bestScore = allPlacements[i].Score;
						}
					}
				}
				else
				{
					//Choose a random one
					chosenPlacementIndex = _random.Next(0, allPlacements.Count);
				}

				board = allPlacements[chosenPlacementIndex].Board;

				resultingBoards.Add(board);

				areaCovered += pieces[pieceIndex].TotalUsedLocations;
				pieceIndex++;
			}

			var score = areaCovered / (float)(BoardState.Width * BoardState.Height);

			var result = new List<TrainingSample>(resultingBoards.Count);
			for (var i = 0; i < resultingBoards.Count; i++)
			{
				result.Add(new TrainingSample(resultingBoards[i], score));
			}

			return result;
		}

		private List<BoardWithParent> GetAllPossiblePlacements(in BoardState board, PieceDefinition piece)
		{
			var result = new List<BoardWithParent>();

			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						if (board.CanPlace(bitmap, x, y))
						{
							var clone = board;
							clone.Place(bitmap, x, y);
							result.Add(new BoardWithParent(clone));
						}
					}
				}
			}

			return result;
		}

	}
}