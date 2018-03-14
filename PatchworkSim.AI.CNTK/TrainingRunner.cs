using System;
using System.Collections.Generic;
using System.Linq;
using CNTK;
using PatchworkSim.Loggers;

namespace PatchworkSim.AI.CNTK
{
	public class TrainingRunner
	{
		public void Run()
		{
			var device = DeviceDescriptor.GPUDevice(0);

			int batchSize = 1024;
			var trainer = new ModelTrainer("../../../../PatchworkSim.AI.CNTK/keras/model.dnn", batchSize, device);
			var boardEvaluator = new BulkBoardEvaluator(trainer.ModelFunc, device);
			var evaluatorTreeSearch = new CNTKEvaluatorTreeSearch(boardEvaluator, 16);
			var greedyPlacer = new CNTKNoLookaheadPlacementStrategy(boardEvaluator);

			int generation = 0;
			int rand = 0;

			while (true)
			{
				//TestIt(generation, greedyPlacer);

				int totalAreaCovered = 0;
				int gamesPlayed = 0;
				while (!trainer.ReadyToTrain)
				{
					//Random pieces and do preplacements
					var pieces = SimulationHelpers.GetRandomPieces(rand++).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList();

					var boards = evaluatorTreeSearch.PreplaceAll(pieces, out var areaCovered);

					//Pass those to the trainer
					Console.WriteLine($"Managed to cover {areaCovered} resulting in {boards.Count} boards");
					totalAreaCovered += areaCovered;
					trainer.RecordPlacementsForTraining(boards, areaCovered);
					gamesPlayed++;
				}

				Console.WriteLine($"Score: {totalAreaCovered} @ {totalAreaCovered * 100.0 / (gamesPlayed * BoardState.Width * BoardState.Height)}%");

				trainer.Train();
				generation++;
				trainer.Save($"./model-{generation.ToString().PadLeft(4, '0')}-{DateTimeOffset.UtcNow.Ticks}.dnn");
			}
		}

		private void TestIt(int generation, CNTKNoLookaheadPlacementStrategy placer)
		{
			var pieces = new[]
			{
				SimulationHelpers.GetRandomPieces(0).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList(),
				SimulationHelpers.GetRandomPieces(1).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList(),
				SimulationHelpers.GetRandomPieces(2).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList(),
				SimulationHelpers.GetRandomPieces(3).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList(),
			};

			int areaCovered = 0;
			var oldBoards = new BoardState[4];
			var boards = new BoardState[4];
			var stillPlacing = new[] { true, true, true, true };

			var noPieces = new PieceCollection();

			while (stillPlacing.Any(x => x))
			{
				for (var i = 0; i < 4; i++)
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

				Console.WriteLine("----------------------------------------");
				ConsoleLogger.PrintBoardsDiff(oldBoards, boards);
			}

			Console.WriteLine($"{generation} Covered {areaCovered}");
		}
	}
}