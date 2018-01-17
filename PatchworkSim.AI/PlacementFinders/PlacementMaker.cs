﻿using System;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;

namespace PatchworkSim.AI.PlacementFinders
{
	public class PlacementMaker : IPlacementDecisionMaker
	{
		public static readonly PlacementMaker FirstPossibleInstance = new PlacementMaker(FirstPossiblePlacementStrategy.Instance);
		public static readonly PlacementMaker SimpleClosestToWallAndCornerInstance = new PlacementMaker(SimpleClosestToWallAndCornerStrategy.Instance);
		public static readonly PlacementMaker ClosestToCornerLeastHolesTieBreakerInstance = new PlacementMaker(ClosestToCornerLeastHolesTieBreakerPlacementStrategy.Instance);
		public static readonly PlacementMaker NextToPieceEdgeLeastHolesTieBreakerInstance = new PlacementMaker(NextToPieceEdgeLeastHolesTieBreakerPlacementStrategy.Instance);
		public static readonly PlacementMaker TightDoublerInstance = new PlacementMaker(TightPlacementStrategy.InstanceDoubler);
		public static readonly PlacementMaker TightIncrementInstance = new PlacementMaker(TightPlacementStrategy.InstanceIncrement);
		public static readonly PlacementMaker ExhaustiveMostFuturePlacementsInstance = new PlacementMaker(ExhaustiveMostFuturePlacementsPlacementStrategy.Instance1);

		private readonly IPlacementStrategy _strategy;

		private PlacementMaker(IPlacementStrategy strategy)
		{
			_strategy = strategy;
		}

		public void PlacePiece(SimulationState state)
		{
			if (_strategy.TryPlacePiece(state.PlayerBoardState[state.PieceToPlacePlayer], state.PieceToPlace, state.Pieces, state.NextPieceIndex, out var bitmap, out var x, out var y))
				state.PerformPlacePiece(bitmap, x, y);
			else
				throw new Exception("There is no where to place the piece");
		}

		public string Name => _strategy.Name;
	}
}