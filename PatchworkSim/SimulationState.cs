using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PatchworkSim;

public class SimulationState
{
	/// <summary>
	/// The index of the final location on the board, players cannot advance past this location
	/// </summary>
	public const int EndLocation = 53;

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

	public ISimulationLogger Logger = NullSimulationLogger.Instance;

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
	public BoardState[] PlayerBoardState;

	/// <summary>
	/// How many positions on each players board have been covered by patches (pieces or leather patches)
	/// </summary>
	public FixedArray2Int PlayerBoardUsedLocationsCount;

	/// <summary>
	/// The amount of buttons a player will earn when passing a button income marker
	/// </summary>
	public FixedArray2Int PlayerButtonIncome;

	/// <summary>
	/// How many buttons each player has, each player starts with 5
	/// </summary>
	public FixedArray2Int PlayerButtonAmount;

	/// <summary>
	/// The index of each player on the board, players start at 0
	/// </summary>
	public FixedArray2Int PlayerPosition;

	/// <summary>
	/// The index of the player that has received the 7x7 bonus
	/// </summary>
	public int? SevenXSevenBonusPlayer;

	/// <summary>
	/// The list of pieces (index in to PieceDefinition.AllPieceDefinitions) in picking order
	/// </summary>
	public PieceCollection Pieces;

	/// <summary>
	/// The index (into Pieces) of the next piece (the next three will include the two after that, use modular maths!)
	/// </summary>
	public int NextPieceIndex;

	/// <summary>
	/// The active player is always the player least far down the board, in the case of a tie it is the last player that moved (They will be on top when played with physical pieces)
	/// </summary>
	public int ActivePlayer;

	public int NonActivePlayer => ActivePlayer == 0 ? 1 : 0;

	/// <summary>
	/// After a player has purchased a piece, this will be the piece they need to place.
	/// If they pass a leather patch then that will be placed here too
	/// </summary>
	public PieceDefinition PieceToPlace;

	/// <summary>
	/// Player that owns the PieceToPlace (required as when collecting leather patches during a move, that could make them not the ActivePlayer anymore)
	/// </summary>
	public int PieceToPlacePlayer = -1;

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

	public SimulationState()
		: this(null, 0)
	{
	}

	/// <param name="pieces">This list is not cloned, we take ownership and modify the given list</param> //TODO: THIS COMMENT IS WRONG
	public SimulationState(List<int> pieces, int nextPieceIndex)
	{
		if (pieces != null)
			Pieces.Populate(pieces);
		NextPieceIndex = nextPieceIndex;

		LeatherPatchesIndex = 0;

		Fidelity = SimulationFidelity.FullSimulation;

		PlayerBoardState = new BoardState[2];
		PlayerButtonAmount[0] = PlayerStartingButtons;
		PlayerButtonAmount[1] = PlayerStartingButtons;
		ActivePlayer = 0;
	}

	/// <summary>
	/// Move the active player in front of the other player (A: Advance and Receive Buttons)
	/// </summary>
	public void PerformAdvanceMove()
	{
		if (PieceToPlace != null)
			throw new Exception("Cannot move when there is a piece waiting to be placed");
		if (GameHasEnded)
			throw new Exception("Players cannot make moves, the game has finished");

		var targetPosition = Math.Min(EndLocation, PlayerPosition[NonActivePlayer] + 1);

		//We always get one button for every space we move
		PlayerButtonAmount[ActivePlayer] += targetPosition - PlayerPosition[ActivePlayer];

		int player = ActivePlayer;
		MoveActivePlayer(targetPosition);

		Logger.PlayerAdvanced(player);
	}

