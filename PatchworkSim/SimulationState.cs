using System;
using System.Collections;
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
		/// The width and height of the board the player places pieces they purchase on
		/// </summary>
		public const int PlayerBoardSize = 9;

		/// <summary>
		/// How many buttons each player starts the game with
		/// </summary>
		public const int PlayerStartingButtons = 5;

		/// <summary>
		/// After moving from before this index to this index (or greater) a player gains button income
		/// </summary>
		public static readonly int[] ButtonIncomeMarkers = { 5, 11, 17, 23, 29, 35, 41, 47, 53 };

		/// <summary>
		/// The indexes of the leather patches. After arriving on (or after) this location, the player must place a 1x1 piece on their board
		/// </summary>
		public static readonly int[] LeatherPatches = { 20, 26, 32, 44, 50 };

		/// <summary>
		/// The index of the next LeatherPatch to give (When someone passes it), or the length of the array if all have been claimed
		/// </summary>
		public int LeatherPatchesIndex;

		/// <summary>
		/// The quality of the simulation
		/// </summary>
		public SimulationFidelity Fidelity;

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
		/// The list of pieces (index in to PieceDefinition.AllPieceDefinitions) in picking order
		/// </summary>
		public List<int> Pieces;

		/// <summary>
		/// The index (into Pieces) of the next piece (the next three will include the two after that, use modular maths!)
		/// </summary>
		public int NextPieceIndex;

		/// <summary>
		/// The active player is always the player least far down the board, in the case of a tie it is the last player that moved (They will be on top when played with physical pieces)
		/// </summary>
		public int ActivePlayer;

		private int NonActivePlayer => ActivePlayer == 0 ? 1 : 0;

		//TODO LATER
		/*
		/// <summary>
		/// After a player has purchased a piece, this will be the piece, they need to place.
		/// If they pass a leather patch then that will be placed here too
		/// </summary>
		public PieceDefinition PieceToPlace;

		/// <summary>
		/// Player that owns the PieceToPlace (required as collects leather patches during move, that could make them not the ActivePlayer anymore)
		/// </summary>
		public int PieceToPlacePlayer;
		*/
		public bool GameHasEnded
		{
			get { return PlayerPosition[0] == EndLocation && PlayerPosition[1] == EndLocation; }
		}

		public int WinningPlayer
		{
			get
			{
				if (!GameHasEnded)
					throw new Exception("Game has not ended");

				var totalWorth0 = CalculatePlayerEndGameWorth(0);
				var totalWorth1 = CalculatePlayerEndGameWorth(1);

				if (totalWorth0 > totalWorth1)
					return 0;
				if (totalWorth1 > totalWorth0)
					return 1;
				//Otherwise whoever arrived at the end first wins, this will be the non-active player (as the active player will be on top of the non-active player at the end location)
				return NonActivePlayer;
			}
		}

		public SimulationState(IList<int> pieces, int nextPieceIndex)
		{
			Pieces = pieces.ToList();
			NextPieceIndex = nextPieceIndex;

			LeatherPatchesIndex = 0;

			Fidelity = SimulationFidelity.FullSimulation;

			PlayerBoardState = new[] { new bool[PlayerBoardSize, PlayerBoardSize], new bool[PlayerBoardSize, PlayerBoardSize] };
			PlayerBoardUsedLocationsCount = new[] { 0, 0 };
			PlayerButtonIncome = new[] { 0, 0 };
			PlayerButtonAmount = new[] { PlayerStartingButtons, PlayerStartingButtons };
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

			//Check if the player gets a leather patch
			if (LeatherPatchesIndex < LeatherPatches.Length && targetPosition >= LeatherPatches[LeatherPatchesIndex])
			{
				//Note: If the player cannot place the patch it is just discarded https://boardgamegeek.com/thread/1538861/cant-put-1x1-patch-what-happens

				if (Fidelity == SimulationFidelity.FullSimulation)
				{
					//Check if we get a leather patch to place TODO
					throw new Exception("Leather patch check is not implemented");
				}
				else
				{
					//Only get the points if their board isn't full
					if (PlayerBoardUsedLocationsCount[ActivePlayer] < PlayerBoardSize * PlayerBoardSize)
						PlayerBoardUsedLocationsCount[ActivePlayer]++;
				}

				LeatherPatchesIndex++;
			}

			//We moved in front of them, change the active player
			if (PlayerPosition[ActivePlayer] > PlayerPosition[NonActivePlayer])
			{
				ActivePlayer = NonActivePlayer;
			}
		}

		/// <summary>
		/// Purchase the given piece for placement (B: Take and Place a Patch)
		/// </summary>
		/// <param name="pieceIndex">The index of the patch in the Pieces List</param>
		public void PerformPurchasePiece(int pieceIndex)
		{
			//Check the piece is one of the next 3
			if (pieceIndex != NextPieceIndex && pieceIndex % Pieces.Count != (NextPieceIndex + 1) % Pieces.Count && pieceIndex % Pieces.Count != (NextPieceIndex + 2) % Pieces.Count)
				throw new Exception("pieceIndex (" + pieceIndex + ") is not one of the next 3 pieces");
			var piece = PieceDefinition.AllPieceDefinitions[Pieces[pieceIndex]];

			//Check the player can afford it
			if (PlayerButtonAmount[ActivePlayer] < piece.ButtonCost)
				throw new Exception("Player is trying to purchase a piece they cannot afford");

			//TODO(?) Check the player can place it

			//1. Choose a Patch
			Pieces.RemoveAt(pieceIndex);
			//2. Move the Neutral Token
			NextPieceIndex = pieceIndex;
			//3. Pay for the Patch
			PlayerButtonAmount[ActivePlayer] -= piece.ButtonCost;

			if (Fidelity == SimulationFidelity.NoPiecePlacing)
			{
				//4. Place the Patch on Your Quilt Board
				PlayerBoardUsedLocationsCount[ActivePlayer] += piece.TotalUsedLocations;
				PlayerButtonIncome[ActivePlayer] += piece.ButtonsIncome;

				//5. Move Your Time Token
				MoveActivePlayer(Math.Min(EndLocation, PlayerPosition[ActivePlayer] + piece.TimeCost));
			}
			else
			{
				throw new NotImplementedException("Cannot purchase a piece in hifi");
				//4. Place the Patch on Your Quilt Board
				//TODO: Get the player to place it (And update their UsedLocationsCount + ButtonIncome)
				//5. Move Your Time Token
				//TODO: Move the player
			}
		}

		/// <summary>
		/// Calculates the players worth (for end game scoring)
		/// </summary>
		public int CalculatePlayerEndGameWorth(int player)
		{
			int emptySpaces = PlayerBoardSize * PlayerBoardSize - PlayerBoardUsedLocationsCount[player];

			//TODO: 7x7 special tile
			return PlayerButtonAmount[player] - 2 * emptySpaces;
		}
	}
}