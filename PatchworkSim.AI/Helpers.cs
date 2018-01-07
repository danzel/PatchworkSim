using System.Linq;

namespace PatchworkSim.AI
{
	public static class Helpers
	{
		public static bool CanPlace(bool[,] board, PieceDefinition piece)
		{
			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (var y = 0; y < SimulationState.PlayerBoardSize - bitmap.GetLength(1); y++)
				{
					for (var x = 0; x < SimulationState.PlayerBoardSize - bitmap.GetLength(0); x++)
					{
						if (BitmapOps.CanPlace(board, bitmap, x, y))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public static bool ActivePlayerCanPurchasePiece(SimulationState state, PieceDefinition piece)
		{
			return piece.TotalUsedLocations < SimulationState.PlayerBoardSize * SimulationState.PlayerBoardSize - state.PlayerBoardUsedLocationsCount[state.ActivePlayer] 
				&& piece.ButtonCost < state.PlayerButtonAmount[state.ActivePlayer] 
				&& (state.Fidelity == SimulationFidelity.NoPiecePlacing || CanPlace(state.PlayerBoardState[state.ActivePlayer], piece));
		}

		/// <summary>
		/// Gets the next piece available for purchasing, provide 0-2 to get the pieces that can be purchased
		/// </summary>
		public static PieceDefinition GetNextPiece(SimulationState state, int lookForwardAmount)
		{
			return PieceDefinition.AllPieceDefinitions[state.Pieces[(state.NextPieceIndex + lookForwardAmount) % state.Pieces.Count]];
		}

		public static int ButtonIncomeAmountAfterPosition(int currentPosition)
		{
			//TODO: This could be a normal loop or an array lookup or some other faster method
			return SimulationState.ButtonIncomeMarkers.Count(c => c > currentPosition);
		}
	}
}
