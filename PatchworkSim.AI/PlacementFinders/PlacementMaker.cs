using System;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;

namespace PatchworkSim.AI.PlacementFinders
{
	public class PlacementMaker : IPlacementDecisionMaker
	{
		public static readonly PlacementMaker FirstPossibleInstance = new PlacementMaker(FirstPossiblePlacementStrategy.Instance);

		private readonly IPlacementStrategy _strategy;

		private PlacementMaker(FirstPossiblePlacementStrategy strategy)
		{
			_strategy = strategy;
		}

		public void PlacePiece(SimulationState state)
		{
			if (_strategy.TryPlacePiece(state.PlayerBoardState[state.PieceToPlacePlayer], state.PieceToPlace, out var bitmap, out var x, out var y))
				state.PerformPlacePiece(bitmap, x, y);
			else
				throw new Exception("There is no where to place the piece");
		}

		public string Name => _strategy.Name;
	}
}