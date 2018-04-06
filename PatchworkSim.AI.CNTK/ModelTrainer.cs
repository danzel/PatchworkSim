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
		private readonly int _totalMemorySize;
		private readonly DeviceDescriptor _device;

		private readonly List<TrainingSample> _samples;
		private int _samplesIndex;

		private readonly float[] _trainingData;
		private readonly float[] _labelData;

		private readonly Variable _labelsVar;
		private readonly Trainer _trainer;

		public ModelTrainer(string modelPath, int batchSize, int totalMemorySize, DeviceDescriptor device)
		{
			_batchSize = batchSize;
			_totalMemorySize = totalMemorySize;
			_device = device;
			ModelFunc = Function.Load(modelPath, device);

			_samples = new List<TrainingSample>(totalMemorySize);

			_trainingData = new float[BoardState.Width * BoardState.Height * _batchSize];
			_labelData = new float[_batchSize];

			_labelsVar = CNTKLib.InputVariable(new int[] { ModelFunc.Output.Shape.TotalSize }, DataType.Float, new AxisVector() { ModelFunc.Output.DynamicAxes[0] });

			var mseLoss = CNTKLib.ReduceMean(CNTKLib.Square(CNTKLib.Minus(ModelFunc, _labelsVar).Output).Output, new Axis(0));
			var mseLoss2 = CNTKLib.ReduceMean(CNTKLib.Square(CNTKLib.Minus(ModelFunc, _labelsVar).Output).Output, new Axis(0));

			_trainer = Trainer.CreateTrainer(ModelFunc, mseLoss, mseLoss2, new List<Learner>
			{
				Learner.SGDLearner(ModelFunc.Parameters(), new TrainingParameterScheduleDouble(0.01, (uint)_batchSize))
				//CNTKLib.AdaDeltaLearner(new ParameterVector(new List<Parameter>(modelFunc.Parameters())), new TrainingParameterScheduleDouble(1))
			});
		}

		public void RecordPlacementsForTraining(List<TrainingSample> samples)
		{
			//Add memories until we are full, then overwrite old memories
			for (var i = 0; i < samples.Count; i++)
			{
				if (_samples.Count < _totalMemorySize)
					_samples.Add(samples[i]);
				else
					_samples[_samplesIndex] = samples[i];
				_samplesIndex = (_samplesIndex + 1) % _totalMemorySize;
			}
		}

		public bool ReadyToTrain => _samples.Count >= _batchSize;

		private void PrepareDataForTraining()
		{
			//Console.WriteLine("Creating batch");
			//Copy _batchSize worth
			for (var i = 0; i < _batchSize; i++)
			{
				var index = _rng.Next(0, _samples.Count); //Might get the same one twice, probably not worth worrying about
				var board = _samples[index].Board;
				CNTKHelpers.CopyBoardToArray(board, _trainingData, i * BoardState.Width * BoardState.Height);

				//var goodnessScale = _samples[index].FinalAreaCovered / (float)(BoardState.Width * BoardState.Height);
				//goodnessScale = goodnessScale * goodnessScale * goodnessScale;
				//TODO: Is this a good scaling curve?
				//x^3: 0.5 -> 0.12, 0.8 -> 0.5

				_labelData[i] = _samples[index].Score;
			}
		}

		public TrainingResult Train()
		{
			//Console.WriteLine("Will train with sample count " + _samples.Count);

			float totalTrainLoss = 0;
			float totalEvaluation = 0;

			PrepareDataForTraining();

			var input = ModelFunc.Arguments.Single();

			var inputBatch = Value.CreateBatch(input.Shape, _trainingData, 0, _batchSize * BoardState.Width * BoardState.Height, _device);
			var labelBatch = Value.CreateBatch(ModelFunc.Output.Shape, _labelData, 0, _batchSize, _device);

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

			return new TrainingResult(totalTrainLoss, totalEvaluation);
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