using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PatchworkSim;
using PatchworkWebRunner.Services;
using System;

namespace PatchworkWebRunner.Controllers;

/// <summary>
/// Interacts with PatchworkSim
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PatchworkController : Controller
{
	private readonly ILogger<PatchworkController> _logger;
	private readonly PatchworkService _patchwork;

	const int PlayerObservations = 10;
	const int LookAheadPieceAmount = 12;
	const int PieceFields = 3;

	const float PlayerPositionScale = SimulationState.EndLocation;
	static readonly float IncomesRemainingScale = SimulationState.ButtonIncomeMarkers.Length;
	const float ButtonIncomeScale = 40;
	const float ButtonAmountScale = 40;
	const float UsedLocationsScale = BoardState.Width * BoardState.Height;

	/// <summary>
	/// Constructor
	/// </summary>
	public PatchworkController(ILogger<PatchworkController> logger, PatchworkService patchwork)
	{
		_logger = logger;
		_patchwork = patchwork;
	}

	/// <summary>
	/// Tells you how big the observation array returned will be
	/// </summary>
	/// <returns></returns>
	[HttpGet]
	[Route("observation-size")]
	public int ObservationSize()
	{
		return PlayerObservations + LookAheadPieceAmount * PieceFields;
	}

	/// <summary>
	/// Create a new simulation, remote player always plays first
	/// </summary>
	[HttpPost]
	[Route("create")]
	public CreateResponse Create([FromBody] CreateRequest value)
	{
		(var sim, var gameId) = _patchwork.CreateSimulation(value.RandomSeed, value.OpponentStrength);


		var res = new CreateResponse { GameId = gameId, Reward = 0, ObservationForNextMove = CreateObservation(sim) };
		return res;
	}

	/// <summary>
	/// Perform the given move, returning the new observation and reward
	/// </summary>
	[HttpPost]
	[Route("perform-move")]
	public MoveResponse PerformMove([FromBody] MoveRequest move)
	{
		var sim = _patchwork.GetState(move.GameId);
		var currentValue = CalculateValue(sim);

		if (sim.GameHasEnded || sim.ActivePlayer != 0)
			throw new Exception("Game is over or it isn't our turn");

		//Perform the move
		bool moveWasInvalid = false;
		if (move.Move > 0)
		{
			var piece = PatchworkSim.AI.Helpers.GetNextPiece(sim, move.Move - 1);
			var canPurchase = PatchworkSim.AI.Helpers.ActivePlayerCanPurchasePiece(sim, piece);

			if (canPurchase)
			{
				sim.PerformPurchasePiece(sim.NextPieceIndex + move.Move - 1);
			}
			else
			{
				moveWasInvalid = true;
				move.Move = 0;
			}
		}
		if (move.Move == 0)
		{
			sim.PerformAdvanceMove();
		}

		//Let opponent do their move (if it is their turn)
		var opponent = _patchwork.GetOpponent(move.GameId);
		while (!sim.GameHasEnded && sim.ActivePlayer == 1)
		{
			opponent.MakeMove(sim);
		}
		if (sim.GameHasEnded)
			_patchwork.RemoveGame(move.GameId);


		//Calculate resulting reward
		var resultingValue = CalculateValue(sim);

		var res = new MoveResponse
		{
			Reward = CalculateReward(sim, moveWasInvalid, currentValue, resultingValue),
			GameHasEnded = sim.GameHasEnded,
			ObservationForNextMove = CreateObservation(sim)
		};
		if (sim.GameHasEnded)
			res.WinningPlayer = sim.WinningPlayer;

		return res;
	}

	private float CalculateReward(SimulationState sim, bool moveWasInvalid, float currentValue, float resultingValue)
	{
		var res = resultingValue - currentValue;
		if (sim.GameHasEnded)
		{
			if (sim.WinningPlayer == 0)
				res += 1;
			else
				res -= 1;
		}

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

	private float[] CreateObservation(SimulationState sim)
	{
		var observationForNextMove = new float[PlayerObservations + LookAheadPieceAmount * PieceFields];
		observationForNextMove[0] = sim.PlayerButtonIncome[0] / ButtonIncomeScale;
		observationForNextMove[1] = sim.PlayerButtonIncome[1] / ButtonIncomeScale;
		observationForNextMove[2] = sim.PlayerButtonAmount[0] / ButtonAmountScale;
		observationForNextMove[3] = sim.PlayerButtonAmount[1] / ButtonAmountScale;
		observationForNextMove[4] = sim.PlayerBoardUsedLocationsCount[0] / UsedLocationsScale;
		observationForNextMove[5] = sim.PlayerBoardUsedLocationsCount[1] / UsedLocationsScale;
		observationForNextMove[6] = sim.PlayerPosition[0] / PlayerPositionScale;
		observationForNextMove[7] = sim.PlayerPosition[1] / PlayerPositionScale;
		observationForNextMove[8] = SimulationHelpers.ButtonIncomeAmountAfterPosition(sim.PlayerPosition[0]) / IncomesRemainingScale;
		observationForNextMove[9] = SimulationHelpers.ButtonIncomeAmountAfterPosition(sim.PlayerPosition[1]) / IncomesRemainingScale;

		int baseIndex = PlayerObservations;

		for (var i = 0; i < LookAheadPieceAmount; i++)
		{
			var piece = PatchworkSim.AI.Helpers.GetNextPiece(sim, i);

			var b = baseIndex + (i * PieceFields);
			observationForNextMove[b + 0] = piece.ButtonCost / ButtonAmountScale;
			observationForNextMove[b + 1] = piece.ButtonsIncome / ButtonIncomeScale;
			observationForNextMove[b + 2] = piece.TotalUsedLocations / UsedLocationsScale;
		}

		return observationForNextMove;
	}

	/// <summary>
	/// Observation details
	/// </summary>
	public class Observation
	{
		/// <summary>
		/// Observation array
		/// </summary>
		public required float[] ObservationForNextMove { get; set; }

		/// <summary>
		/// Reward gained from the previous move.
		/// If the provided move was invalid, -1 will be added (and the 'move ahead' move will be performed)
		/// If the game is won, 1 will be added.
		/// If the game is lost, -1 will be added.
		/// </summary>
		public float Reward { get; set; }
	}

	/// <summary>
	/// Create a new simulation
	/// </summary>
	public class CreateRequest
	{
		/// <summary>
		/// RandomSeed to order the pieces using
		/// </summary>
		public int? RandomSeed { get; set; }

		/// <summary>
		/// Iterations the MCTS opponent will run
		/// </summary>
		public int OpponentStrength { get; set; }
	}

	/// <summary>
	/// Result of creating a new simulation
	/// </summary>
	public class CreateResponse : Observation
	{
		/// <summary>
		/// The GameId to use for further interaction with this game
		/// </summary>
		public int GameId { get; set; }
	}

	/// <summary>
	/// Details to perform a move
	/// </summary>
	public class MoveRequest
	{
		/// <summary>
		/// ID of the game we are performing a move on
		/// </summary>
		public int GameId { get; set; }

		/// <summary>
		/// What move to make. 0: move, 1-3 purchase piece 0-2
		/// </summary>
		public int Move { get; set; }
	}

	/// <summary>
	/// Result of performing a move
	/// </summary>
	public class MoveResponse : Observation
	{
		/// <summary>
		/// True if the game has ended
		/// </summary>
		public bool GameHasEnded { get; set; }

		/// <summary>
		/// Index of player that won (you want 0)
		/// </summary>
		public int WinningPlayer { get; set; }
	}
}
