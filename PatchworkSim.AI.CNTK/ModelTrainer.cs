using System;
using System.Collections.Generic;
using System.Linq;
using CNTK;

namespace PatchworkSim.AI.CNTK
{
	public class ModelTrainer
	{
		public readonly Function ModelFunc;
		private readonly int _batchSize;
		private readonly DeviceDescriptor _device;

		public int Generation = 0;

		private int _dataIndex;
		private readonly float[] _trainingData;
		private readonly float[] _labelData;

		public ModelTrainer(string modelPath, int batchSize, DeviceDescriptor device)
		{
			_batchSize = batchSize;
			_device = device;
			ModelFunc = Function.Load(modelPath, device);

			_trainingData = new float[9 * 9 * _batchSize * 50];
			_labelData = new float[82 * _batchSize * 50];
		}

		public void RecordPlacementsForTraining(List<BoardState> boards, int finalAreaCovered)
		{
			//Copy boards and finalAreaCovered in to arrays for training

			var trainingBase = _dataIndex * 9 * 9;
			for (var i = 0; i < boards.Count; i++)
			{
				var board = boards[i];
				for (var x = 0; x < BoardState.Width; x++)
				{
					for (var y = 0; y < BoardState.Height; y++)
					{
						_trainingData[trainingBase] = board[x, y] ? 1 : 0;
						trainingBase++;
					}
				}
			}

			var labelBase = _dataIndex * 82;
			for (var i = 0; i < boards.Count; i++)
			{
				_labelData[labelBase + finalAreaCovered] = 1;
				labelBase += 82;
			}

			_dataIndex += boards.Count;
		}

		public void Train()
		{
			//TODO ^^

			var input = ModelFunc.Arguments.Single();

			var labelsVar = CNTKLib.InputVariable(new int[] { ModelFunc.Output.Shape.TotalSize }, DataType.Float, new AxisVector() { ModelFunc.Output.DynamicAxes[0] });

			var trainingLoss = CNTKLib.CrossEntropyWithSoftmax(ModelFunc, labelsVar, "lossFunction");
			var prediction = CNTKLib.ClassificationError(ModelFunc, labelsVar, "predictionError");

			var learningRatePerSample = new TrainingParameterScheduleDouble(1);

			var trainer = Trainer.CreateTrainer(ModelFunc, trainingLoss, prediction, new List<Learner>
			{
				Learner.SGDLearner(ModelFunc.Parameters(), learningRatePerSample)
			});

			var inputBatch = Value.CreateBatch(input.Shape, _trainingData, 0, _dataIndex * BoardState.Width * BoardState.Height, _device);
			var labelBatch = Value.CreateBatch(ModelFunc.Output.Shape, _labelData, 0, _dataIndex * 82, _device);

			var arguments = new Dictionary<Variable, Value>
			{
				{ input, inputBatch },
				{ labelsVar, labelBatch }
			};

			trainer.TrainMinibatch(arguments, false, _device);

			//https://github.com/Microsoft/CNTK/issues/2954
			inputBatch.Erase();
			labelBatch.Erase();

			Generation++;
			float trainLossValue = (float)trainer.PreviousMinibatchLossAverage();
			float evaluationValue = (float)trainer.PreviousMinibatchEvaluationAverage();
			Console.WriteLine($"Minibatch: {Generation} CrossEntropyLoss = {trainLossValue}, EvaluationCriterion = {evaluationValue}");

			_dataIndex = 0;
			Array.Clear(_trainingData, 0, _trainingData.Length);
			Array.Clear(_labelData, 0, _labelData.Length);
		}

		public void Save(string path)
		{
			ModelFunc.Save(path);
		}
	}
}