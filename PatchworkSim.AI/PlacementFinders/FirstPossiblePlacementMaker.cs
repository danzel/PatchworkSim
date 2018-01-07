using System;

namespace PatchworkSim.AI.PlacementFinders
{
	public class FirstPossiblePlacementMaker : IPlacementDecisionMaker
	{
		public static readonly FirstPossiblePlacementMaker Instance = new FirstPossiblePlacementMaker();

		private FirstPossiblePlacementMaker()
		{
		}

		public void PlacePiece(SimulationState state)
		{
			var ourBoard = state.PlayerBoardState[state.PieceToPlacePlayer];

			foreach (var bitmap in state.PieceToPlace.PossibleOrientations)
			{
				for (var y = 0; y < SimulationState.PlayerBoardSize - bitmap.GetLength(1); y++)
				{
					for (var x = 0; x < SimulationState.PlayerBoardSize - bitmap.GetLength(0); x++)
					{
						if (BitmapOps.CanPlace(ourBoard, bitmap, x, y))
						{
							state.PerformPlacePiece(bitmap, x, y);
							return;
						}
					}
				}
			}

			throw new Exception("There is no where to place the piece");
		}

		public string Name => "FirstPossible";
	}
}
