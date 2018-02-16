using System.Linq;

namespace PatchworkSim.AI
{
	public static class Helpers
	{
		private static int[] _buttonIncomeAmountAfterPositionCache;

		static Helpers()
		{
			_buttonIncomeAmountAfterPositionCache = new int[SimulationState.EndLocation + 1];
			for (var i = 0; i < _buttonIncomeAmountAfterPositionCache.Length; i++)
			{
				_buttonIncomeAmountAfterPositionCache[i] = SimulationState.ButtonIncomeMarkers.Count(c => c > i);
			}
		}

		/// <summary>
		/// This runs from the highest x/y backwards, as a rule of thumb placement strategies should try place starting at 0,0 so this is likely to finish quicker
		/// </summary>
		public static bool CanPlace(BoardState board, PieceDefinition piece)
		{
			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (var y = BoardState.Height - bitmap.Height; y >= 0; y--)
				{
					for (var x = BoardState.Width - bitmap.Width; x >= 0 ; x--)
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
			return piece.TotalUsedLocations <= BoardState.Width * BoardState.Height - state.PlayerBoardUsedLocationsCount[state.ActivePlayer] 
				&& piece.ButtonCost <= state.PlayerButtonAmount[state.ActivePlayer] 
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
			return _buttonIncomeAmountAfterPositionCache[currentPosition];
		}

		/// <summary>
		/// Estimates the end game value of the given players position.
		/// Doesn't care that you would be able to spend any income to buy more pieces later. Basically assumes all players advance to end
		/// </summary>
		public static int EstimateEndgameValue(SimulationState state, int player)
		{
			//space used * 2 + buttons + income * incomes remaining
			return state.PlayerBoardUsedLocationsCount[player] * 2 + state.PlayerButtonAmount[player] + state.PlayerButtonIncome[player] * ButtonIncomeAmountAfterPosition(state.PlayerPosition[player]);
		}
	}
}
