using System;
using System.Collections.Generic;
using System.Linq;
using CNTK;

namespace PatchworkSim.AI.CNTK
{
	internal struct TrainingSample
	{
		public readonly BoardState Board;
		public readonly int FinalAreaCovered;

		public TrainingSample(BoardState board, int finalAreaCovered)
		{
			Board = board;
			FinalAreaCovered = finalAreaCovered;
		}
	}

	public class ModelTrainer
	{
		public readonly Function ModelFunc;
		private readonly int _batchSize;
		private readonly DeviceDescriptor _device;

		public int Generation = 0;

		private readonly List<TrainingSample> _samples;

		private readonly float[] _trainingData;
		private readonly float[] _labelData;

		private readonly Variable _labelsVar;
		private readonly Trainer _trainer;

		public ModelTrainer(string modelPath, int batchSize, DeviceDescriptor device)
		{
			_batchSize = batchSize;
			_device = device;
			ModelFunc = Function.Load(modelPath, device);

			_samples = new List<TrainingSample>(batchSize * 2);

			_trainingData = new float[BoardState.Width * BoardState.Height * _batchSize];
			_labelData = new float[82 * _batchSize];

			_labelsVar = CNTKLib.InputVariable(new int[] { ModelFunc.Output.Shape.TotalSize }, DataType.Float, new AxisVector() { ModelFunc.Output.DynamicAxes[0] });

			var trainingLoss = CNTKLib.CrossEntropyWithSoftmax(ModelFunc, _labelsVar, "lossFunction");
			var prediction = CNTKLib.ClassificationError(ModelFunc, _labelsVar, "predictionError");

			var learningRatePerSample = new TrainingParameterScheduleDouble(0.1);

			_trainer = Trainer.CreateTrainer(ModelFunc, trainingLoss, prediction, new List<Learner>
			{
				Learner.SGDLearner(ModelFunc.Parameters(), learningRatePerSample)
			});
		}

		public void RecordPlacementsForTraining(List<BoardState> boards, int finalAreaCovered)
		{
			for (var i = 0; i < boards.Count; i++)
				_samples.Add(new TrainingSample(boards[i], finalAreaCovered));
		}

		public bool ReadyToTrain => _samples.Count >= _batchSize;

		private void PrepareDataForTraining()
		{
			ShuffleSamples();

			//TODO: Keep only _batchSize worth


			for (var i = 0; i < _batchSize; i++)
			{
				var board = _samples[i].Board;
				CNTKHelpers.CopyBoardToArray(board, _trainingData, i * BoardState.Width * BoardState.Height);
			}

			for (var i = 0; i < _batchSize; i++)
			{
				_labelData[(i * 82) + _samples[i].FinalAreaCovered] = 1;
			}

			_samples.Clear();
		}

		public void Train()
		{
			PrepareDataForTraining();

			var input = ModelFunc.Arguments.Single();


			var inputBatch = Value.CreateBatch(input.Shape, _trainingData, 0, _batchSize * BoardState.Width * BoardState.Height, _device);
			var labelBatch = Value.CreateBatch(ModelFunc.Output.Shape, _labelData, 0, _batchSize * 82, _device);

			var arguments = new Dictionary<Variable, Value>
			{
				{ input, inputBatch },
				{ _labelsVar, labelBatch }
			};

			_trainer.TrainMinibatch(arguments, false, _device);

			//https://github.com/Microsoft/CNTK/issues/2954
			inputBatch.Erase();
			labelBatch.Erase();

			Generation++;
			float trainLossValue = (float)_trainer.PreviousMinibatchLossAverage();
			float evaluationValue = (float)_trainer.PreviousMinibatchEvaluationAverage();
			Console.WriteLine($"Minibatch: {Generation} CrossEntropyLoss = {trainLossValue}, EvaluationCriterion = {evaluationValue}");

			Array.Clear(_trainingData, 0, _trainingData.Length);
			Array.Clear(_labelData, 0, _labelData.Length);
		}

		public void Save(string path)
		{
			ModelFunc.Save(path);
		}

		private readonly Random rng = new Random(0);

		public void ShuffleSamples()
		{

			int n = _samples.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				var value = _samples[k];
				_samples[k] = _samples[n];
				_samples[n] = value;
			}
		}
	}
}