	private void MoveActivePlayer(int targetPosition)
	{
#if DEBUG
		if (targetPosition > EndLocation)
			throw new ArgumentOutOfRangeException(nameof(targetPosition), nameof(targetPosition) + " (" + targetPosition + ") is past the EndLocation (" + EndLocation + ")");
		if (targetPosition <= PlayerPosition[ActivePlayer])
			throw new ArgumentOutOfRangeException(nameof(targetPosition), nameof(targetPosition) + " (" + targetPosition + ") is not past the current position of this player");
#endif
		var startPosition = PlayerPosition[ActivePlayer];
		//Move us
		PlayerPosition[ActivePlayer] = targetPosition;


		//Check if we get buttons for passing a button marker
		PlayerButtonAmount[ActivePlayer] += PlayerButtonIncome[ActivePlayer] * (SimulationHelpers.ButtonIncomeAmountAfterPosition(startPosition) - SimulationHelpers.ButtonIncomeAmountAfterPosition(targetPosition));

		//Check if the player gets a leather patch
		if (LeatherPatchesIndex < LeatherPatches.Length && targetPosition >= LeatherPatches[LeatherPatchesIndex])
		{
			//Note: If the player cannot place the patch it is just discarded https://boardgamegeek.com/thread/1538861/cant-put-1x1-patch-what-happens

			if (Fidelity == SimulationFidelity.FullSimulation)
			{
				//Only get the piece if their board isn't full
				if (PlayerBoardUsedLocationsCount[ActivePlayer] < BoardState.Width * BoardState.Height)
				{
					PieceToPlace = PieceDefinition.LeatherTile;
					PieceToPlacePlayer = ActivePlayer;
				}
			}
			else
			{
				//Only get the points if their board isn't full
				if (PlayerBoardUsedLocationsCount[ActivePlayer] < BoardState.Width * BoardState.Height)
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
#if DEBUG
		if (PieceToPlace != null)
			throw new Exception("Cannot purchase a piece when there is one waiting to be placed");
#endif

		//Check the piece is one of the next 3
		pieceIndex = pieceIndex % Pieces.Count;
#if DEBUG
		if (pieceIndex != NextPieceIndex % Pieces.Count && pieceIndex != (NextPieceIndex + 1) % Pieces.Count && pieceIndex != (NextPieceIndex + 2) % Pieces.Count)
			throw new Exception("pieceIndex (" + pieceIndex + ") is not one of the next 3 pieces");
#endif
		var piece = PieceDefinition.AllPieceDefinitions[Pieces[pieceIndex]];

		//Check the player can afford it
#if DEBUG
		if (PlayerButtonAmount[ActivePlayer] < piece.ButtonCost)
			throw new Exception("Player is trying to purchase a piece they cannot afford");
#endif

		//TODO(?) Check the player can place it

		//1. Choose a Patch
		Pieces.RemoveAt(pieceIndex);
		//2. Move the Neutral Token
		NextPieceIndex = pieceIndex % Pieces.Count;
		//3. Pay for the Patch
		PlayerButtonAmount[ActivePlayer] -= piece.ButtonCost;

		Logger.PlayerPurchasedPiece(ActivePlayer, piece);

		if (Fidelity == SimulationFidelity.NoPiecePlacing)
		{
			PerformPurchasePlaceSteps45(piece, ActivePlayer);
		}
		else
		{
			PieceToPlace = piece;
			PieceToPlacePlayer = ActivePlayer;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void PerformPurchasePlaceSteps45(PieceDefinition piece, int player)
	{
		//4. Place the Patch on Your Quilt Board
		PlayerBoardUsedLocationsCount[player] += piece.TotalUsedLocations;
		PlayerButtonIncome[player] += piece.ButtonsIncome;

		//5. Move Your Time Token
		if (piece.TimeCost > 0)
			MoveActivePlayer(Math.Min(EndLocation, PlayerPosition[player] + piece.TimeCost));
	}

	/// <summary>
	/// Place the piece in the given orientation at the given x,y position
	/// </summary>
	public void PerformPlacePiece(PieceBitmap bitmap, int x, int y)
	{
		if (PieceToPlace == null)
			throw new Exception("Cannot place a piece when there is none to place");
		if (!PieceToPlace.PossibleOrientations.Contains(bitmap))
			throw new Exception("Given bitmap does not belong to the piece to be placed");

		PlayerBoardState[PieceToPlacePlayer].Place(bitmap, x, y);

		//Check if they got 7x7 tile
		if (!SevenXSevenBonusPlayer.HasValue && PlayerBoardState[PieceToPlacePlayer].Has7x7Coverage)
		{
			SevenXSevenBonusPlayer = PieceToPlacePlayer;
		}


		var piece = PieceToPlace;
		var player = PieceToPlacePlayer;

		PieceToPlace = null;
		PieceToPlacePlayer = -1;

		PerformPurchasePlaceSteps45(piece, player);

		Logger.PlayerPlacedPiece(player, piece, x, y, bitmap);
	}

	/// <summary>
	/// Calculates the players worth (for end game scoring)
	/// </summary>
	public int CalculatePlayerEndGameWorth(int player)
	{
		int emptySpaces = BoardState.Width * BoardState.Height - PlayerBoardUsedLocationsCount[player];

		var amount = PlayerButtonAmount[player] - 2 * emptySpaces;
		if (SevenXSevenBonusPlayer == player)
			amount += 7;
		return amount;
	}

	public SimulationState Clone()
	{
		var state = new SimulationState();
		CloneTo(state);

		return state;
	}

	public void CloneTo(SimulationState target)
	{
#if DEBUG
		if (Pieces.Count == 0)
			throw new Exception("We dont have pieces WTF");
#endif
		target.Pieces = Pieces;

		target.NextPieceIndex = NextPieceIndex;

		//Copy over other values
		target.LeatherPatchesIndex = LeatherPatchesIndex;

		target.Fidelity = Fidelity;

		target.PlayerBoardState[0] = PlayerBoardState[0];
		target.PlayerBoardState[1] = PlayerBoardState[1];

		target.PlayerBoardUsedLocationsCount = PlayerBoardUsedLocationsCount;

		target.PlayerButtonIncome = PlayerButtonIncome;

		target.PlayerButtonAmount = PlayerButtonAmount;

		target.PlayerPosition = PlayerPosition;

		target.SevenXSevenBonusPlayer = SevenXSevenBonusPlayer;

		target.ActivePlayer = ActivePlayer;

		target.PieceToPlace = PieceToPlace;

		target.PieceToPlacePlayer = PieceToPlacePlayer;
	}
}