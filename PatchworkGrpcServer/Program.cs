using Grpc.Core;
using PatchworkSim;
using PatchworkSim.AI.MoveMakers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatchworkGrpcServer;

class Program
{
	const int Port = 50900;

	static void Main(string[] args)
	{
		Server server = new Server
		{
			Services = { PatchworkServer.BindService(new PatchworkServerImpl(new PatchworkService())) },
			Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
		};
		server.Start();

		Console.WriteLine("PatchworkServer listening on port " + Port);
		Console.WriteLine("Press any key to stop the server...");
		Console.ReadKey();

		server.ShutdownAsync().Wait();
	}
}

class PatchworkServerImpl : PatchworkServer.PatchworkServerBase
{
	const int PlayerObservations = 10;
	const int LookAheadPieceAmount = 24;
	const int PieceFields = 3;

	const float PlayerPositionScale = SimulationState.EndLocation;
	static readonly float IncomesRemainingScale = SimulationState.ButtonIncomeMarkers.Length;
	private readonly PatchworkService _patchwork;
	const float ButtonIncomeScale = 40;
	const float ButtonAmountScale = 40;
	const float UsedLocationsScale = BoardState.Width * BoardState.Height;

	public PatchworkServerImpl(PatchworkService patchwork)
	{
		_patchwork = patchwork;
	}
	public override Task<StaticConfigReply> GetStaticConfig(StaticConfigRequest request, ServerCallContext context)
	{
		return Task.FromResult(new StaticConfigReply { ObservationSize = PlayerObservations + LookAheadPieceAmount * PieceFields });
	}

	public override Task<CreateReply> Create(CreateRequest request, ServerCallContext context)
	{
		(var sim, var gameId) = _patchwork.CreateSimulation(null /*request.RandomSeed*/, request.OpponentStrength);
		//Console.WriteLine("Created game " + gameId);

		var res = new CreateReply
		{
			GameId = gameId,
			Observation = new Observation()
		};

		PopulateObservation(res.Observation, sim);
		return Task.FromResult(res);
	}

	public override Task<MoveReply> PerformMove(MoveRequest request, ServerCallContext context)
	{
		var sim = _patchwork.GetState(request.GameId);
		var currentValue = CalculateValue(sim);

		if (sim.GameHasEnded || sim.ActivePlayer != 0)
			throw new Exception("Game is over or it isn't our turn");

		//Perform the move
		bool moveWasInvalid = false;
		if (request.Move > 0)
		{
			var piece = PatchworkSim.AI.Helpers.GetNextPiece(sim, request.Move - 1);
			var canPurchase = PatchworkSim.AI.Helpers.ActivePlayerCanPurchasePiece(sim, piece);

			if (canPurchase)
			{
				sim.PerformPurchasePiece(sim.NextPieceIndex + request.Move - 1);
			}
			else
			{
				moveWasInvalid = true;
				request.Move = 0;
			}
		}
		if (request.Move == 0)
		{
			sim.PerformAdvanceMove();
		}

		//Let opponent do their move (if it is their turn)
		var opponent = _patchwork.GetOpponent(request.GameId);
		while (!sim.GameHasEnded && sim.ActivePlayer == 1)
		{
			opponent.MakeMove(sim);
		}
		if (sim.GameHasEnded)
			_patchwork.RemoveGame(request.GameId);


		//Calculate resulting reward
		var resultingValue = CalculateValue(sim);

		var res = new MoveReply
		{
			GameHasEnded = sim.GameHasEnded,
			Observation = new Observation
			{
				Reward = CalculateReward(sim, moveWasInvalid, currentValue, resultingValue)
			}
		};
		if (sim.GameHasEnded)
		{
			res.WinningPlayer = sim.WinningPlayer;
			//Console.WriteLine("Game Ended " + request.GameId);
		}

		PopulateObservation(res.Observation, sim);

		return Task.FromResult(res);
	}

