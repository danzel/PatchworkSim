using Grpc.Core;
using PatchworkSim.AI.MoveMakers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static PatchworkServer;

public static partial class PatchworkServer
{
	public interface IPatchworkServerClient
	{
		StaticConfigReply GetStaticConfig(StaticConfigRequest request);
		TrainReply Train(TrainRequest request);
		EvaluateReply Evaluate(EvaluateRequest request);
	}

	public partial class PatchworkServerClient : IPatchworkServerClient
	{
		public StaticConfigReply GetStaticConfig(StaticConfigRequest request)
		{
			return GetStaticConfig(request, null);
		}

		public TrainReply Train(TrainRequest request)
		{
			return Train(request, null);
		}

		public EvaluateReply Evaluate(EvaluateRequest request)
		{
			return Evaluate(request, null);
		}
	}

	//class ResultBox<T>
	//{
	//	public T? Item;
	//}
	/*
	class BatchingPatchworkServerClient : IPatchworkServerClient
	{
		private readonly IPatchworkServerClient _client;

		private EvaluateRequest _nextEval;
		private SemaphoreSlim _nextSemaphore;
		private ResultBox<EvaluateReply> _nextResult;
		private bool _requestInProgress;

		private Semaphore _previousSemaphore;
		private bool _someoneWaitingOnPreviousSemaphore;


		public BatchingPatchworkServerClient(IPatchworkServerClient client)
		{
			_client = client;

			_nextEval = new EvaluateRequest();
			_nextSemaphore = new SemaphoreSlim(0);
		}

		public EvaluateReply Evaluate(EvaluateRequest request)
		{
			int index;
			EvaluateRequest eval;
			SemaphoreSlim semi;
			ResultBox<EvaluateReply> result;
			bool shouldRunRequest = false;

			SemaphoreSlim previous;
			bool shouldWaitOnPrevious = false;

			lock (this)
			{
				index = _nextEval.State.Count;
				eval = _nextEval;
				semi = _nextSemaphore;
				result = _nextResult;

				_nextEval.State.AddRange(request.State); //TODO AddRange is bad perf

				if (!_requestInProgress)
				{
					shouldRunRequest = true;

					_requestInProgress = true;

					_nextEval = new EvaluateRequest();
					_nextSemaphore = new SemaphoreSlim(0);
					_nextResult = new ResultBox<EvaluateReply>();
				}
				else if (!_someoneWaitingOnPreviousSemaphore)
				{
					_someoneWaitingOnPreviousSemaphore = true;
					shouldWaitOnPrevious = true;

				}
			}

			if (shouldRunRequest)
			{
				Console.WriteLine("Running " + eval.State.Count);
				result.Item = _client.Evaluate(eval);
				lock (this)
				{
					semi.Release(eval.State.Count); //Enough for everyone

				}
			}
			else
			{
				semi.Wait();
			}
		}

		public StaticConfigReply GetStaticConfig(StaticConfigRequest request)
		{
			return GetStaticConfig(request, null);
		}

		public TrainReply Train(TrainRequest request)
		{
			return Train(request, null);
		}
	}*/
}

namespace PatchworkSim.AI.KerasAlphaZero
{
	public class Program
	{
		public static void Main()
		{
			var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
			var client = new PatchworkServerClient(channel);

			var config = client.GetStaticConfig(new StaticConfigRequest());

			if (config.ObservationSize != GameStateFactory.PlayerObservations + GameStateFactory.LookAheadPieceAmount * GameStateFactory.PieceFields)
				throw new Exception();

			new Program(client).Run();
		}

		private IPatchworkServerClient _client;
		private readonly MoveOnlyMonteCarloTreeSearchAlphaZeroMoveMaker _ai;
		private readonly MoveOnlyMonteCarloTreeSearchMoveMaker _opp;

		public Program(IPatchworkServerClient client)
		{
			_client = client;
			_ai = new MoveOnlyMonteCarloTreeSearchAlphaZeroMoveMaker(_client, 100);
			_opp = new MoveOnlyMonteCarloTreeSearchMoveMaker(20);
		}

		public void Run()
		{
			Evaluate();

			var req = new TrainRequest();


			for (var g = 0; g < 1000; g++)
			{
				var timer = Stopwatch.StartNew();

				//Parallel.For(0, 8, (i) =>
				for (var i = 0; i < 8; i++)
				{
					Console.WriteLine("Starting game " + i);
					var p0Samples = new List<TrainSample>();
					var p1Samples = new List<TrainSample>();

					var state = new SimulationState(SimulationHelpers.GetRandomPieces(), 0);
					state.Fidelity = SimulationFidelity.NoPiecePlacing;

					while (!state.GameHasEnded)
					{
						var sampleSet = state.ActivePlayer == 0 ? p0Samples : p1Samples;

						var sample = _ai.MakeMoveWithResult(state);
						sampleSet.Add(sample);
					}

					//Set the winner on the samples from the winner
					var winningSamples = state.WinningPlayer == 0 ? p0Samples : p1Samples;
					for (var j = 0; j < winningSamples.Count; j++)
						winningSamples[j].IsWin = true;

					//TODO: AddRange is terrible performance
					lock (req)
					{
						req.Samples.AddRange(p0Samples);
						req.Samples.AddRange(p1Samples);
					}
				}//);
				timer.Stop();
				Console.WriteLine("Took " + timer.ElapsedMilliseconds);

				//if (req.Samples.Count >= 256)
				{
					Console.WriteLine("Training " + req.Samples.Count);

					_client.Train(req);

					req.Samples.Clear();

					Evaluate();
				}
			}
			Console.WriteLine("Hi");
			Console.ReadLine();
		}

		private void Evaluate()
		{
			Console.WriteLine($"Evaluating vs " + _opp.Name);

			var players = new IMoveDecisionMaker[] { _ai, _opp };

			//Play X games (even amount), each player gets to start on each of the boards once
			for (var game = 0; game < 4; game++)
			{
				var state = new SimulationState(SimulationHelpers.GetRandomPieces(game / 2), 0);
				state.Fidelity = SimulationFidelity.NoPiecePlacing;
				state.ActivePlayer = game % 2;

				while (!state.GameHasEnded)
				{
					players[state.ActivePlayer].MakeMove(state);
				}

				Console.WriteLine($"Game {game}. Winner {state.WinningPlayer}. Scores: {state.CalculatePlayerEndGameWorth(0)} / {state.CalculatePlayerEndGameWorth(1)}");
			}
		}

	}
}
