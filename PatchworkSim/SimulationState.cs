using System;
using System.Collections.Generic;
using System.Linq;

namespace PatchworkSim
{
	public class SimulationState
	{
		/// <summary>
		/// The index of the final location on the board, players cannot advance past this location
		/// </summary>
		public const int EndLocation = 53;

		/// <summary>
		/// After moving from before this index to this index (or greater) a player gains button income
		/// </summary>
		public static readonly int[] ButtonIncomeMarkers = { 5, 11, 17,23, 29, 35, 41, 47, 53 };

		/// <summary>
		/// The indexes of the leather patches. After arriving on (or after) this location, the player must place a 1x1 piece on their board (and remove that item from here, only the first player to arrive gets one)
		/// </summary>
		public List<int> LeatherPatches; //If removing from and cloning this is slow, can be replaced by a static array and an index

		/// <summary>
		/// What positions on each players board have a patch placed on them (pieces or leather patches)
		/// </summary>
		public bool[][,] PlayerBoardState;

		/// <summary>
		/// How many positions on each players board have been covered by patches (pieces or leather patches)
		/// </summary>
		public int[] PlayerBoardUsedLocationsCount;

		/// <summary>
		/// The amount of buttons a player will earn when passing a button income marker
		/// </summary>
		public int[] PlayerButtonIncome;

		/// <summary>
		/// How many buttons each player has, each player starts with 5
		/// </summary>
		public int[] PlayerButtonAmount;

		/// <summary>
		/// The index of each player on the board, players start at 0
		/// </summary>
		public int[] PlayerPosition;

		/// <summary>
		/// The active player is always the player least far down the board, in the case of a tie it is the last player that moved (They will be on top when played with physical pieces)
		/// </summary>
		public int ActivePlayer;

		private int NonActivePlayer { get { return ActivePlayer == 0 ? 1 : 0; } }

		public bool GameHasEnded {  get { return PlayerPosition[0] == EndLocation && PlayerPosition[1] == EndLocation; } }

		public SimulationState()
		{
			LeatherPatches = new List<int> { 20, 26, 32, 44, 50 };

			PlayerBoardState = new[] { new bool[9, 9], new bool[9, 9] };
			PlayerBoardUsedLocationsCount = new[] { 0, 0 };
			PlayerButtonIncome = new[] { 0, 0 };
			PlayerButtonAmount = new[] { 0, 0 };
			PlayerPosition = new[] { 0, 0 };
			ActivePlayer = 0;
		}

		/// <summary>
		/// Move the active player in front of the other player (A: Advance and Receive Buttons)
		/// </summary>
		public void PerformAdvanceMove()
		{
			if (GameHasEnded)
				throw new Exception("Players cannot make moves, the game has finished");

			var targetPosition = Math.Min(EndLocation, PlayerPosition[NonActivePlayer] + 1);

			//We always get one button for every space we move
			PlayerButtonAmount[ActivePlayer] += targetPosition - PlayerPosition[ActivePlayer];

			MoveActivePlayer(targetPosition);
		}

		private void MoveActivePlayer(int targetPosition)
		{
			if (targetPosition > EndLocation)
				throw new ArgumentOutOfRangeException(nameof(targetPosition), nameof(targetPosition) + " (" + targetPosition + ") is past the EndLocation (" + EndLocation + ")");
			if (targetPosition <= PlayerPosition[ActivePlayer])
				throw new ArgumentOutOfRangeException(nameof(targetPosition), nameof(targetPosition) + " (" + targetPosition + ") is not past the current position of this player");

			var startPosition = PlayerPosition[ActivePlayer];
			//Move us
			PlayerPosition[ActivePlayer] = targetPosition;


			//Check if we get buttons for passing a button marker
			if (ButtonIncomeMarkers.Any(b => b > startPosition && b <= targetPosition))
			{
				PlayerButtonAmount[ActivePlayer] += PlayerButtonIncome[ActivePlayer];
			}

			//Check if we get a leather patch to place TODO

			//We moved in front of them, change the active player
			if (PlayerPosition[ActivePlayer] > PlayerPosition[NonActivePlayer])
			{
				ActivePlayer = NonActivePlayer;
			}
		}
	}
}