	private float CalculateReward(SimulationState sim, bool moveWasInvalid, float currentValue, float resultingValue)
	{
		var res = resultingValue - currentValue;
		res *= 10;
		/*if (sim.GameHasEnded)
		{
			if (sim.WinningPlayer == 0)
				res += 1;
			else
				res -= 1;
		}*/

		//if (moveWasInvalid)
		//	res -= 1;

		return res;
	}

	private float CalculateValue(SimulationState sim)
	{
		return CalculateValue(sim, 0) - CalculateValue(sim, 1);
	}

	private float CalculateValue(SimulationState state, int player)
	{
		return state.PlayerBoardUsedLocationsCount[player] * 2 / UsedLocationsScale +
			state.PlayerButtonAmount[player] / ButtonAmountScale +
			state.PlayerButtonIncome[player] * SimulationHelpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[player]) / ButtonIncomeScale;
	}

	private void PopulateObservation(Observation res, SimulationState sim)
	{
		res.ObservationForNextMove.Add(sim.PlayerButtonIncome[0] / ButtonIncomeScale);
		res.ObservationForNextMove.Add(sim.PlayerButtonIncome[1] / ButtonIncomeScale);
		res.ObservationForNextMove.Add(sim.PlayerButtonAmount[0] / ButtonAmountScale);
		res.ObservationForNextMove.Add(sim.PlayerButtonAmount[1] / ButtonAmountScale);
		res.ObservationForNextMove.Add(sim.PlayerBoardUsedLocationsCount[0] / UsedLocationsScale);
		res.ObservationForNextMove.Add(sim.PlayerBoardUsedLocationsCount[1] / UsedLocationsScale);
		res.ObservationForNextMove.Add(sim.PlayerPosition[0] / PlayerPositionScale);
		res.ObservationForNextMove.Add(sim.PlayerPosition[1] / PlayerPositionScale);
		res.ObservationForNextMove.Add(SimulationHelpers.ButtonIncomeAmountAfterPosition(sim.PlayerPosition[0]) / IncomesRemainingScale);
		res.ObservationForNextMove.Add(SimulationHelpers.ButtonIncomeAmountAfterPosition(sim.PlayerPosition[1]) / IncomesRemainingScale);

		for (var i = 0; i < LookAheadPieceAmount; i++)
		{
			var piece = PatchworkSim.AI.Helpers.GetNextPiece(sim, i);

			res.ObservationForNextMove.Add(piece.ButtonCost / ButtonAmountScale);
			res.ObservationForNextMove.Add(piece.ButtonsIncome / ButtonIncomeScale);
			res.ObservationForNextMove.Add(piece.TotalUsedLocations / UsedLocationsScale);
		}
	}
}

/// <summary>
/// Holds Patchwork instances
/// </summary>
public class PatchworkService
{
	private readonly Dictionary<int, SimulationState> _simulations = new Dictionary<int, SimulationState>();
	private readonly Dictionary<int, MoveOnlyMonteCarloTreeSearchMoveMaker> _opponents = new Dictionary<int, MoveOnlyMonteCarloTreeSearchMoveMaker>();
	private int _nextSim;

	/// <summary>
	/// Constructor
	/// </summary>
	public PatchworkService()
	{
	}

	internal (SimulationState state, int gameId) CreateSimulation(int? randomSeed, int mctsIterations)
	{
		lock (this)
		{
			var id = ++_nextSim;

			var state = new SimulationState(SimulationHelpers.GetRandomPieces(randomSeed), 0);
			state.Fidelity = SimulationFidelity.NoPiecePlacing;
			var opp = new MoveOnlyMonteCarloTreeSearchMoveMaker(mctsIterations);

			_simulations[id] = state;
			_opponents[id] = opp;

			return (state, id);
		}
	}

	internal SimulationState GetState(int gameId)
	{
		lock (this)
			return _simulations[gameId];
	}

	internal IMoveDecisionMaker GetOpponent(int gameId)
	{
		lock (this)
			return _opponents[gameId];
	}

	internal void RemoveGame(int gameId)
	{
		lock (this)
		{
			_simulations.Remove(gameId);
			_opponents.Remove(gameId);
		}
	}
}
