using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PatchworkSim.AI
{
	public static class Helpers
	{
		/// <summary>
		/// This runs from the highest x/y backwards, as a rule of thumb placement strategies should try place starting at 0,0 so this is likely to finish quicker
		/// </summary>
		public static bool CanPlace(BoardState board, PieceDefinition piece)
		{
			for (var index = 0; index < piece.PossibleOrientations.Length; index++)
			{
				var bitmap = piece.PossibleOrientations[index];
				for (var y = BoardState.Height - bitmap.Height; y >= 0; y--)
				{
					for (var x = BoardState.Width - bitmap.Width; x >= 0; x--)
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

		/// <summary>
		/// Finds a placement (Not guaranteed to be any good) for the given piece on the given board
		/// </summary>
		public static void GetFirstPlacement(BoardState board, PieceDefinition piece, out PieceBitmap bitmap, out int x, out int y)
		{
			for (var index = 0; index < piece.PossibleOrientations.Length; index++)
			{
				bitmap = piece.PossibleOrientations[index];
				for (y = BoardState.Height - bitmap.Height; y >= 0; y--)
				{
					for (x = BoardState.Width - bitmap.Width; x >= 0; x--)
					{
						if (board.CanPlace(bitmap, x, y))
						{
							return;
						}
					}
				}
			}

			throw new Exception("GetFirstPlacement couldn't find a placement");
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

		/// <summary>
		/// Estimates the end game value of the given players position.
		/// Doesn't care that you would be able to spend any income to buy more pieces later. Basically assumes all players advance to end.
		/// Provides a slight bias towards getting button income early (Useful for minimax style ai)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int EstimateEndgameValue(SimulationState state, int player)
		{
			//space used * 2 + buttons + 1.1 * income * incomes remaining
			return state.PlayerBoardUsedLocationsCount[player] * 2 + state.PlayerButtonAmount[player] + (11 * state.PlayerButtonIncome[player] * SimulationHelpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[player]) / 10);
		}

		/// <summary>
		/// Estimates the end game value of the given players position.
		/// Doesn't care that you would be able to spend any income to buy more pieces later. Basically assumes all players advance to end.
		/// Does not bias getting button income early
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int EstimateEndgameValueUnbiased(SimulationState state, int player)
		{
			//space used * 2 + buttons + income * incomes remaining
			return state.PlayerBoardUsedLocationsCount[player] * 2 + state.PlayerButtonAmount[player] + state.PlayerButtonIncome[player] * SimulationHelpers.ButtonIncomeAmountAfterPosition(state.PlayerPosition[player]);
		}
	}
}
