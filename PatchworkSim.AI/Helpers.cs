using System.Linq;

namespace PatchworkSim.AI
{
	public static class Helpers
	{
		/// <summary>
		/// This runs from the highest x/y backwards, as a rule of thumb placement strategies should try place starting at 0,0 so this is likely to finish quicker
		/// </summary>
		public static bool CanPlace(BoardState board, PieceDefinition piece)
		{
			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (var y = BoardState.Height - bitmap.GetLength(1) - 1; y >= 0; y--)
				{
					for (var x = BoardState.Width - bitmap.GetLength(0) - 1; x >= 0 ; x--)
					{
						if (board.CanPlace(bitmap, x, y))
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
			return piece.TotalUsedLocations < BoardState.Width * BoardState.Height - state.PlayerBoardUsedLocationsCount[state.ActivePlayer] 
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
