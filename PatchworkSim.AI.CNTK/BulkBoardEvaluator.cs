﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace PatchworkSim.AI.CNTK
{
	internal class BulkBoardEvaluator
	{
		private readonly DeviceDescriptor _device;
		private readonly Function _modelFunc;
		private readonly Variable _inputVar;
		private readonly Variable _outputVar;
		private readonly NDShape _inputShape;

		public BulkBoardEvaluator(Function modelFunc, DeviceDescriptor device)
		{
			_device = device;
			//https://github.com/Microsoft/CNTK/blob/release/latest/Examples/Evaluation/CNTKAzureTutorial01/CNTKAzureTutorial01/Controllers/ValuesController.cs

			_modelFunc = modelFunc;

			_inputVar = _modelFunc.Arguments.Single();
			_outputVar = _modelFunc.Output;

			_inputShape = _inputVar.Shape;

			//Width and channels are swapped, maybe should swap things in the model?
			//int imageWidth = inputShape[0];
			//int imageHeight = inputShape[1];
			//int imageChannels = inputShape[2];
			//int imageSize = inputShape.TotalSize;
		}

		public void Evaluate(List<BoardWithParent> boards)
		{
			//https://github.com/Microsoft/CNTK/blob/06ee7b83a5a95457f2e6e1fd8a452f28dc47ebd0/Examples/Evaluation/CNTKLibraryCSEvalCPUOnlyExamples/CNTKLibraryCSEvalExamples.cs#L105
			// EvaluationBatchOfImages
			const int maxBatchSize = 20000; //TODO: Depends on GPU memory size

			for (var start = 0; start < boards.Count; start += maxBatchSize)
			{
				var count = Math.Min(maxBatchSize, boards.Count - start);

				float[] vals = new float[BoardState.Width * BoardState.Height * count];

				for (var i = 0; i < count; i++)
				{
					CNTKHelpers.CopyBoardToArray(boards[start + i].Board, vals, i * BoardState.Width * BoardState.Height);
				}

				var inputDataMap = new Dictionary<Variable, Value>();
				var inputVal = Value.CreateBatch(_inputShape, vals, _device);
				inputDataMap.Add(_inputVar, inputVal);

				var outputDataMap = new Dictionary<Variable, Value>();
				outputDataMap.Add(_outputVar, null);

				_modelFunc.Evaluate(inputDataMap, outputDataMap, _device);

				var outputVal = outputDataMap[_outputVar];
				var outputData = outputVal.GetDenseData<float>(_outputVar);

				for (var b = 0; b < count; b++)
				{
					var ourData = outputData[b];

					boards[start + b].SetScore(ourData[1]);
				}

				//https://github.com/Microsoft/CNTK/issues/2954
				inputVal.Erase();
				outputVal.Erase();
			}
		}
	}
}
