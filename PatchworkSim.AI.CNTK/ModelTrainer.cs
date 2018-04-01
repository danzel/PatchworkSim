using System;
using System.Collections.Generic;
using System.Linq;
using CNTK;

namespace PatchworkSim.AI.CNTK
{
	public struct TrainingSample
	{
		public readonly BoardState Board;
		public readonly float Score;

		public TrainingSample(BoardState board, float score)
		{
			Board = board;
			Score = score;
		}
	}

	public class ModelTrainer
	{
		public readonly Function ModelFunc;
		private readonly int _batchSize;
		private readonly int _batchesPerCycle;
		private readonly DeviceDescriptor _device;

		public int Generation = 0;

		private readonly List<TrainingSample> _samples;

		private readonly float[] _trainingData;
		private readonly float[] _labelData;

		private readonly Variable _labelsVar;
		private readonly Trainer _trainer;

		public ModelTrainer(string modelPath, int batchSize, int batchesPerCycle, DeviceDescriptor device)
		{
			_batchSize = batchSize;
			_batchesPerCycle = batchesPerCycle;
			_device = device;
			ModelFunc = Function.Load(modelPath, device);

			_samples = new List<TrainingSample>(batchSize * (batchesPerCycle + 2));

			_trainingData = new float[BoardState.Width * BoardState.Height * _batchSize];
			_labelData = new float[(BoardState.Width * BoardState.Height + 1) * _batchSize];

			_labelsVar = CNTKLib.InputVariable(new int[] { ModelFunc.Output.Shape.TotalSize }, DataType.Float, new AxisVector() { ModelFunc.Output.DynamicAxes[0] });

			var trainingLoss = CNTKLib.CrossEntropyWithSoftmax(ModelFunc, _labelsVar, "lossFunction");
			var prediction = CNTKLib.ClassificationError(ModelFunc, _labelsVar, "predictionError");

			var learningRatePerSample = new TrainingParameterScheduleDouble(0.0001, (uint)batchSize);

			_trainer = Trainer.CreateTrainer(ModelFunc, trainingLoss, prediction, new List<Learner>
			{
				Learner.SGDLearner(ModelFunc.Parameters(), learningRatePerSample)
			});
		}

		public void RecordPlacementsForTraining(List<TrainingSample> samples)
		{
			for (var i = 0; i < samples.Count; i++)
			{
				_samples.Add(samples[i]);
			}
		}

		public bool ReadyToTrain => _samples.Count >= _batchSize * _batchesPerCycle;

		private void PrepareDataForTraining(int batch)
		{
			//Copy _batchSize worth
			for (var i = 0; i < _batchSize; i++)
			{
				var index = i + (batch * _batchSize);
				var board = _samples[index].Board;
				CNTKHelpers.CopyBoardToArray(board, _trainingData, i * BoardState.Width * BoardState.Height);

				//var goodnessScale = _samples[index].FinalAreaCovered / (float)(BoardState.Width * BoardState.Height);
				//goodnessScale = goodnessScale * goodnessScale * goodnessScale;
				//TODO: Is this a good scaling curve?
				//x^3: 0.5 -> 0.12, 0.8 -> 0.5

				_labelData[i * 2 + 0] = 1 - _samples[index].Score;
				_labelData[i * 2 + 1] = _samples[index].Score;
			}

		}

		public TrainingResult Train()
		{
			Generation++;

			float totalTrainLoss = 0;
			float totalEvaluation = 0;

			ShuffleSamples();
			for (var i = 0; i < _batchesPerCycle; i++)
			{
				PrepareDataForTraining(i);

				var input = ModelFunc.Arguments.Single();


				var inputBatch = Value.CreateBatch(input.Shape, _trainingData, 0, _batchSize * BoardState.Width * BoardState.Height, _device);
				var labelBatch = Value.CreateBatch(ModelFunc.Output.Shape, _labelData, 0, _batchSize * 2, _device);

				var arguments = new Dictionary<Variable, Value>
				{
					{ input, inputBatch },
					{ _labelsVar, labelBatch }
				};

				_trainer.TrainMinibatch(arguments, false, _device);

				//https://github.com/Microsoft/CNTK/issues/2954
				inputBatch.Erase();
				labelBatch.Erase();

				float trainLossValue = (float)_trainer.PreviousMinibatchLossAverage();
				float evaluationValue = (float)_trainer.PreviousMinibatchEvaluationAverage();
				//Console.WriteLine($"Minibatch: {Generation}[{i}] CrossEntropyLoss = {trainLossValue}, EvaluationCriterion = {evaluationValue}");
				totalTrainLoss += trainLossValue;
				totalEvaluation += evaluationValue;

				Array.Clear(_trainingData, 0, _trainingData.Length);
				Array.Clear(_labelData, 0, _labelData.Length);
			}

			_samples.Clear();
			return new TrainingResult(totalTrainLoss / _batchesPerCycle, totalEvaluation / _batchesPerCycle);
		}

		public void Save(string path)
		{
			ModelFunc.Save(path);
		}

		private readonly Random _rng = new Random(0);

		public void ShuffleSamples()
		{
			int n = _samples.Count;
			while (n > 1)
			{
				n--;
				int k = _rng.Next(n + 1);
				var value = _samples[k];
				_samples[k] = _samples[n];
				_samples[n] = value;
			}
		}

		public struct TrainingResult
		{
			public readonly float LossAverage;
			public readonly float EvaluationAverage;

			public TrainingResult(float lossAverage, float evaluationAverage)
			{
				LossAverage = lossAverage;
				EvaluationAverage = evaluationAverage;
			}
		}
	}
}