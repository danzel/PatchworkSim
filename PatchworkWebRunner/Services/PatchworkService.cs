using Microsoft.Extensions.Logging;
using PatchworkSim;
using PatchworkSim.AI.MoveMakers;
using System;
using System.Collections.Generic;

namespace PatchworkWebRunner.Services
{
	/// <summary>
	/// Holds Patchwork instances
	/// </summary>
	public class PatchworkService
	{
		private readonly ILogger<PatchworkService> _logger;

		private readonly Dictionary<int, SimulationState> _simulations = new Dictionary<int, SimulationState>();
		private readonly Dictionary<int, MoveOnlyMonteCarloTreeSearchMoveMaker> _opponents = new Dictionary<int, MoveOnlyMonteCarloTreeSearchMoveMaker>();
		private int _nextSim;

		/// <summary>
		/// Constructor
		/// </summary>
		public PatchworkService(ILogger<PatchworkService> logger)
		{
			_logger = logger;
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
}
