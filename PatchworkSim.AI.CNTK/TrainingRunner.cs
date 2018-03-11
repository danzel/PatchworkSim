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
			var preplacer = new CNTKEvaluatorTreeSearchPreplacer(new BulkBoardEvaluator(trainer.ModelFunc, device), 4, 4);

			int generation = 0;
			int rand = 0;
			var boards = new List<BoardState>();

			while (true)
			{
				TestIt(generation, preplacer);

				int totalAreaCovered = 0;
				for (var i = 0; i < batchSize; i++)
				{
					//Random pieces and do preplacements
					boards.Clear();
					var pieces = SimulationHelpers.GetRandomPieces(rand++).Select(x => PieceDefinition.AllPieceDefinitions[x]).ToList();
					int areaCovered = 0;

					var board = new BoardState();
					while (Helpers.CanPlace(board, pieces[0]))
					{
						var placement = preplacer.Preplace(board, pieces);
						board.Place(placement.Bitmap, placement.X, placement.Y);

						areaCovered += pieces[0].TotalUsedLocations;
						pieces.RemoveAt(0);

						boards.Add(board);
					}

					//Pass those to the trainer
					totalAreaCovered += areaCovered;
					trainer.RecordPlacementsForTraining(boards, areaCovered);
				}

				Console.WriteLine($"Score: {totalAreaCovered} @ {totalAreaCovered / (double)(batchSize * BoardState.Width * BoardState.Height)}%");

				trainer.Train();
				generation++;
				trainer.Save($"./model-{generation.ToString().PadLeft(4, '0')}-{DateTimeOffset.UtcNow.Ticks}.dnn");
			}
		}

		private void TestIt(int generation, CNTKEvaluatorTreeSearchPreplacer preplacer)
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

			while (stillPlacing.Any(x => x))
			{
				for (var i = 0; i < 4; i++)
				{
					oldBoards[i] = boards[i];

					if (stillPlacing[i] && Helpers.CanPlace(boards[i], pieces[i][0]))
					{
						var board = boards[i];
						var placement = preplacer.Preplace(board, pieces[i]);
						board.Place(placement.Bitmap, placement.X, placement.Y);

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